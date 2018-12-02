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
        private GraphicsDeviceManager graphics;
        private SpriteBatch batch;
        private BitmapFont font;
        private Color clearColor = Color.DarkSlateBlue * 0.33f;

        StringBuilder bbb;
        ListArray<GlyphSprite> sprites;
        ListArray<BitmapFont.Glyph> glyphs;
        Stopwatch watch = new Stopwatch();
        Queue<double> lastTimes = new Queue<double>(new double[] { 0 });
        Random rng = new Random();

        public Frame()
        {
            graphics = new GraphicsDeviceManager(this);
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
            batch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<BitmapFont>("Sensation");

            bbb = new StringBuilder();

            Task.Run(StartLoadingSubreddit);

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

            sprites = new ListArray<GlyphSprite>();
            glyphs = new ListArray<BitmapFont.Glyph>();

            glyphs.Clear();
            font.GetGlyphs(bbb, glyphs);

            base.LoadContent();
        }

        private async void StartLoadingSubreddit()
        {
            var reddit = new RedditService();
            var subreddit = await reddit.GetSubredditAsync("Terraria");

            foreach(var post in subreddit.EnumeratePosts(5))
            {
                allPosts.Add(new PostGraphicsObject(post, GraphicsDevice));
            }
        }

        List<PostGraphicsObject> allPosts = new List<PostGraphicsObject>();

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime time)
        {
            GraphicsDevice.Clear(clearColor);

            batch.Begin();
            batch.DrawString(font, ((int)(lastTimes.Average() * 100f) / 100f).ToString(), new Vector2(0, 0), Color.Green);
            batch.End();

            watch.Restart();

            int x = 10;
            int y = 10;
            foreach(var post in allPosts)
            {
                batch.Begin();

                float srcY = y;

                var thumb = post.Thumbnail;
                if (post.HasThumbnail && thumb != null)
                {
                    int w = thumb.Width;
                    int h = thumb.Height;
                    batch.Draw(post.Thumbnail, new Rectangle(x, y, w, h), Color.White);

                    y += h;
                }
                else
                    y += 50;

                float textX = post.HasThumbnail ? 200 : 150;
                batch.DrawString(font, post.Root.Title, new Vector2(textX, srcY + 5), Color.White);

                y += 25;

                batch.End();
            }

            watch.Stop();

            lastTimes.Enqueue(watch.Elapsed.TotalMilliseconds);
            if (lastTimes.Count > 20)
                lastTimes.Dequeue();

            base.Draw(time);
        }
    }

    class PostGraphicsObject
    {
        private bool _thumbnailRequested;
        private Texture2D _thumbnail;

        public Post Root { get; }
        public GraphicsDevice GraphicsDevice { get; }

        public float ThumbnailFade;

        public bool HasThumbnail => !string.IsNullOrWhiteSpace(Root.Thumbnail) && Root.Thumbnail != "self";
        public Texture2D Thumbnail
        {
            get
            {
                if (!HasThumbnail)
                    return null;

                if(_thumbnail == null && !_thumbnailRequested)
                {
                    WebResourceManager.RequestTexture(Root.Thumbnail, GraphicsDevice, (t) => _thumbnail = t);
                    _thumbnailRequested = true;
                }
                return _thumbnail;
            }
        }

        public PostGraphicsObject(Post root, GraphicsDevice device)
        {
            Root = root;
            GraphicsDevice = device;

            ThumbnailFade = 1;
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
