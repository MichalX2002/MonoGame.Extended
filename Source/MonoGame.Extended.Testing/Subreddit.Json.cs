using Newtonsoft.Json;
using System.Net;

namespace MonoGame.Extended.Testing
{
    public partial class Subreddit
    {
        [JsonObject]
        private class AboutData
        {
            [JsonProperty("community_icon")]
            public string CommunityIcon { get; }

            [JsonProperty("icon_img")]
            public string IconImg { get; }

            [JsonConstructor]
            internal AboutData(string community_icon, string icon_img)
            {
                CommunityIcon = community_icon;
                IconImg = icon_img;
            }
        }

        [JsonObject]
        private class PostCollectionData
        {
            public Post[] Children { get; }
            public string After { get; }

            [JsonConstructor]
            internal PostCollectionData(Post[] children, string after)
            {
                Children = children;
                After = after;
            }
        }

        [JsonObject]
        public class PostData
        {
            public string ID { get; }
            public string Title { get; }
            public string Author { get; }
            public int Score { get; set; }
            public string Url { get; }
            public int CommentCount { get; }

            public Image Thumbnail { get; }
            public bool HasThumbnail => Thumbnail != null;

            public PreviewCollection Preview { get; }
            public bool HasPreview => Preview != null; // && Preview.Enabled;

            [JsonConstructor]
            internal PostData(
                string id, string title, string author, int score, string url,
                string thumbnail, int num_comments, int? thumbnail_width, int? thumbnail_height,
                PreviewCollection preview)
            {
                ID = id;
                Title = WebUtility.HtmlDecode(title);
                Author = author;
                Score = score;
                Url = WebUtility.HtmlDecode(url);
                CommentCount = num_comments;
                Preview = preview;

                if (thumbnail.StartsWith("http"))
                    Thumbnail = new Image(thumbnail, thumbnail_width ?? -1, thumbnail_height ?? -1);

                if (preview != null)
                    Preview = preview;
            }

            [JsonObject]
            public class PreviewCollection
            {
                public PreviewImage[] Images { get; }
                public bool Enabled { get; }

                [JsonConstructor]
                internal PreviewCollection(PreviewImage[] images, bool enabled)
                {
                    Images = images;
                    Enabled = enabled;
                }
            }

            [JsonObject]
            public class PreviewImage
            {
                public Image Source { get; }
                public Image[] Resolutions { get; }

                [JsonConstructor]
                internal PreviewImage(Image source, Image[] resolutions)
                {
                    Source = source;
                    Resolutions = resolutions;
                }
            }

            [JsonObject]
            public class Image
            {
                public string Url { get; }
                public int Width { get; }
                public int Height { get; }

                [JsonConstructor]
                internal Image(string url, int width, int height)
                {
                    Url = WebUtility.HtmlDecode(url);
                    Width = width;
                    Height = height;
                }
            }
        }

        #region Holder Objects
        [JsonObject]
        private class About
        {
            public AboutData Data { get; }

            [JsonConstructor]
            internal About(AboutData data)
            {
                Data = data;
            }
        }

        [JsonObject]
        private class PostCollection
        {
            public PostCollectionData Data { get; }

            [JsonConstructor]
            internal PostCollection(PostCollectionData data)
            {
                Data = data;
            }
        }

        [JsonObject]
        public class Post
        {
            public PostData Data { get; }
            public int PostNumber { get; set; }

            [JsonConstructor]
            internal Post(PostData data)
            {
                Data = data;
                PostNumber = -1;
            }
        }
        #endregion
    }
}
