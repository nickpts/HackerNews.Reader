using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using FluentAssertions;

namespace HackerNews.Reader.Test
{
    [TestClass]
    public class ReaderTests
    {
        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReaderThrowsExceptionIfNoSiteSpecified()
        {
            var reader = new Reader(string.Empty, string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ReaderThrowsExceptionIfUnsupportedSite()
        {
            var reader = new Reader("dailywtf", string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReaderThrowsExceptionIfInvalidInput()
        {
            var reader = new Reader("hackernews", "test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReaderThrowsExceptionIfInvalidNumberOfPosts()
        {
            var reader = new Reader("hackernews", "110");
        }


    }
}
