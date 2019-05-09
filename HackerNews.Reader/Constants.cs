#region License

/*
 * All content copyright Marko Lahma, unless otherwise indicated. All rights reserved.
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

using System.Collections.Generic;

namespace HackerNews.Reader
{
	/// <summary>
	/// Specifies type of posts to retrieve
	/// </summary>
	public enum PostType
	{
		/// <summary>
		/// General interest stories submitted from users
		/// </summary>
		Stories,

		/// <summary>
		/// Employers advertising openings
		/// </summary>
		Jobs,

		/// <summary>
		/// Help with technical or other questions
		/// </summary>
		Ask,

		/// <summary>
		/// Demonstrations of technical projects/papers, etc.
		/// </summary>
		Show
	}

	/// <summary>
	/// Specifies how deep to go when retrieving comments for a story
	/// </summary>
	public enum CommentLevel
	{
		/// <summary>
		/// Do not rerieve any comments
		/// </summary>
		None, 

		/// <summary>
		/// Only direct descendants to the story, no replies to comments
		/// </summary>
		FirstLevel, 

		/// <summary>
		/// All descendnants
		/// </summary>
		Full 
	}

	public class Constants
	{
		/// <summary>
		/// TODO: move these out to a config file
		/// </summary>
		public const string HackerNewsTopStoriesUri = "https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty";
		public const string HackerNewsJobsUri = "https://hacker-news.firebaseio.com/v0/jobstories.json?print=pretty";
		public const string HackernewsAskUri = "https://hacker-news.firebaseio.com/v0/askstories.json?print=pretty";
		public const string HackerNewsShowUri = "https://hacker-news.firebaseio.com/v0/showstories.json?print=pretty";
		
		public static readonly Dictionary<PostType, string> postTypes = new Dictionary<PostType, string>()
		{
			{ PostType.Stories, HackerNewsTopStoriesUri },
			{ PostType.Jobs, HackerNewsJobsUri },
			{ PostType.Ask, HackernewsAskUri },
			{ PostType.Show, HackerNewsShowUri }
		};

	}
}
