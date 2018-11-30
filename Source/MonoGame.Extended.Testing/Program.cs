using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;

namespace MonoGame.Extended.Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var frame = new Frame())
                frame.Run();

            var src = new StringBuilder("hi on you mister 69");
            var dst = new StringBuilder();
            src.CopyTo(0, dst, new char[3], src.Length);

            Console.WriteLine("\"" + src + "\"");
            Console.WriteLine("\"" + dst + "\"");
            Console.ReadKey();
        }
    }

    class Frame : Game
    {
        private GraphicsDeviceManager manager;
        private SpriteBatch batch;
        private BitmapFont font;

        public Frame()
        {
            manager = new GraphicsDeviceManager(this);
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

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {


            base.Draw(gameTime);
        }
    }
}
