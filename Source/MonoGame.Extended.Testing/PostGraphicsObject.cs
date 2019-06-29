using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Color = Microsoft.Xna.Framework.Color;

namespace MonoGame.Extended.Testing
{
    public class PostGraphicsObject
    {
        public const string HttpAccept_Image = "image/png, image/jpeg, image/jpg, image/bmp, image/gif";

        private Image<Rgba32> _thumbnailImage;
        private Texture2D _thumbnailTexture;
        
        private Image<Rgba32> _postImage;
        private Texture2D _postTexture;

        public Subreddit.Post Post { get; }
        public Subreddit.PostData Data => Post.Data;
        public GraphicsDevice GraphicsDevice { get; }
        public ResourceRequester Requester { get; }

        public bool IsVisible;
        public bool IsTextDirty;
        public RectangleF Boundaries;

        public Vector2 MainTextPosition;
        public Vector2 StatusTextPosition;
        public SizeF MainTextSize { get; private set; }
        public SizeF StatusTextSize { get; private set; }
        public ListArray<GlyphSprite> CachedMainText { get; }
        public ListArray<GlyphSprite> CachedStatusText { get; }

        public RectangleF ThumbnailDst;
        public float ThumbnailFade;
        public bool HasThumbnail { get; }
        public bool IsThumbnailRequested { get; private set; }
        public bool IsThumbnailDownloaded { get; private set; }
        public bool IsThumbnailLoaded { get; private set; }
        public bool IsThumbnailFaulted { get; private set; }

        public bool HasPreview => Data.HasPreview;
        public bool IsPostImageDownloaded { get; private set; }
        public bool IsPostImageRequested { get; private set; }
        public bool IsPostImageLoaded { get; private set; }
        public bool IsPostImageFaulted { get; private set; }
        public ResourceResponse PreviewResponse { get; private set; }

        public Texture2D ThumbnailTexture
        {
            get
            {
                RequestThumbnailImage();
                return _thumbnailTexture;
            }
        }

        public Texture2D PostTexture
        {
            get
            {
                RequestPostImage();
                return _postTexture;
            }
        }

        public PostGraphicsObject(Subreddit.Post post, GraphicsDevice device, ResourceRequester requester)
        {
            Post = post;
            GraphicsDevice = device;
            Requester = requester;

            CachedMainText = new ListArray<GlyphSprite>();
            CachedStatusText = new ListArray<GlyphSprite>();
            IsTextDirty = true;
            ThumbnailFade = 1;

            HasThumbnail =
                Data.HasThumbnail &&
                Data.Thumbnail.Width != -1 && 
                Data.Thumbnail.Height != -1;
        }

        public void RequestThumbnailImage()
        {
            if (!HasThumbnail)
                return;

            if (_thumbnailTexture == null &&
                !IsThumbnailRequested && 
                !IsThumbnailFaulted &&
                !IsThumbnailDownloaded &&
                !IsThumbnailLoaded)
            {
                IsThumbnailRequested = true;
                Requester.Request(new Uri(Data.Thumbnail.Url), HttpAccept_Image, false, OnThumbnailResponse, null);
            }
        }

        public void RequestPostImage()
        {
            if (!HasPreview)
                return;

            if (_postTexture == null &&
                !IsPostImageRequested &&
                !IsPostImageFaulted &&
                !IsPostImageDownloaded &&
                !IsPostImageLoaded)
            {
                IsPostImageRequested = true;
                PreviewResponse = Requester.Request(
                    new Uri(Data.Preview.Images[0].Source.Url), HttpAccept_Image, true, OnPostImageResponse, 
                    (u, x) => Console.WriteLine(u + ": " + x));
            }
        }

        private SurfaceFormat GetSurfaceFormat(int channels)
        {
            if (channels == 3)
                return SurfaceFormat.Rgb24;
            else if(channels == 4)
                return SurfaceFormat.Rgba32;
            throw new ArgumentOutOfRangeException(nameof(channels), $"{channels} are unsupported.");
        }

        public void UploadThumbnailTexture()
        {
            if (IsThumbnailDownloaded && !IsThumbnailFaulted &&
                _thumbnailTexture == null && _thumbnailImage != null)
            {
                int channels = _thumbnailImage.PixelType.BitsPerPixel / 8;
                var format = GetSurfaceFormat(channels);
                _thumbnailTexture = new Texture2D(GraphicsDevice, _thumbnailImage.Width, _thumbnailImage.Height, false, format);
                _thumbnailTexture.SetData(_thumbnailImage.GetPixelSpan());
                IsThumbnailLoaded = true;

                _thumbnailImage.Dispose();
                _thumbnailImage = null;
            }
        }

