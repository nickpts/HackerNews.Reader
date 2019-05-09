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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HackerNews.Reader.Test
{
    [TestClass]
    public class PostTests
    {
        public const string MassiveTestValue = "RandomTestValueOfGreaterLengthRandomTestValueOfGreaterLength" +
                "RandomTestValueOfGreaterLengthRandomTestValueOfGreaterLength" +
                "RandomTestValueOfGreaterLengthRandomTestValueOfGreaterLength" +
                "RandomTestValueOfGreaterLengthRandomTestValueOfGreaterLength" +
                "RandomTestValueOfGreaterLengthRandomTestValueOfGreaterLength" +
                "RandomTestValueOfGreaterLengthRandomTestValueOfGreaterLength" +
                "RandomTestValueOfGreaterLengthRandomTestValueOfGreaterLength" +
                "RandomTestValueOfGreaterLengthRandomTestValueOfGreaterLength" +
                "RandomTestValueOfGreaterLengthRandomTestValueOfGreaterLength" +
                "RandomTestValueOfGreaterLengthRandomTestValueOfGreaterLength";

        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PostShouldThrowExceptionIfEmptyTitle()
        {
            var post = new Post() { };
            post.Validate();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PostShouldThrowExceptionIfTitleMoreThanLimit()
        {
            var post = new Post() { Title = MassiveTestValue };

            post.Validate();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PostShouldThrowExceptionIfAuthorEmpty()
        {
            var post = new Post() { Title = "test" };
            post.Validate();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PostShouldThrowExceptionIfAuthorMoreThanLimit()
        {
            var post = new Post() { Title = "test", By = MassiveTestValue };
            post.Validate();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PostShouldThrowExceptionIfUrlIsMalformed()
        {
            var post = new Post() { Title = "test", By = "test", Url = "test" };
            post.Validate();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PostShouldThrowExceptionIfScoreLessThanZero()
        {
            var post = new Post() { Title = "test", By = "test", Url = "http://www.hackernews.com", Score = -1 };
            post.Validate();
        }
    }
}
