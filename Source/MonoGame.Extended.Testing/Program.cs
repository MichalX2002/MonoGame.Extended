using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;

namespace MonoGame.Extended.Testing
{
    class Frame : Game
    {
        private GraphicsDeviceManager _graphicsManager;
        private SpriteBatch _batch;
        private BitmapFont _font;
        private Color _clearColor = Color.DarkSlateBlue * 0.33f;

        private ResourceManager _resourceManager;

        private Stopwatch _watch = new Stopwatch();
        private Queue<double> _lastTimes = new Queue<double>(new double[] { 0 });
        private Random _rng = new Random();
        
        private Matrix graphicsMatrix;
        private List<PostGraphicsObject> postGraphics = new List<PostGraphicsObject>();

        private Vector2 offset = new Vector2(10, 10);
        private Vector2 scrollOffset;
        private float lastScroll;
        private float smoothScroll;
        private float layoutHeight;

        int _postsLeftToLoad;
        int _firstVisibleGraphic;
        int _lastVisibleGraphic;

        public Frame()
        {
            _graphicsManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Window.AllowUserResizing = true;
            IsMouseVisible = true;

            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            _batch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<BitmapFont>("Sensation");

            Task.Run(StartLoadingSubreddit);
            
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            _resourceManager?.Dispose();
            _processPosts = false;

            base.UnloadContent();
        }

        private bool _processPosts = true;

        private async void StartLoadingSubreddit()
        {
            try
            {
                var reddit = new RedditService();
                var subreddit = await reddit.GetSubredditAsync("Terraria");

                _resourceManager = new ResourceManager();

                _postsLeftToLoad = 20;
                var postEnumerator = subreddit.EnumeratePosts(1500, 15).GetEnumerator();

                var watch = new Stopwatch();
                double totalTime = 0;
                int c = 0;

                while (_processPosts)
                {
                    while (_postsLeftToLoad > 0 && postEnumerator.MoveNext() && _processPosts)
                    {
                        var post = postEnumerator.Current;
                        _postsLeftToLoad--;

                        c++;
                        //if (!post.HasThumbnail) continue;

                        var obj = new PostGraphicsObject(post, GraphicsDevice, _resourceManager);
                        obj.RenderMainText(_font, Color.White, Vector2.One);
                        obj.RenderStatusText(_font, Color.White, Vector2.One);
                        postGraphics.Add(obj);

                        watch.Restart();
                        DoObjectLayout();
                        watch.Stop();

                        totalTime += watch.Elapsed.TotalMilliseconds;
                        //Console.WriteLine("Layout took " + (int)(watch.Elapsed.TotalMilliseconds * 1000) / 1000f + "ms");
                    }
                    Thread.Sleep(10);
                }

                Console.WriteLine("Layout of " + postGraphics.Count + " posts took " + (int)(totalTime * 10) / 10f + "ms");
                Console.WriteLine("Posts iterated: " + c);
            }
            catch(Exception exc)
            {
                Console.WriteLine("Loading subreddit failed: " + exc.Message);
            }
        }

        protected override void Update(GameTime time)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Viewport view = GraphicsDevice.Viewport;
            UpdateScroll(view, time);
            UpdateObjectVisibility(view, out _firstVisibleGraphic, out _lastVisibleGraphic);

            if (_firstVisibleGraphic != -1 && _lastVisibleGraphic != -1)
            {
                LoadThumbnails(_firstVisibleGraphic, _lastVisibleGraphic, 6);

                if(_postsLeftToLoad == 0 && _lastVisibleGraphic + 15 > postGraphics.Count)
                {
                    _postsLeftToLoad += 5;
                }
            }

            base.Update(time);
        }

        protected override void Draw(GameTime time)
        {
            GraphicsDevice.Clear(_clearColor);

            _watch.Restart();
            DrawGraphicTextures();
            DrawGraphicText();
            _watch.Stop();

            //_lastTimes.Enqueue(_watch.Elapsed.TotalMilliseconds);
            //if (_lastTimes.Count > 20)
            //    _lastTimes.Dequeue();

            _batch.Begin();
            int gCount = _lastVisibleGraphic - _firstVisibleGraphic;
            _batch.DrawString(_font, gCount + "/" + postGraphics.Count, new Vector2(1, 0), Color.Green);
            //_batch.DrawString(_font, _firstVisibleGraphic + " - " + _lastVisibleGraphic, new Vector2(1, 30), Color.Red);

            _batch.End();

            if(_resourceManager != null)
                DrawDownloaderDebug(_resourceManager.Downloader);

            base.Draw(time);
        }

