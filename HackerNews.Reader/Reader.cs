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
        public List<Exception> Exceptions { get; protected set; }
        public const string hackerNewsTopStoriesApiCall = "https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty";


        public Reader(int posts)
        {
            if (posts > 100 | posts == 0)
            {
                throw new ArgumentException("Please specify a number of posts between 1-100");
            }

            Posts = posts;
            Exceptions = new List<Exception>();
        }

        public List<Post> Run(bool retrieveComments = false)
        {
            List<Post> posts = new List<Post>();
            var topStories = GetPost(hackerNewsTopStoriesApiCall);
            List<int> ids = JsonConvert.DeserializeObject<List<int>>(topStories).Take(Posts).ToList();

            int rank = 1;

            foreach (int id in ids)
            {
                try
                {
                    string link = $"https://hacker-news.firebaseio.com/v0/item/{id}.json?print=pretty";
                    var list = GetPost(link);
                    Post article = JsonConvert.DeserializeObject<Post>(list);

                    if (retrieveComments)
                    {
                        foreach (var commentId in article.Kids)
                        {
                            string commentLink = $"https://hacker-news.firebaseio.com/v0/item/{commentId}.json?print=pretty";
                            var jsonComment = GetPost(commentLink);
                            Post comment = JsonConvert.DeserializeObject<Post>(jsonComment);
                            article.Comments.Add(comment);
                        }
                    }

                    article.Validate();

                    // the API call retrieves posts according to rank which is not contained in the response
                    // so we increment here.
                    article.Rank = rank++;

                    posts.Add(article);
                }
                catch (Exception e)
                {
                    Exceptions.Add(e);
                }
            }

            return posts;
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
