using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.ViewportAdapters
{
    public class WindowViewportAdapter : ViewportAdapter
    {
        protected readonly GameWindow Window;

        public override int ViewportWidth => Window.ClientBounds.Width;
        public override int ViewportHeight => Window.ClientBounds.Height;
        public override int VirtualWidth => Window.ClientBounds.Width;
        public override int VirtualHeight => Window.ClientBounds.Height;

        public WindowViewportAdapter(GameWindow window, GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            Window = window;
            window.ClientSizeChanged += OnClientSizeChanged;
        }

        public override Matrix GetScaleMatrix()
        {
            return Matrix.Identity;
        }

        private void OnClientSizeChanged(GameWindow window)
        {
            int x = window.ClientBounds.Width;
            int y = window.ClientBounds.Height;
            GraphicsDevice.Viewport = new Viewport(0, 0, x, y);
        }
    }
}