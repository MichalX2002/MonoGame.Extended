using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame.Extended.Testing
{
    public class PostGraphicsObject
    {
        private bool _thumbnailRequested;
        private Texture2D _thumbnail;

        public Post Root { get; }
        public GraphicsDevice GraphicsDevice { get; }
        public ResourceDownloader Downloader { get; }

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
                    _thumbnailRequested = true;
                    Downloader.Request(Root.Thumbnail, (url, response) =>
                    {
                        switch (response.ContentType.ToLower())
                        {
                            case "image/jpg":
                            case "image/jpeg":
                            case "image/png":
                            case "image/bmp":
                            case "image/gif":
                                using (var stream = response.GetResponseStream())
                                    _thumbnail = Texture2D.FromStream(GraphicsDevice, stream);
                                IsThumbnailLoaded = true;
                                break;
                        }
                    });
                }
                return _thumbnail;
            }
        }

        public PostGraphicsObject(Post root, GraphicsDevice device, ResourceDownloader downloader)
        {
            Root = root;
            GraphicsDevice = device;
            Downloader = downloader;
            HasThumbnail = Root.Thumbnail.StartsWith("http");

            CachedText = new ListArray<GlyphSprite>(root.Title.Length);
            ThumbnailFade = 1;
        }

        public void DoLayout(Viewport view, float offsetY, Vector2 translation)
        {
            const float graphicExtraWidth = 8;
            const float textOffsetY = 6;
            const float textOnlyExtraHeight = 40;
            
            TextPosition = new Vector2(HasThumbnail ? 170 : 70, offsetY + textOffsetY);
            Position = new Vector2(0, offsetY);

            if (IsThumbnailLoaded)
            {
                ThumbnailDst = new RectangleF(
                    0, offsetY, Thumbnail.Width, Thumbnail.Height);

                Size.Width = TextPosition.X + TextSize.Width;
                Size.Height = MathHelper.Clamp(ThumbnailDst.Size.Height, textOffsetY * 2 + TextSize.Height, 200);
            }
            else
            {
                Size = TextSize;
                Size.Width += TextPosition.X;
                Size.Height += textOffsetY + textOnlyExtraHeight;
            }
            Size.Width += graphicExtraWidth;

            float realY = Position.Y + translation.Y;
            IsVisible = realY > -Size.Height && realY < view.Height;
        }

        public void PreRenderText(BitmapFont font, Color color, Vector2 scale)
        {
            CachedText.Clear();
            TextSize = font.GetGlyphSprites(
                CachedText, Root.Title, Vector2.Zero, color, 0, Vector2.Zero, scale, 0, null);
        }
    }
}
