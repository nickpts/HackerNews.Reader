using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HackerNews.Reader
{
    /// <summary>
    /// Basic DTO, includes some validation and serialization logic
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
        public string Type { get; set; }
        public int Rank { get; set; }

        public int Comments { get; set; }

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

            if (Comments < 0)
                throw new ArgumentException("Comments cannot be less than zero");

            if (Rank < 0)
                throw new ArgumentException("Rank cannot be less than zero");
        }

        [OnSerializing]
        internal void OnSerializingMethod(StreamingContext context)
        {
            Comments = Kids != null ? Kids.Count : 0;
            Kids = new List<int>();
        }
    }
}
