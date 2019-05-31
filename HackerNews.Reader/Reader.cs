#region License

/*
 * All content copyright Nick Patsaris, unless otherwise indicated. All rights reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy
 * of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 */

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace HackerNews.Reader
{
	/// <summary>
	/// Uses the HackerNews API to asynchronously retrieve top stories/ask/jobs/polls
	/// Returns either POCO or JSON
	/// </summary>
	public class PostReader
	{
		private CommentLevel _commentRecursionLevel;
		private int _numberOfPosts = 0;

		public PostReader(int numberfOfPosts = 100, CommentLevel level = CommentLevel.None)
		{
			if (numberfOfPosts == 0 || numberfOfPosts < 0) throw new ArgumentException(nameof(numberfOfPosts));

			_commentRecursionLevel = level;
			_numberOfPosts = numberfOfPosts;
		}

		/// <summary>
		/// Gets top story posts
		/// </summary>
		public IEnumerable<Post> GetTopStories(CancellationToken token) => Get(token, PostType.Stories);
		
		/// <summary>
		/// Gets top job posts
		/// </summary>
		public IEnumerable<Post> GetTopJobs(CancellationToken token) => Get(token, PostType.Jobs);
		
		/// <summary>
		/// Get top show posts
		/// </summary>
		public IEnumerable<Post> GetTopShow(CancellationToken token) => Get(token, PostType.Show);
		
		/// <summary>
		/// Get top ask posts
		/// </summary>
		public IEnumerable<Post> GetTopAsk(CancellationToken token) => Get(token, PostType.Ask);
		
		/// <summary>
		/// Gets posts by their ids
		/// </summary>
		public IEnumerable<Post> GetById(int[] ids, CancellationToken token)
		{
			foreach (int id in ids)
			{
				yield return GetById(id, token).Result;
			}
		}

		/// <summary>
		/// Gets posts and comments in json format, optionally outputs to console
		/// </summary>
		public IEnumerable<string> GetPostsInJsonFormat(CancellationToken token, PostType postType = PostType.Stories, bool outputToConsole = false)
		{
			var posts = Get(token, postType);

			foreach (var post in posts)
			{
				token.ThrowIfCancellationRequested();

				string json = JsonConvert.SerializeObject(post, Formatting.Indented);

				if (outputToConsole)
					Console.WriteLine(json);

				yield return json;
			}
		}

		#region Private implementation

		/// <summary>
		/// Gets the top n number of posts specified on instantiation. 
		/// </summary>
		/// <returns></returns>
		private IEnumerable<Post> Get(CancellationToken token, PostType postType = PostType.Stories)
		{
			var uri = InvokeHackerNewsApi(Constants.postTypes[postType]);
			var ids = JsonConvert.DeserializeObject<List<int>>(uri.Result).Take(_numberOfPosts).ToList();

			foreach (int i in ids)
			{
				token.ThrowIfCancellationRequested();
				yield return GetById(i, token).Result;
			}
		}

		/// <summary>
		/// /// If all comments are specified, calls itself recursively to find descendants.
		/// /// </summary>
		private Post GetComments(Post parent, CancellationToken token)
		{
			if (!parent.HasKids) return parent;

			foreach (var commentId in parent.Kids)
			{
				string commentLink = $"https://hacker-news.firebaseio.com/v0/item/{commentId}.json?print=pretty";
				var jsonComment = InvokeHackerNewsApi(commentLink);
				Post descendantComment = JsonConvert.DeserializeObject<Post>(jsonComment.Result);

				if (token.IsCancellationRequested)
					throw new TaskCanceledException($"Stopped at comment: { descendantComment.Id } at { DateTime.Now }");

				if (descendantComment == null)
					continue;

				parent.Comments.Add(descendantComment);

				if (_commentRecursionLevel == CommentLevel.Full)
				{
					GetComments(descendantComment, token);
				}
			}

			return parent;
		}

		/// <summary>
		/// Gets a single post by id
		/// </summary>
		private async Task<Post> GetById(int id, CancellationToken token, bool returnNullIfNotHiringPost = false)
		{
			string link = $"https://hacker-news.firebaseio.com/v0/item/{id}.json?print=pretty";
			var list = await InvokeHackerNewsApi(link);
			Post article = JsonConvert.DeserializeObject<Post>(list);

			if (token.IsCancellationRequested)
				throw new TaskCanceledException($"Stopped at parent: { article.Id } at { DateTime.Now }");

			if (_commentRecursionLevel != CommentLevel.None)
			{
				article = GetComments(article, token);
			}

			article.Validate();

			return article;
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
