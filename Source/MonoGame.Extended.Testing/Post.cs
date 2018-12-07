using Newtonsoft.Json.Linq;

namespace MonoGame.Extended.Testing
{
    public class Post
    {
        public int PostNumber { get; }
        public string ID { get; }
        public string Title { get; }
        public string Author { get; }
        public int Score { get; set; }
        public string Url { get; }
        public int NumberOfComments { get; }

        public string Thumbnail { get; }
        public bool HasThumbnail { get; }
        public int ThumbnailWidth { get; }
        public int ThumbnailHeight { get; }

        public Post(int postNumber, JToken data)
        {
            PostNumber = postNumber;
            ID = data["id"].ToString();
            Title = data["title"].ToString();
            Author = data["author"].ToString();
            Score = data["score"].Value<int>();
            Url = data["url"].ToString();
            NumberOfComments = data["num_comments"].Value<int>();

            Thumbnail = data["thumbnail"].ToString();
            HasThumbnail = Thumbnail.StartsWith("http");
            ThumbnailWidth = GetInt(data["thumbnail_width"]);
            ThumbnailHeight = GetInt(data["thumbnail_height"]);
        }

        private int GetInt(JToken value)
        {
            return value.Type == JTokenType.Integer ? value.Value<int>() : -1;
        } 
    }
}