using System;
using System.Collections.Generic;

using Newtonsoft.Json;

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
