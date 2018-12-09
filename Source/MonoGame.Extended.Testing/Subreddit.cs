using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Extended.Testing
{
    public partial class Subreddit
    {
        private bool _aboutHasErrored;
        private bool _aboutIsLoaded;

        private StringBuilder _urlBuilder;
        private JsonSerializer _serializer;

        public string Name { get; }
        public string CommunityIcon { get; private set; }
        public string IconImage { get; private set; }

        public RedditService Service { get; }

        public Subreddit(string name, RedditService service)
        {
            Name = name;
            Service = service;

            _urlBuilder = new StringBuilder();
            _serializer = new JsonSerializer();
        }

        public IEnumerable<Post> EnumeratePosts(int limit, int postsPerRequest)
        {
            if (postsPerRequest <= 5)
                postsPerRequest = 5;
            else if (postsPerRequest > 110)
                postsPerRequest = 110;

            string after = null;
            int usedLimit = limit;

            while (usedLimit > 0)
            {
                if (usedLimit <= 0)
                    yield break;

                int queryLimit = Math.Min(postsPerRequest, usedLimit);
                _urlBuilder.Append($"r/{Name}/.json?limit={queryLimit}");

                if (after != null)
                {
                    _urlBuilder.Append("&after=");
                    _urlBuilder.Append(after);
                }

                int count = limit - usedLimit;
                if (count > 0)
                {
                    _urlBuilder.Append("&count=");
                    _urlBuilder.Append(count);
                }

                string url = _urlBuilder.ToString();
                _urlBuilder.Clear();

                var jsonReader = Service.GetJsonReader(url);
                var container = _serializer.Deserialize<JsonPostCollection>(jsonReader);
                after = container.Data.After;

                var children = container.Data.Children;
                if (children.Length == 0)
                    yield break;

                for (int j = 0; j < children.Length; j++)
                {
                    if (usedLimit <= 0)
                        yield break;

                    usedLimit--;
                    yield return new Post(j + count + 1, children[j].Data);
                }
            }
        }

        private void LoadAbout()
        {
            if (_aboutHasErrored || _aboutIsLoaded)
                return;

            try
            {
                var jsonReader = Service.GetJsonReader($"r/{Name}/about/.json");
                var container = _serializer.Deserialize<JsonAbout>(jsonReader);

                if (container == null)
                    _aboutHasErrored = true;
                else
                {
                    CommunityIcon = container.Data.CommunityIcon;
                    IconImage = container.Data.IconImg;
                    _aboutIsLoaded = true;
                }
            }
            catch
            {
                _aboutHasErrored = true;
            }
        }
    }
}