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

        public Vector2 MainTextPosition;
        public Vector2 StatusTextPosition;
        public SizeF MainTextSize { get; private set; }
        public SizeF StatusTextSize { get; private set; }
        public ListArray<GlyphSprite> CachedMainText { get; }
        public ListArray<GlyphSprite> CachedStatusText { get; }

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
            
            CachedMainText = new ListArray<GlyphSprite>();
            CachedStatusText = new ListArray<GlyphSprite>();
            ThumbnailFade = 1;

            HasThumbnail =
                Root.HasThumbnail
                && Root.ThumbnailWidth != -1
                && Root.ThumbnailHeight != -1;
        }

        public void DoLayout(float offsetY)
        {
            const float graphicExtraWidth = 8;
            const float textOffsetY = 6;
            const float textOnlyExtraHeight = 40;

            Position = new Vector2(0, offsetY);
            MainTextPosition = new Vector2(220, Position.Y + textOffsetY);
            StatusTextPosition = new Vector2(6, MainTextPosition.Y);

            if (HasThumbnail)
            {
                const float dstOffsetX = 10;
                float dstX = 40;
                if (StatusTextPosition.X + StatusTextSize.Width + dstOffsetX > dstX)
                    dstX = StatusTextSize.Width + dstOffsetX;

                ThumbnailDst = new RectangleF(
                    dstX, offsetY, Root.ThumbnailWidth, Root.ThumbnailHeight);

                Size.Width = MainTextPosition.X + MainTextSize.Width;
                Size.Height = MathHelper.Clamp(ThumbnailDst.Height, textOffsetY * 2 + MainTextSize.Height, 200);
            }
            else
            {
                Size = MainTextSize;
                Size.Width += MainTextPosition.X;
                Size.Height += textOffsetY + textOnlyExtraHeight;
            }
            Size.Width += graphicExtraWidth;
        }

        public void CheckVisibility(Viewport view, Vector2 translation)
        {
            float realY = Position.Y + translation.Y;
            IsVisible = realY > -Size.Height && realY < view.Height;
        }

        public void RenderMainText(BitmapFont font, Color color, Vector2 scale)
        {
            CachedMainText.Clear();
            MainTextSize = font.GetGlyphSprites(
                CachedMainText, Root.Title, Vector2.Zero, color, 0, Vector2.Zero, scale, 0, null);
        }

        public void RenderStatusText(BitmapFont font, Color color, Vector2 scale)
        {
            CachedStatusText.Clear();
            StatusTextSize = font.GetGlyphSprites(
                CachedStatusText, Root.PostNumber.ToString(), Vector2.Zero, color, 0, Vector2.Zero, scale, 0, null);
        }
    }
}
