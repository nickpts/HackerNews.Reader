using System;
using System.Collections.Generic;
using System.Text;

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
		None, // do not retrieve any comments
		FirstLevel, // only comments directly to the story, no replies
		Full // all comments
	}

	public class Constants
	{
		public const string HackerNewsTopStoriesUri = "https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty";
		public const string HackerNewsJobsUri = "https://hacker-news.firebaseio.com/v0/jobstories.json?print=pretty";
		public const string HackernewsAskUri = "https://hacker-news.firebaseio.com/v0/askstories.json?print=pretty";
		public const string HackerNewsShowUri = "https://hacker-news.firebaseio.com/v0/showstories.json?print=pretty";

	}
}
