using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MonoGame.Extended.Testing
{
    public class RedditService : IDisposable
    {
        private WebClient _client;

        public RedditService()
        {
            _client = new WebClient();
        }

        public async Task<IEnumerable<string>> GetSubredditsAsync()
        {
            var jObject = await GetObjectAsync(".json?raw_json=1");
            var jArray = jObject["data"]["children"] as JArray;

            IEnumerable<string> ParseSubreddits(JArray array)
            {
                foreach (var obj in array)
                    yield return obj["data"]["subreddit"].ToString();
            }

            var subredditEnumerable = ParseSubreddits(jArray);
            return subredditEnumerable.OrderBy(x => x);
        }

        public async Task<Subreddit> GetSubredditAsync(string name)
        {
            var jObject = await GetObjectAsync("r/" + name + "/.json?raw_json=1");
            var obj = (jObject["data"]["children"] as JArray)[0];
            return new Subreddit(obj["data"]["subreddit"].ToString(), this);
        }

        public async Task<IList<string>> SearchForSubredditsAsync(string query)
        {
            var jObject = await GetObjectAsync("subreddits/search.json?raw_json=1&q=" + query);
            var jArray = jObject["data"]["children"] as JArray;

            var subreddits = new List<string>();
            foreach (var obj in jArray)
                subreddits.Add(obj["data"]["display_name"].ToString());
            return subreddits;
        }

        /*
        public async Task<Post> GetPostAsync(int postNumber, string id)
        {
            var jObject = await GetObjectAsync("api/info.json?raw_json=1&id=t3_" + id);
            var obj = (jObject["data"]["children"] as JArray)[0];
            return new Post(postNumber, obj["data"]);
        }
        */

        public async Task<JObject> GetObjectAsync(string relativeUrl)
        {
            string uri = "https://www.reddit.com/" + relativeUrl;
            using (var stream = await _client.OpenReadTaskAsync(uri))
            using (var streamReader = new StreamReader(stream))
            {
                var jsonReader = new JsonTextReader(streamReader);
                return await JObject.LoadAsync(jsonReader);
            }
        }

        public JObject GetObject(string relativeUrl)
        {
            using (var jsonReader = GetJsonReader(relativeUrl))
                return JObject.Load(jsonReader);
        }

        public JsonTextReader GetJsonReader(string relativeUrl)
        {
            string uri = "https://www.reddit.com/" + relativeUrl;
            var stream = _client.OpenRead(uri);
            var streamReader = new StreamReader(stream);
            return new JsonTextReader(streamReader);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}