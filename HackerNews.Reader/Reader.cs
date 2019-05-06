using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HackerNews.Reader
{
    /// <summary>
    /// Uses the HackerNews API to asynchronously retrieve top stories/ask/jobs/polls
    /// Returns either POCO or JSON
    /// </summary>
    public class Reader
    {
        private CommentLevel _commentRecursionLevel;
        private int _numberOfPosts = 0;

		public Dictionary<PostType, string> postTypes = new Dictionary<PostType, string>()
		{
			{ PostType.Stories, Constants.HackerNewsTopStoriesUri },
			{ PostType.Jobs, Constants.HackerNewsJobsUri },
			{ PostType.Ask, Constants.HackernewsAskUri },
			{ PostType.Show, Constants.HackerNewsShowUri }
        };

        public Reader(int numberOfPosts = 100, CommentLevel level = CommentLevel.None)
        {
            if (numberOfPosts == 0 || numberOfPosts < 0)
                throw new ArgumentException("Please specify a valid number of posts");

            _commentRecursionLevel = level;
            _numberOfPosts = numberOfPosts;
        }

		public async Task<Post> GetPostById(int id, bool returnNullIfNotHiringPost = false)
		{
			string link = $"https://hacker-news.firebaseio.com/v0/item/{id}.json?print=pretty";
			var list = await InvokeHackerNewsApi(link);
			Post article = JsonConvert.DeserializeObject<Post>(list);

			// Special case, due to the way "who's hiring" posts are stored in HackerNews
			// It is only possible to find them through the title, so this was condition
			// was added if one is only interested in them, it enables filtering posts that 
			// are not hiring without having to get full comments
			if (returnNullIfNotHiringPost && !article.IsHiring) return null;

			if (_commentRecursionLevel != CommentLevel.None)
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
				var post = GetPostById(id, true);

				if (post != null)
					yield return post.Result;
            }
        }

        #region Private implementation

        /// <summary>
        /// If all comments are specified, calls itself recursively to find descendants.
        /// </summary>
        /// <returns></returns>
        private Post GetComments(Post parent)
        {
            if (!parent.HasKids) return parent;

            foreach (var commentId in parent.Kids)
            {
                string commentLink = $"https://hacker-news.firebaseio.com/v0/item/{commentId}.json?print=pretty";
                var jsonComment = InvokeHackerNewsApi(commentLink);
                Post descendantComment = JsonConvert.DeserializeObject<Post>(jsonComment.Result);

				if (descendantComment == null)
					continue;

				parent.Comments.Add(descendantComment);

                if (_commentRecursionLevel == CommentLevel.Full)
                {
					GetComments(descendantComment);
                }
            }

            return parent;
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
