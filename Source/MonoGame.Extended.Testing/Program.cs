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
            WebResourceManager.Unload();

            base.UnloadContent();
        }

        private async void StartLoadingSubreddit()
        {
            var reddit = new RedditService();
            var subreddit = await reddit.GetSubredditAsync("Terraria");

            foreach(var post in subreddit.EnumeratePosts(3))
            {
                var obj = new PostGraphicsObject(post, GraphicsDevice);
                obj.PreRenderText(_font, Color.White, Vector2.One);
                postGraphics.Add(obj);
            }
            Console.WriteLine(postGraphics.Count);
        }

        Matrix graphicsMatrix;
        List<PostGraphicsObject> postGraphics = new List<PostGraphicsObject>();

        protected override void Update(GameTime gameTime)
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

            offset = new Vector2(10, 10);
            if(postGraphics.Count > 0)
                scrollOffset.Y -= 1;

            base.Update(gameTime);
        }

        private Vector2 offset;
        private Vector2 scrollOffset;

        protected override void Draw(GameTime time)
        {
            GraphicsDevice.Clear(_clearColor);

            _watch.Restart();
            _batch.Begin();

            for (int i = 0; i < postGraphics.Count; i++)
            {
                var graphic = postGraphics[i];
                if (!graphic.IsVisible)
                    continue;
                 
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
            _batch.DrawString(_font, ((int)(_lastTimes.Average() * 100f) / 100f).ToString(), new Vector2(1, 0), Color.Green);
            _batch.End();

            base.Draw(time);
        }
    }

    class PostGraphicsObject
    {
        private bool _thumbnailRequested;
        private Texture2D _thumbnail;
        
        public Post Root { get; }
        public GraphicsDevice GraphicsDevice { get; }
        
        public bool IsVisible;
        public Vector2 Position;
        public SizeF Size;

        public Vector2 TextPosition;
        public SizeF TextSize { get; private set; }
        public ListArray<GlyphSprite> CachedText { get; }

        public RectangleF ThumbnailDst;
        public float ThumbnailFade;
        public bool HasThumbnail { get; }
        public bool IsThumbnailLoaded { get; private set; }
        public Texture2D Thumbnail
        {
            get
            {
                if (!HasThumbnail)
                    return null;

                if (_thumbnail == null && !_thumbnailRequested)
                {
                    WebResourceManager.RequestTexture(Root.Thumbnail, GraphicsDevice, (t) =>
                    {
                        _thumbnail = t;
                        IsThumbnailLoaded = true;
                    });
                    _thumbnailRequested = true;
                }
                return _thumbnail;
            }
        }

        public PostGraphicsObject(Post root, GraphicsDevice device)
        {
            Root = root;
            GraphicsDevice = device;
            HasThumbnail = Root.Thumbnail.StartsWith("http");

            CachedText = new ListArray<GlyphSprite>(root.Title.Length);
            ThumbnailFade = 1;
        }

        public void DoLayout(Viewport view, float offsetY, Vector2 translation)
        {
            const float graphicExtraWidth = 8;
            const float textOffsetY = 6;
            const float textOnlyExtraHeight = 50;

            float realY = translation.Y + offsetY;
            IsVisible = realY > 0 || realY < view.Height;

            TextPosition = new Vector2(HasThumbnail ? 200 : 100, offsetY + textOffsetY);
            Position = new Vector2(0, offsetY);

            if (IsThumbnailLoaded)
            {
                ThumbnailDst = new RectangleF(
                    translation.X, offsetY, Thumbnail.Width, Thumbnail.Height);
                
                Size.Width = TextPosition.X + TextSize.Width;
                Size.Height = ThumbnailDst.Size.Height;
            }
            else
            {
                Size = TextSize;
                Size.Width += TextPosition.X;
                Size.Height += textOffsetY + textOnlyExtraHeight;
            }
            Size.Width += graphicExtraWidth;
        }
    
        public void PreRenderText(BitmapFont font, Color color, Vector2 scale)
        {
            CachedText.Clear();
            TextSize = font.GetGlyphSprites(
                CachedText, Root.Title, Vector2.Zero, color, 0, Vector2.Zero, scale, 0, null);
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
