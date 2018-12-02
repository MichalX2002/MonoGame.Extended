

namespace MonoGame.Extended.Testing
{
    public class Post
    {
        public string ID { get; }
        public string Title { get; }
        public string Author { get; }
        public int Score { get; set; }
        public string Thumbnail { get; }
        public string Url { get; }
        public int NumberOfComments { get; }

        public Post(string id, string title, string author, int score, string thumbnail, string url, int numComments)
        {
            ID = id;
            Title = title;
            Author = author;
            Score = score;
            Thumbnail = thumbnail;
            Url = url;
            NumberOfComments = numComments;
        }
    }
}