        private void UpdateObjectVisibility(Viewport view, out int firstVisible, out int lastVisible)
        {
            Vector2 totalOffset = offset + scrollOffset;
            graphicsMatrix = Matrix.CreateTranslation(totalOffset.X, totalOffset.Y, 0);

            firstVisible = -1;
            lastVisible = -1;
            for (int i = 0; i < postGraphics.Count; i++)
            {
                var graphic = postGraphics[i];
                graphic.CheckVisibility(view, totalOffset);

                if (graphic.IsVisible)
                {
                    lastVisible = i;
                    if (firstVisible == -1)
                        firstVisible = i;
                }
            }
        }

        private void LoadThumbnails(int firstVisible, int lastVisible, int extra)
        {
            int last = Math.Min(postGraphics.Count, lastVisible + extra);
            for (int i = firstVisible; i < last; i++)
            {
                var graphic = postGraphics[i];
                if (graphic.HasThumbnail)
                {
                    graphic.RequestThumbnail();
                    graphic.UploadThumbnail(GraphicsDevice);
                }
            }
        }

        private void DoObjectLayout()
        {
            float graphicOffsetY = 0;
            for (int i = 0; i < postGraphics.Count; i++)
            {
                var graphic = postGraphics[i];
                graphic.DoLayout(graphicOffsetY);
                graphicOffsetY += graphic.Size.Height + 10;
            }
            layoutHeight = graphicOffsetY;
        }

        private void UpdateScroll(Viewport view, GameTime time)
        {
            float newScroll = Mouse.GetState().ScrollWheelValue;
            float scroll = newScroll - lastScroll;
            lastScroll = newScroll;
            smoothScroll = MathHelper.Lerp(smoothScroll + scroll, 0, time.Delta * 8f);

            const float scrollUpThreshold = 12;
            float scrollDownThreshold = -layoutHeight + view.Height - 12;

            if (scrollOffset.Y > scrollUpThreshold)
                scrollOffset.Y = MathHelper.Lerp(scrollOffset.Y, scrollUpThreshold, time.Delta * 30f);
            else if (scrollOffset.Y < scrollDownThreshold)
                scrollOffset.Y = MathHelper.Lerp(scrollOffset.Y, scrollDownThreshold, time.Delta * 30f);

            scrollOffset.Y += MathHelper.Clamp(smoothScroll * 13.2f * time.Delta, -350, 350);
        }

        private void DrawGraphicTextures()
        {
            _batch.Begin(transformMatrix: graphicsMatrix);
            for (int i = _firstVisibleGraphic; i <= _lastVisibleGraphic + 1; i++)
            {
                if (i < 0 || i >= postGraphics.Count)
                    break;

                var graphic = postGraphics[i];
                if (!graphic.IsVisible)
                    continue;

                if (graphic.Thumbnail != null)
                    _batch.Draw(graphic.Thumbnail, graphic.ThumbnailDst.ToRectangle(), Color.White);
                _batch.DrawRectangle(new RectangleF(graphic.Position, graphic.Size), Color.Red, 1);
            }
            _batch.End();
        }

        private void DrawGraphicText()
        {
            _batch.Begin(transformMatrix: graphicsMatrix);
            for (int i = _firstVisibleGraphic; i <= _lastVisibleGraphic; i++)
            {
                if (i < 0 || i >= postGraphics.Count)
                    break;

                var graphic = postGraphics[i];
                if (!graphic.IsVisible)
                    continue;

                _batch.DrawString(graphic.CachedMainText, graphic.MainTextPosition);
                _batch.DrawString(graphic.CachedStatusText, graphic.StatusTextPosition);
            }
            _batch.End();
        }

        private void DrawDownloaderDebug(ResourceDownloader downloader)
        {
            _batch.Begin();
            
            int threadCount = downloader.Threads.Count;
            int size = MathHelper.Clamp((int)Math.Sqrt(threadCount), 1, 3);
            float width = size * 50;
            float startX = GraphicsDevice.Viewport.Width - width;
            float offX = 0;
            float offY = 5;
            
            for (int i = 0; i < threadCount; i++)
            {
                var request = downloader.Threads[i].CurrentRequest;
                
                Vector2 pos = new Vector2(startX + offX, offY);
                offX += 50;
                if(offX >= width)
                {
                    offX = 0;
                    offY += 35;
                }

                bool working = request == null ? false : true;
                Color brickColor = working ? Color.LimeGreen : Color.PaleVioletRed;
                _batch.DrawFilledRectangle(new RectangleF(pos.X, pos.Y, 45, 30), brickColor);

                double progress = 0;
                if (working)
                {
                    if (request.ContentLength > 0 && request.BytesDownloaded >= 0)
                        progress = (double)request.BytesDownloaded / request.ContentLength;
                }

                string ps = (working ? (int)(progress * 100f) : -1).ToString();
                _batch.DrawString(_font, ps, pos + new Vector2(9, 5), Color.DarkBlue);
            }
            
            _batch.End();
        }
    }
   
    class Program
    {
        private static void Main(string[] args)
        {
            using (var frame = new Frame())
                frame.Run();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}