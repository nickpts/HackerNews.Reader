using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
    /// in JSON format.
    /// </summary>
    public class Reader
    {
        private bool _retrieveComments;
        private CommentLevel _commentRecursionLevel;
        private int _numberOfPosts = 0;

        public Dictionary<PostType, string> postTypes = new Dictionary<PostType, string>()
        {
            { PostType.Stories,  "https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty" },
            { PostType.Jobs, "https://hacker-news.firebaseio.com/v0/jobstories.json?print=pretty" },
            { PostType.Ask, "https://hacker-news.firebaseio.com/v0/askstories.json?print=pretty" },
            { PostType.Show, "https://hacker-news.firebaseio.com/v0/showstories.json?print=pretty" }
        };

        public Reader(int numberOfPosts = 100, bool retrieveComments = false, CommentLevel level = CommentLevel.None)
        {
            if (numberOfPosts == 0 || numberOfPosts < 0)
                throw new ArgumentException("Please specify a valid number of posts");

            _retrieveComments = retrieveComments;
            _commentRecursionLevel = level;
            _numberOfPosts = numberOfPosts;
        }

		public async Task<Post> GetPostById(int id)
		{
			string link = $"https://hacker-news.firebaseio.com/v0/item/{id}.json?print=pretty";
			var list = await InvokeHackerNewsApi(link);
			Post article = JsonConvert.DeserializeObject<Post>(list);

			if (_retrieveComments)
			{
				article = GetComments(article);
			}

			article.Validate();

			return article;
		}

		public IEnumerable<Post> GetPosts(PostType postType = PostType.Stories)
        {
            var uri = InvokeHackerNewsApi(postTypes[postType]);
			var ids = JsonConvert.DeserializeObject<List<int>>(uri.Result).Take(_numberOfPosts).ToList();

			foreach (int i in ids)
			{
				yield return GetPostById(i).Result;
			}
        }

		public IEnumerable<Post> GetPostsById(int[] ids)
		{
			foreach (int id in ids)
			{
				yield return GetPostById(id).Result;
			}
		}

		/// <summary>
		/// Retrieves posts and comments in json format, optionally outputs to console
		/// </summary>
		/// <param name="postType"></param>
		/// <param name="outputToConsole"></param>
		/// <returns></returns>
		public IEnumerable<string> GetPostsInJsonFormat(PostType postType = PostType.Stories, bool outputToConsole = false)
        {
            var posts = GetPosts(postType);

			foreach (var post in posts)
            {
                string json = JsonConvert.SerializeObject(post, Formatting.Indented);

                if (outputToConsole)
                    Console.WriteLine(json);

				yield return json;
            }
        }

        /// <summary>
        /// Retrieves only "Who is hiring" posts
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Post> GetHiringPosts()
        {
            var uri = InvokeHackerNewsApi(postTypes[PostType.Stories]);
            var ids = JsonConvert.DeserializeObject<List<int>>(uri.Result).Take(_numberOfPosts).ToArray();

            foreach (int id in ids)
            {
                string link = $"https://hacker-news.firebaseio.com/v0/item/{id}.json?print=pretty";
                var list = InvokeHackerNewsApi(link);
                Post article = JsonConvert.DeserializeObject<Post>(list.Result);

                if (!article.IsHiring) continue;

                if (_retrieveComments)
                {
                    article = GetComments(article);
                }

                article.Validate();

				yield return article;
            }
        }

        #region Private implementation

        /// <summary>
        /// If all comments are specified, calls itself recursively to find descendants.
        /// </summary>
        /// <returns></returns>
        private Post GetComments(Post comment)
        {
            if (!comment.HasKids) return comment;

            foreach (var commentId in comment.Kids)
            {
                string commentLink = $"https://hacker-news.firebaseio.com/v0/item/{commentId}.json?print=pretty";
                var jsonComment = InvokeHackerNewsApi(commentLink);
                Post descendantComment = JsonConvert.DeserializeObject<Post>(jsonComment.Result);
                comment.Comments.Add(comment);

                if (_commentRecursionLevel == CommentLevel.Full)
                {
					GetComments(descendantComment);
                }
            }

            return comment;
        }

        private async Task<string> InvokeHackerNewsApi(string url)
        {
            using (var httpClient = new HttpClient())
            {
				return await httpClient.GetStringAsync(new Uri(url));
            }
        }

        #endregion
    }
}
