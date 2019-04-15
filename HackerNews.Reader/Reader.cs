using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;

namespace HackerNews.Reader
{
    /// <summary>
    /// Uses the HackerNews API to read top stories/ask/jobs/polls
    /// Performs validation according to spec and outputs results to console
    /// in JSON format. Exceptions are stored for examination at the end of the stream
    /// </summary>
    public class Reader
    {
        public int Posts { get; protected set; }
        public readonly List<string> SupportedSites = new List<string> { "hackernews" };
        public List<Exception> Exceptions { get; protected set; }

        public Reader(string site, string postsInput)
        {          
            bool parse = int.TryParse(postsInput, out int posts);   

            if (string.IsNullOrEmpty(site))
                throw new ArgumentNullException("Site to scrape not specified");

            if (!SupportedSites.Contains(site))
            {
                throw new NotSupportedException($"{site} not currently supported!");
            }

            if (!parse)
                throw new ArgumentException("Invalid number of posts specified - a number is expected");

            if (posts > 100 | posts == 0)
            {
                throw new ArgumentException("Please specify a number of posts between 1-100");
            }

            Posts = posts;
            Exceptions = new List<Exception>();
        }

        public Task Run()
        {
            var topStories = GetPost("https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty");
            List<int> ids = JsonConvert.DeserializeObject<List<int>>(topStories).Take(Posts).ToList();

            int rank = 1;

            foreach (int id in ids)
            {
                try
                {
                    string link = $"https://hacker-news.firebaseio.com/v0/item/{id}.json?print=pretty";

                    var list = GetPost(link);

                    Post article = JsonConvert.DeserializeObject<Post>(list);
                    article.Validate();

                    // the API call retrieves posts according to rank which is not contained in the response
                    // so we increment here.
                    article.Rank = rank++;

                    Console.WriteLine(JsonConvert.SerializeObject(article, Formatting.Indented));
                    Console.WriteLine();
                }
                catch (Exception e)
                {
                    Exceptions.Add(e);
                }
            }

            return Task.CompletedTask;

        }

        public Task ShowExceptions()
        {
            Exceptions.ForEach(ex => Console.WriteLine(ex));

            return Task.CompletedTask;
        }

        private string GetPost(string url)
        {
            using (var httpClient = new HttpClient())
            {
                return httpClient.GetStringAsync(new Uri(url)).Result; // no real benefit from async/await here
            }
        }
    }
}
