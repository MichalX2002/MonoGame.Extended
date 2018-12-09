
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

        internal Post(int postNumber, Subreddit.JsonPostData data)
        {
            PostNumber = postNumber;

            ID = data.ID;
            Title = data.Title;
            Author = data.Author;
            Score = data.Score;
            Url = data.Url;
            NumberOfComments = data.NumComments;

            Thumbnail = data.Thumbnail;
            HasThumbnail = Thumbnail.StartsWith("http");
            ThumbnailWidth = data.ThumbnailWidth ?? -1;
            ThumbnailHeight = data.ThumbnailHeight ?? -1;
        }
    }
}