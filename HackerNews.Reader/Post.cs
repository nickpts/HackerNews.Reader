using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HackerNews.Reader
{
    /// <summary>
    /// Basic POCO, includes some validation and serialization logic
    /// </summary>
    public class Post
    {
        public string Title { get; set; }
        public string By { get; set; }
        public string Url { get; set; }
        public int Id { get; set; }
        public List<int> Kids { get; set; }
        public int Score { get; set; }
        public int Time { get; set; }
        public string Text { get; set; }
        public bool HasKids => Kids != null;
        public List<Post> Comments { get; set; } = new List<Post>();
        public bool IsHiring => Title.Contains("Ask HN: Who is hiring");

        public void Validate()
        {
            if (string.IsNullOrEmpty(Title) || Title.Length > 256)
                throw new ArgumentException("Title cannot be empty or longer than 256 characters");

            if (string.IsNullOrEmpty(By) || By.Length > 256)
                throw new ArgumentException("Author cannot be empty or longer than 256 characters");
            
            //not testing for url as ask/jobs/polls have empty url 

            if (!string.IsNullOrEmpty(Url) && !Uri.IsWellFormedUriString(Url, UriKind.Absolute))
                throw new ArgumentException($"Uri: {Url} is malformed");

            if (Score < 0)
                throw new ArgumentException("Points cannot be less than zero");

        }
    }
}
