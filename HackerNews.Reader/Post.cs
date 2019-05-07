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

using System;
using System.Collections.Generic;

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
		public string Type { get; set; }

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
