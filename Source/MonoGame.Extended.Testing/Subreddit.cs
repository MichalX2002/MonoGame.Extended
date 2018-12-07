using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
            var urlBuilder = new StringBuilder();
            string after = null;
            int usedLimit = limit;

            while (usedLimit > 0)
            {
                if (usedLimit <= 0)
                    yield break;

                urlBuilder.Append($"r/{Name}/.json?limit={usedLimit}");

                if (after != null)
                {
                    urlBuilder.Append("&after=");
                    urlBuilder.Append(after);
                }

                int count = limit - usedLimit;
                if (count > 0)
                {
                    urlBuilder.Append("&count=");
                    urlBuilder.Append(count);
                }

                var jObject = Service.GetObject(urlBuilder.ToString());
                urlBuilder.Clear();

                var data = jObject["data"];
                after = data["after"].ToString();

                var jArray = data["children"] as JArray;
                if (jArray.Count == 0)
                    yield break;

                for (int j = 0; j < jArray.Count; j++)
                {
                    if (usedLimit <= 0)
                        yield break;

                    usedLimit--;
                    yield return new Post(j + count + 1, jArray[j]["data"]);
                }
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