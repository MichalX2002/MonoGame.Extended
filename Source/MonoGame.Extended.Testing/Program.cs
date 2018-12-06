using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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

        //private StringBuilder _bbb;
        //private ListArray<GlyphSprite> _sprites;
        //private ListArray<BitmapFont.Glyph> _glyphs

        private Stopwatch _watch = new Stopwatch();
        private Queue<double> _lastTimes = new Queue<double>(new double[] { 0 });
        private Random _rng = new Random();
        
        private ResourceDownloader _downloader;
        private Matrix graphicsMatrix;
        private List<PostGraphicsObject> postGraphics = new List<PostGraphicsObject>();

        private Vector2 offset = new Vector2(10, 10);
        private Vector2 scrollOffset;
        private float lastScroll;
        private float smoothScroll;

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

            _downloader = new ResourceDownloader();
            _downloader.Start();

            Task.Run(StartLoadingSubreddit);

            //_bbb = new StringBuilder();

            /*
            bbb.Append(' ', 30);
            for (int i = 0; i < 18; i++)
            {
                if(i == 0)
                    bbb.Append('h', 187);
                else
                    bbb.Append('h', 200);
            
                bbb.Append('\n');
                bbb.Append('e', 200);
                bbb.Append('\n');
                bbb.Append('j', 450);
                bbb.Append('\n');
            }
            */

            //_sprites = new ListArray<GlyphSprite>();
            //_glyphs = new ListArray<BitmapFont.Glyph>();
            //
            //_glyphs.Clear();
            //_font.GetGlyphs(_bbb, _glyphs);

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            _downloader.Unload();

            base.UnloadContent();
        }

        private async void StartLoadingSubreddit()
        {
            var reddit = new RedditService();
            var subreddit = await reddit.GetSubredditAsync("Terraria");

            foreach(var post in subreddit.EnumeratePosts(100))
            {
                var obj = new PostGraphicsObject(post, GraphicsDevice, _downloader);
                obj.PreRenderText(_font, Color.White, Vector2.One);
                postGraphics.Add(obj);
            }
            Console.WriteLine(postGraphics.Count);
        }

        protected override void Update(GameTime time)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Vector2 totalOffset = offset + scrollOffset;
            graphicsMatrix = Matrix.CreateTranslation(totalOffset.X, totalOffset.Y, 0);

            Viewport view = GraphicsDevice.Viewport;
            float graphicOffsetY = 0;
            for (int i = 0; i < postGraphics.Count; i++)
            {
                var graphic = postGraphics[i];
                graphic.DoLayout(view, graphicOffsetY, totalOffset);
                graphicOffsetY += graphic.Size.Height + 10;
            }

            float newScroll = Mouse.GetState().ScrollWheelValue;
            float scroll = newScroll - lastScroll;
            lastScroll = newScroll;
            smoothScroll = MathHelper.Lerp(smoothScroll + scroll, 0, time.Delta * 8f);
            
            const float scrollUpThreshold = 12;
            if (scrollOffset.Y > scrollUpThreshold)
                scrollOffset.Y = MathHelper.Lerp(scrollOffset.Y, scrollUpThreshold, time.Delta * 12.5f);
            scrollOffset.Y += MathHelper.Clamp(smoothScroll * 1.2f * time.Delta, -400, 400);
            
            base.Update(time);
        }
        
        protected override void Draw(GameTime time)
        {
            GraphicsDevice.Clear(_clearColor);

            _watch.Restart();
            _batch.Begin(transformMatrix: graphicsMatrix);

            int lastC = 0;
            for (int i = 0; i < postGraphics.Count; i++)
            {
                var graphic = postGraphics[i];
                if (!graphic.IsVisible)
                    continue;
                lastC++;
                 
                if (graphic.HasThumbnail && graphic.Thumbnail != null)
                    _batch.Draw(graphic.Thumbnail, graphic.ThumbnailDst.ToRectangle(), Color.White);
                _batch.DrawString(graphic.CachedText, graphic.TextPosition);

                _batch.DrawRectangle(new RectangleF(graphic.Position, graphic.Size), Color.Red, 1);
            }
        
            _batch.End();
            _watch.Stop();

            _lastTimes.Enqueue(_watch.Elapsed.TotalMilliseconds);
            if (_lastTimes.Count > 20)
                _lastTimes.Dequeue();

            _batch.Begin();
            _batch.DrawString(_font, /*((int)(_lastTimes.Average() * 100f) / 100f).ToString() + " | " + */ lastC + "/" + postGraphics.Count, new Vector2(1, 0), Color.Green);
            _batch.End();

            base.Draw(time);
        }
    }
   
    // #OLD DRAWING CODE
    //watch.Restart();

    //sprites.Clear();
    //font.GetGlyphSprites(sprites, bbb, Vector2.Zero, Color.White, 0, Vector2.Zero, Vector2.One, 0, null);

    //watch.Stop();

    //double tot = time.TotalGameTime.TotalSeconds * 3f;
    //for (int i = 0; i < sprites.Count; i++)
    //{
    //    ref GlyphSprite gs = ref sprites.GetReferenceAt(i);
    //    gs.Position.X += (float)(Math.Sin(tot + i / 2f) * 3);
    //    gs.Position.Y += (float)(Math.Cos(tot + i / 2f) * 3);
    //}
    
    //batch.Begin();
    //batch.DrawString(sprites);
    //batch.End();

    class Program
    {
        static void Main(string[] args)
        {
            using (var frame = new Frame())
                frame.Run();

            //var src = new StringBuilder("hi on you mister 69");
            //var dst = new StringBuilder();
            //src.CopyTo(0, dst, new char[3], src.Length);

            //Console.WriteLine("\"" + src + "\"");
            //Console.WriteLine("\"" + dst + "\"");
            //Console.ReadKey();
        }
    }
}