        public void UploadPostTexture()
        {
            if (IsPostImageDownloaded && !IsPostImageFaulted &&
                _postTexture == null && _postImage != null)
            {
                int channels = _postImage.PixelType.BitsPerPixel / 8;
                var format = GetSurfaceFormat(channels);
                _postTexture = new Texture2D(GraphicsDevice, _postImage.Width, _postImage.Height, false, format);
                _postTexture.SetData(_postImage.GetPixelSpan());
                IsPostImageLoaded = true;

                _postImage.Dispose();
                _postImage = null;
            }
        }

        private void OnThumbnailResponse(Uri uri, ResourceStream stream)
        {
            if (!ValidateContentTypeForImage(stream))
                return;
            try
            {
                _thumbnailImage = Image.Load<Rgba32>(stream);
            }
            catch (Exception exc)
            {
                IsThumbnailFaulted = true;
                Console.WriteLine(exc);
            }
            IsThumbnailDownloaded = true;
        }

        private void OnPostImageResponse(Uri uri, ResourceStream stream)
        {
            if (!ValidateContentTypeForImage(stream))
                return;
            try
            {
                _postImage = Image.Load<Rgba32>(stream);
            }
            catch(Exception exc)
            {
                IsPostImageFaulted = true;
                Console.WriteLine(exc);
            }
            IsPostImageDownloaded = true;
        }

        private bool ValidateContentTypeForImage(ResourceStream stream)
        {
            switch (stream.ContentType.ToLowerInvariant())
            {
                case "image/jpg":
                case "image/jpeg":
                case "image/png":
                case "image/bmp":
                case "image/gif":
                    return true;

                default:
                    return false;
            }
        }

        public void DoLayout(float offsetY)
        {
            const float graphicExtraWidth = 8;
            const float textOffsetY = 6;
            const float textOnlyExtraHeight = 40;

            Boundaries.Position = new Vector2(0, offsetY);
            MainTextPosition = new Vector2(220, Boundaries.Y + textOffsetY);
            StatusTextPosition = new Vector2(6, MainTextPosition.Y);

            if (HasThumbnail)
            {
                const float dstOffsetX = 10;
                float dstX = 40;
                if (StatusTextPosition.X + StatusTextSize.Width + dstOffsetX > dstX)
                    dstX = StatusTextSize.Width + dstOffsetX;

                ThumbnailDst = new RectangleF(
                    dstX, offsetY, Data.Thumbnail.Width, Data.Thumbnail.Height);

                Boundaries.Width = MainTextPosition.X + MainTextSize.Width;
                Boundaries.Height = MathHelper.Clamp(ThumbnailDst.Height, textOffsetY * 2 + MainTextSize.Height, 200);
            }
            else
            {
                Boundaries.Size = MainTextSize;
                Boundaries.Width += MainTextPosition.X;
                Boundaries.Height += textOffsetY + textOnlyExtraHeight;
            }
            Boundaries.Width += graphicExtraWidth;
        }

        public void CheckVisibility(Viewport view, Vector2 translation)
        {
            float realY = Boundaries.Y + translation.Y;
            IsVisible = realY > -Boundaries.Height && realY < view.Height;
        }

        static readonly HashSet<char> _spacingChars = new HashSet<char>(5)
        {
            ' ', ',', '.', '!', '?', ':'
        };

        public static void DivideTextIntoLines(string text, StringBuilder output, int maxCharsInLine)
        {
            for (int i = 0; i < text.Length; i++)
            {
                output.Append(text[i]);

                if ((i + 1) % maxCharsInLine == 0)
                {
                    int ci = i;
                    while (ci > 2 && !_spacingChars.Contains(output[ci]))
                        ci--;

                    output.Insert(ci + 1, '\n');
                }
            }
        }

        public void RenderMainText(BitmapFont font, Color color, Vector2 scale, int maxWidthInChars)
        {
            var builder = StringBuilderPool.Rent(Data.Title.Length + 5);
            DivideTextIntoLines(Data.Title, builder, maxWidthInChars);

            CachedMainText.Clear();
            MainTextSize = font.GetGlyphSprites(
                CachedMainText, builder, Vector2.Zero, color, 0, Vector2.Zero, scale, 0, null);

            StringBuilderPool.Return(builder);
        }

        public void RenderStatusText(BitmapFont font, Color color, Vector2 scale)
        {
            CachedStatusText.Clear();
            StatusTextSize = font.GetGlyphSprites(
                CachedStatusText, Post.PostNumber.ToString(), Vector2.Zero, color, 0, Vector2.Zero, scale, 0, null);
        }
    }
}
