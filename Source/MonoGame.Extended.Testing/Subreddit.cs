using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MonoGame.Extended.Testing
{
    public class Subreddit
    {
        private bool _aboutHasErrored = false;
        private JObject _about;

        public string Name { get; }
        public string CommunityIconUrl => GetAboutValue("community_icon").ToString();
        public string IconImageUrl => GetAboutValue("icon_img").ToString();

        public RedditService Service { get; }

        public Subreddit(string name, RedditService service)
        {
            Name = name;
            Service = service;
        }

        public IEnumerable<Post> EnumeratePosts(int limit)
        {
            var jObject = Service.GetObject($"r/{Name}/.json?limit={limit}");
            var jArray = jObject["data"]["children"] as JArray;
            
            foreach (var obj in jArray)
            {
                var data = obj["data"];
                yield return new Post(
                    data["id"].ToString(),
                    data["title"].ToString(),
                    data["author"].ToString(),
                    data["score"].Value<int>(),
                    data["thumbnail"].ToString(),
                    data["url"].ToString(),
                    data["num_comments"].Value<int>());
            }
        }

        private object GetAboutValue(string key)
        {
            if (_about == null)
                LoadAbout();

            if (_about == null)
                return null;

            return _about[key];
        }

        private void LoadAbout()
        {
            if (_aboutHasErrored || _about != null)
                return;

            try
            {
                var obj = Service.GetObject($"r/{Name}/about/.json");

                if (obj == null)
                    _aboutHasErrored = true;
                else
                    _about = obj["data"] as JObject;

                if (_about == null)
                    _aboutHasErrored = true;
            }
            catch
            {
                _aboutHasErrored = true;
            }
        }
    }
}