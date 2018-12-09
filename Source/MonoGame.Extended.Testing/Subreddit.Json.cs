using Newtonsoft.Json;

namespace MonoGame.Extended.Testing
{
    public partial class Subreddit
    {
        [JsonObject]
        private class JsonAboutData
        {
            [JsonProperty("community_icon")]
            public string CommunityIcon { get; }

            [JsonProperty("icon_img")]
            public string IconImg { get; }

            [JsonConstructor]
            public JsonAboutData(string community_icon, string icon_img)
            {
                CommunityIcon = community_icon;
                IconImg = icon_img;
            }
        }

        [JsonObject]
        private class JsonPostCollectionData
        {
            public JsonPost[] Children { get; }
            public string After { get; }

            [JsonConstructor]
            public JsonPostCollectionData(JsonPost[] children, string after)
            {
                Children = children;
                After = after;
            }
        }

        [JsonObject]
        internal class JsonPostData
        {
            public string ID { get; }
            public string Title { get; }
            public string Author { get; }
            public int Score { get; set; }
            public string Url { get; }
            public string Thumbnail { get; }

            [JsonProperty("num_comments")]
            public int NumComments { get; }

            [JsonProperty("thumbnail_width")]
            public int? ThumbnailWidth { get; }

            [JsonProperty("thumbnail_height")]
            public int? ThumbnailHeight { get; }

            [JsonConstructor]
            public JsonPostData(
                string iD, string title, string author, int score, string url,
                string thumbnail, int num_comments, int? thumbnail_width, int? thumbnail_height)
            {
                ID = iD;
                Title = title;
                Author = author;
                Score = score;
                Url = url;
                Thumbnail = thumbnail;
                NumComments = num_comments;
                ThumbnailWidth = thumbnail_width;
                ThumbnailHeight = thumbnail_height;
            }
        }

        #region Holder Objects
        [JsonObject]
        private class JsonAbout
        {
            public JsonAboutData Data { get; }

            public JsonAbout(JsonAboutData data)
            {
                Data = data;
            }
        }

        [JsonObject]
        private class JsonPostCollection
        {
            public JsonPostCollectionData Data { get; }

            [JsonConstructor]
            public JsonPostCollection(JsonPostCollectionData data)
            {
                Data = data;
            }
        }

        [JsonObject]
        internal class JsonPost
        {
            public JsonPostData Data { get; }

            [JsonConstructor]
            public JsonPost(JsonPostData data)
            {
                Data = data;
            }
        }
        #endregion
    }
}
