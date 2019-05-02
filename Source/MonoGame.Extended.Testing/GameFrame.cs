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
        private static readonly char[] measurementChars = new[] { ' ', 'a', 'b', 'c', 'd', 'A', 'B', '1', '2', '3' };

        private GraphicsDeviceManager _graphicsManager;
        private SpriteBatch _spriteBatch;
        private BitmapFont _font26;
        private BitmapFont _font40;
        private Color _clearColor = Color.DarkSlateBlue * 0.33f;

        private ResourceManager _resourceManager;

        private Stopwatch _watch = new Stopwatch();
        private Random _rng = new Random();
        
        private Matrix _graphicsMatrix;
        private List<PostGraphicsObject> _postGraphics = new List<PostGraphicsObject>();
        private CircularProgressBar _postLoadingBar;

        private Vector2 offset = new Vector2(10, 10);
        private Vector2 scrollOffset;
        private float lastScroll;
        private float smoothScroll;
        private float layoutHeight;
        private bool _processPosts = true;

        private int _postsLeftToLoad;
        private int _firstVisibleGraphic;
        private int _lastVisibleGraphic;

        public PostGraphicsObject HoveredPost;
        public PostGraphicsObject OpenPost;

        //private void TextDrawTest(GameTime time)
        //{
        //    _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        //
        //    string str = "w"; //tf big boi\nok this weird";
        //
        //    float s = 5f;
        //
        //    SizeF measure = _font40.MeasureString(str);
        //    Rectangle clip = new Rectangle(130, 115, 70, (int)measure.Height + 15);
        //
        //    float x = (float)(Math.Sin(time.TotalGameTime.TotalSeconds * 1.1f)) * 60 + 155 -10 * s;
        //    //float x = 170;                                                               
        //    float y = (float)(Math.Sin(time.TotalGameTime.TotalSeconds * 1.1f)) * 60 + 115 -10 * s;
        //    _spriteBatch.DrawString(_font40, str, new Vector2(x, y), Color.White, 0, Vector2.Zero, s, SpriteEffects.None, 0, clip);
        //
        //    //_spriteBatch.DrawString(_font40, str, new Vector2(x - 50,  y), Color.White, 0, Vector2.Zero, s, SpriteEffects.None, 0, null);
        //    //_spriteBatch.DrawString(_font40, str, new Vector2(x + 50,  y), Color.White, 0, Vector2.Zero, s, SpriteEffects.None, 0, null);
        //
        //    _spriteBatch.DrawRectangle(clip, Color.Red);
        //
        //    _spriteBatch.End();
        //}

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

            Input.AddWindow(Window);
            Window.ClientSizeChanged += Window_ClientSizeChanged;
        }

        private void Window_ClientSizeChanged()
        {
            lock (_postGraphics)
            {
                for (int i = 0; i < _postGraphics.Count; i++)
                    _postGraphics[i].IsTextDirty = true;
            }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font26 = Content.Load<BitmapFont>("LiberationSerif Regular 26px");
            _font40 = Content.Load<BitmapFont>("LiberationSerif Regular 40px");

            _postLoadingBar = new CircularProgressBar(150);

            Task.Run(StartLoadingSubreddit);

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            _processPosts = false;
            _resourceManager?.Dispose();

            base.UnloadContent();
        }

        private float GetAverageFontCharWidth(BitmapFont font)
        {
            float total = 0;
            int chars = 0;
            for (int i = 0; i < measurementChars.Length; i++)
            {
                if (font.GetCharacterRegion(measurementChars[i], out var region))
                {
                    total += font.LetterSpacing + region.XAdvance;
                    chars++;
                }
            }
            return total / chars;
        }

        private float GetMaxCharsInView(BitmapFont font, float width)
        {
            float avgWidth = GetAverageFontCharWidth(font);
            return width / avgWidth;
        }

        private int GetMaxCharsInTitle(BitmapFont font, float width, Vector2 scale)
        {
            return (int)(GetMaxCharsInView(font, width) * scale.X);
        }
        
        private async void StartLoadingSubreddit()
        {
            try
            {
                var reddit = new RedditService();
                var subreddit = await reddit.GetSubredditAsync("Terraria");

                _resourceManager = new ResourceManager("Cache");
                _resourceManager.Start();

                _postsLeftToLoad = 20;
                var postEnumerator = subreddit.EnumeratePosts(10000, 25).GetEnumerator();
                
                int c = 0;

                while (_processPosts)
                {
                    while (_postsLeftToLoad > 0 && postEnumerator.MoveNext() && _processPosts)
                    {
                        var post = postEnumerator.Current;
                        _postsLeftToLoad--;

                        c++;
                        //if (!post.HasThumbnail) continue;

                        var graphic = new PostGraphicsObject(post, GraphicsDevice, _resourceManager);
                        lock (_postGraphics)
                            _postGraphics.Add(graphic);

                        DoObjectLayout();
                    }
                    Thread.Sleep(10);
                }
                
                Console.WriteLine("Posts iterated: " + c);
            }
            catch(Exception exc)
            {
                Console.WriteLine("Loading subreddit failed: " + exc);
            }
        }

        protected override void Update(GameTime time)
        {
            Input.Update(time);

            if (Input.IsKeyPressed(Keys.Escape))
            {
                if(OpenPost != null)
                {
                    OpenPost = null;
                }
                else
                    Exit();
            }
            
            Viewport view = GraphicsDevice.Viewport;

            UpdateScroll(view, time);

            if (OpenPost == null)
            {
                UpdateObjectVisibility(view, out _firstVisibleGraphic, out _lastVisibleGraphic);

                if (_firstVisibleGraphic != -1 && _lastVisibleGraphic != -1)
                {
                    RenderObjectGraphics(view, _firstVisibleGraphic, _lastVisibleGraphic);
                    ProcessMouse(_firstVisibleGraphic, _lastVisibleGraphic);
                    LoadThumbnails(_firstVisibleGraphic, _lastVisibleGraphic, 6);

                    if (_postsLeftToLoad == 0 && _lastVisibleGraphic + 15 > _postGraphics.Count)
                        _postsLeftToLoad += 5;
                }
            }

            base.Update(time);
        }

        protected override void Draw(GameTime time)
        {
            GraphicsDevice.Clear(_clearColor);
            
            _watch.Restart();
            if (OpenPost != null)
            {
                DrawFullPost(OpenPost);
            }
            else
            {
                DrawGraphicTextures();
                DrawGraphicText();
            }
            _watch.Stop();

            float dx = GraphicsDevice.Viewport.Width + 1;
            if(_resourceManager != null)
                dx = DrawDownloaderDebug(_resourceManager.Downloader);

            if (OpenPost == null)
            {
                _spriteBatch.Begin();
                int gCount = _lastVisibleGraphic - _firstVisibleGraphic;
                string countStr = gCount + "/" + _postGraphics.Count;
                _spriteBatch.DrawString(_font26, countStr, new Vector2(dx - _font26.MeasureString(countStr).Width - 6, 0), Color.LimeGreen);
                //_batch.DrawString(_font26, _firstVisibleGraphic + " - " + _lastVisibleGraphic, new Vector2(3, _font26.LineHeight - 4), Color.Red);
                _spriteBatch.End();
            }

            base.Draw(time);
        }

        private void DrawFullPost(PostGraphicsObject post)
        {
            var builder = StringBuilderPool.Rent(post.Data.Title.Length + 5);
            float maxCharsInLine = GetMaxCharsInView(_font40, GraphicsDevice.Viewport.Width * 1.1f);
            PostGraphicsObject.DivideTextIntoLines(post.Data.Title, builder, (int)maxCharsInLine);
            
            _spriteBatch.Begin(samplerState: SamplerState.LinearClamp);
            _spriteBatch.DrawString(_font40, builder, new Vector2(8, 4), Color.White);

            if (post.PreviewResponse != null && post.PreviewResponse.ContentLength > 0)
            {
                float p = (float)(post.PreviewResponse.BytesDownloaded / (decimal)post.PreviewResponse.ContentLength);
                if (p >= 0 && p <= 1)
                {
                    var view = GraphicsDevice.Viewport;
                    var pos = new Vector2(view.Width / 2f, view.Height / 2f);
                    _postLoadingBar.Draw(_spriteBatch, pos, 60, Color.White, 8, p);
                }
            }
            _spriteBatch.End();

            post.UploadPostTexture();
            var postTex = post.PostTexture;
            if (postTex != null)
            {

                SizeF size = _font40.MeasureString(builder);

                _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                _spriteBatch.Draw(postTex, new Vector2(5, size.Height + 12), Color.White);
                _spriteBatch.End();
            }
            
            StringBuilderPool.Return(builder);
        }

        private void ProcessMouse(int firstVisible, int lastVisible)
        {
            var mousePos = Vector2.Transform(Input.MousePosition.ToVector2(), Matrix.Invert(_graphicsMatrix));
            HoveredPost = null;
            
            for (int i = firstVisible; i < lastVisible + 1; i++)
            {
                var graphic = _postGraphics[i];
                if (graphic.Boundaries.Contains(mousePos))
                {
                    if(Input.IsMousePressed(MouseButton.Left))
                        OpenPost = graphic;
                    else
                        HoveredPost = graphic;
                    break;
                }
            }
        }

        private void UpdateObjectVisibility(Viewport view, out int firstVisible, out int lastVisible)
        {
            Vector2 totalOffset = offset + scrollOffset;
            _graphicsMatrix = Matrix.CreateTranslation(totalOffset.X, totalOffset.Y, 0) * Matrix.CreateScale(1f);

            firstVisible = -1;
            lastVisible = -1;
            for (int i = 0; i < _postGraphics.Count; i++)
            {
                var graphic = _postGraphics[i];
                graphic.CheckVisibility(view, totalOffset);

                if (graphic.IsVisible)
                {
                    if (firstVisible == -1)
                        firstVisible = i;

                    lastVisible = i;
                }
            }
        }

        private void LoadThumbnails(int firstVisible, int lastVisible, int extra)
        {
            int last = Math.Min(_postGraphics.Count, lastVisible + extra);
            for (int i = firstVisible; i < last; i++)
            {
                var graphic = _postGraphics[i];
                if (graphic.HasThumbnail)
                {
                    graphic.RequestThumbnailImage();
                    graphic.UploadThumbnailTexture();
                }
            }
        }

        private void DoObjectLayout()
        {
            float graphicOffsetY = 0;
            for (int i = 0; i < _postGraphics.Count; i++)
            {
                var graphic = _postGraphics[i];
                graphic.DoLayout(graphicOffsetY);
                graphicOffsetY += graphic.Boundaries.Height + 10;
            }
            layoutHeight = graphicOffsetY;
        }

        private void RenderObjectGraphics(Viewport view, int firstVisible, int lastVisible)
        {
            int count = 0;
            for (int i = firstVisible; i < _lastVisibleGraphic + 1; i++)
            {
                var graphic = _postGraphics[i];
                if (!graphic.IsTextDirty)
                    continue;

                Vector2 titleScale = Vector2.One;
                float textSpace = view.Width - graphic.MainTextPosition.X;
                int maxTitleChars = GetMaxCharsInTitle(_font26, (int)textSpace, titleScale);

                graphic.RenderMainText(_font26, Color.White, titleScale, maxTitleChars);
                graphic.RenderStatusText(_font26, Color.White, Vector2.One);

                graphic.IsTextDirty = false;
                count++;
            }

            if (count > 0)
                DoObjectLayout();
        }

        private void UpdateScroll(Viewport view, GameTime time)
        {
            float newScroll = Mouse.GetState().ScrollWheelValue;
            float scroll = newScroll - lastScroll;
            lastScroll = newScroll;
            smoothScroll = MathHelper.Lerp(smoothScroll + scroll, 0, time.Delta * 8f);

            if (OpenPost != null)
                return;

            const float scrollUpThreshold = 16;
            float scrollDownThreshold = -layoutHeight + view.Height - 12;

            if (scrollOffset.Y > scrollUpThreshold)
                scrollOffset.Y = MathHelper.Lerp(scrollOffset.Y, scrollUpThreshold, time.Delta * 30f);
            else if (scrollOffset.Y < scrollDownThreshold)
                scrollOffset.Y = MathHelper.Lerp(scrollOffset.Y, scrollDownThreshold, time.Delta * 30f);

            scrollOffset.Y += MathHelper.Clamp(smoothScroll * 9f * time.Delta, -350, 350);
        }

        private void DrawGraphicTextures()
        {
            if (_firstVisibleGraphic < 0 || _lastVisibleGraphic < 0)
                return;

            _spriteBatch.Begin(SpriteSortMode.BackToFront, transformMatrix: _graphicsMatrix);
            int last = Math.Min(_postGraphics.Count, _lastVisibleGraphic + 1);
            for (int i = _firstVisibleGraphic; i < last; i++)
            {
                var graphic = _postGraphics[i];
                if (!graphic.IsVisible)
                    continue;

                if (graphic.ThumbnailTexture != null)
                {
                    Rectangle dst = graphic.ThumbnailDst.ToRectangle();
                    _spriteBatch.Draw(graphic.ThumbnailTexture, dst, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.1f);
                } 
                _spriteBatch.DrawRectangle(graphic.Boundaries, HoveredPost == graphic ? Color.Green : Color.Red, 1);
            }
            _spriteBatch.End();
        }

        private void DrawGraphicText()
        {
            _spriteBatch.Begin(transformMatrix: _graphicsMatrix);
            for (int i = _firstVisibleGraphic; i <= _lastVisibleGraphic; i++)
            {
                if (i < 0 || i >= _postGraphics.Count)
                    break;

                var graphic = _postGraphics[i];
                if (!graphic.IsVisible)
                    continue;

                _spriteBatch.DrawString(graphic.CachedMainText, graphic.MainTextPosition);
                _spriteBatch.DrawString(graphic.CachedStatusText, graphic.StatusTextPosition);
            }
            _spriteBatch.End();
        }

        private float DrawDownloaderDebug(ResourceRequester requester)
        {
            int threadCount = requester.Workers.Count;
            int size = MathHelper.Clamp((int)Math.Sqrt(threadCount), 1, 3);
            const float tileWidth = 46;
            float width = size * tileWidth;
            float startX = GraphicsDevice.Viewport.Width - width;
            float offX = 0;
            float offY = 5;

            _spriteBatch.Begin();
            for (int i = 0; i < threadCount; i++)
            {
                var request = requester.Workers[i].CurrentRequest;
                
                Vector2 pos = new Vector2(startX + offX, offY);
                offX += tileWidth;
                if(offX >= width)
                {
                    offX = 0;
                    offY += 35;
                }

                bool working = request == null ? false : true;
                Color brickColor = working ? Color.LimeGreen : Color.PaleVioletRed;
                _spriteBatch.DrawFilledRectangle(new RectangleF(pos.X, pos.Y, tileWidth - 5, 29), brickColor);

                double progress = 0;
                if (working)
                {
                    if (request.ContentLength > 0 && request.BytesDownloaded >= 0)
                        progress = (double)request.BytesDownloaded / request.ContentLength;

                    //Console.WriteLine(request.ContentLength + " / " + request.BytesDownloaded);
                }

                string ps = (working ? (int)(progress * 100f) : -1).ToString();
                _spriteBatch.DrawString(_font26, ps, pos + new Vector2(5, 1), Color.DarkBlue);
            }
            _spriteBatch.End();

            return startX + offX;
        }
    }
}