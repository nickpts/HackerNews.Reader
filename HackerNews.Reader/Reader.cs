using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

using Newtonsoft.Json;

namespace HackerNews.Reader
{
    /// <summary>
    /// Specifies type of posts to retrieve
    /// </summary>
    public enum PostType
    {
        Stories,
        Jobs,
        Ask,
        Show
    }

    /// <summary>
    /// Specifies how deep to go when retrieving comments for a story
    /// </summary>
    public enum CommentLevel
    {
        None, // no replies
        Full // all comments
    }

    /// <summary>
    /// Uses the HackerNews API to read top stories/ask/jobs/polls
    /// Performs validation according to spec and outputs results to console
    /// in JSON format. Exceptions are stored for examination at the end of the stream
    /// </summary>
    public class Reader
    {
        public int Posts { get; protected set; }
        private bool _retrieveComments;
        private CommentLevel _commentRecursionLevel;

        public Dictionary<PostType, string> postTypes = new Dictionary<PostType, string>()
        {
            { PostType.Stories,  "https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty" },
            { PostType.Jobs, "https://hacker-news.firebaseio.com/v0/jobstories.json?print=pretty" },
            { PostType.Ask, "https://hacker-news.firebaseio.com/v0/askstories.json?print=pretty" },
            { PostType.Show, "https://hacker-news.firebaseio.com/v0/showstories.json?print=pretty" }
        };

        public Reader(int posts = 100, bool retrieveComments = false, CommentLevel level = CommentLevel.None)
        {
            if (posts == 0 || posts < 0)
                throw new ArgumentException("Please specify a valid number of posts");

            _retrieveComments = retrieveComments;
            _commentRecursionLevel = level;
            Posts = posts;
        }

        public List<Post> RetrievePosts(PostType postType = PostType.Stories)
        {
            var uri = GetPost(postTypes[postType]);
            var ids = JsonConvert.DeserializeObject<List<int>>(uri).Take(Posts).ToArray();
            
            return RetrievePostsById(ids);
        }

        /// <summary>
        /// Retrieves posts and comments in json format, optionally outputs to console
        /// </summary>
        /// <param name="postType"></param>
        /// <param name="outputToConsole"></param>
        /// <returns></returns>
        public List<string> RetrievePostsInJsonFormat(PostType postType = PostType.Stories, bool outputToConsole = false)
        {
            var posts = RetrievePosts(postType);
            var jsonPosts = new List<string>();

            foreach (var post in posts)
            {
                string json = JsonConvert.SerializeObject(post, Formatting.Indented);
                jsonPosts.Add(json);

                if (outputToConsole)
                    Console.WriteLine(json);
            }

            return jsonPosts;
        }

        public List<Post> RetrievePostsById(int[] ids)
        {
            var posts = new List<Post>();
            int rank = 1;

            foreach (int id in ids)
            {
                string link = $"https://hacker-news.firebaseio.com/v0/item/{id}.json?print=pretty";
                var list = GetPost(link);
                Post article = JsonConvert.DeserializeObject<Post>(list);

                if (_retrieveComments)
                {
                    article = RetrieveComments(article);
                }

                article.Validate();

                // the API call retrieves posts according to rank which is not contained in the response
                // so we increment here.
                article.Rank = rank++;
                posts.Add(article);
            }

            return posts;
        }

        /// <summary>
        /// Retrieves only "Who is hiring" posts
        /// </summary>
        /// <returns></returns>
        public List<Post> RetrieveHiringPosts()
        {
            var uri = GetPost(postTypes[PostType.Stories]);
            var ids = JsonConvert.DeserializeObject<List<int>>(uri).Take(Posts).ToArray();
            var posts = new List<Post>();
            int rank = 1;

            foreach (int id in ids)
            {
                string link = $"https://hacker-news.firebaseio.com/v0/item/{id}.json?print=pretty";
                var list = GetPost(link);
                Post article = JsonConvert.DeserializeObject<Post>(list);

                if (!article.IsHiring) continue;

                if (_retrieveComments)
                {
                    article = RetrieveComments(article);
                }

                article.Validate();

                // the API call retrieves posts according to rank which is not contained in the response
                // so we increment here.
                article.Rank = rank++;
                posts.Add(article);
            }

            return posts;
        }

        #region Private implementation

        /// <summary>
        /// If all comments are specified, calls itself recursively to find descendants.
        /// </summary>
        /// <returns></returns>
        private Post RetrieveComments(Post comment)
        {
            if (!comment.HasKids) return comment;

            foreach (var commentId in comment.Kids)
            {
                string commentLink = $"https://hacker-news.firebaseio.com/v0/item/{commentId}.json?print=pretty";
                var jsonComment = GetPost(commentLink);
                Post descendantComment = JsonConvert.DeserializeObject<Post>(jsonComment);
                comment.Comments.Add(comment);

                if (_commentRecursionLevel == CommentLevel.Full)
                {
                    RetrieveComments(descendantComment);
                }
            }

            return comment;
        }

        private string GetPost(string url)
        {
            using (var httpClient = new HttpClient())
            {
                return httpClient.GetStringAsync(new Uri(url)).Result; // no real benefit from async/await here
            }
        }

        #endregion
    }
}
