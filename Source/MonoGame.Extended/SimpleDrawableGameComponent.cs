using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public abstract class SimpleDrawableGameComponent : SimpleGameComponent, IDrawable
    {
        protected SimpleDrawableGameComponent()
        {
        }

        private bool _isVisible = true;
        public bool Visible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value)
                    return;

                _isVisible = value;
                VisibleChanged?.Invoke(this);
            }
        }

        bool IDrawable.Visible => _isVisible;

        private int _drawOrder;
        public int DrawOrder
        {
            get => _drawOrder;
            set
            {
                if (_drawOrder == value)
                    return;

                _drawOrder = value;
                DrawOrderChanged?.Invoke(this);
            }
        }

        public event SenderDelegate<object> DrawOrderChanged;
        public event SenderDelegate<object> VisibleChanged;

        public abstract void Draw(GameTime gameTime);
    }
}