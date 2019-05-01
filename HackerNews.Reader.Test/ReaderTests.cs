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
        public void ReaderThrowsExceptionIfZeroPostsSpecified()
        {
            var reader = new Reader(0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReaderThrowsExceptionIfNegativePostsSpecified()
        {
            var reader = new Reader(-1);
        }

        [TestMethod]
        public void ReaderRetrievesPostsCorrectly()
        {
            var scraper = new Reader(5, true, CommentRecursionLevel.None);
            scraper.RetrievePosts();
            scraper.ShowExceptions();

            Console.ReadLine();
        }
        [TestMethod]
        public void ReaderRetrievesJsonPostsCorrectly()
        {
            var scraper = new Reader(1, true, CommentRecursionLevel.Full);
            var list = scraper.RetrievePostsInJsonFormat();
            scraper.ShowExceptions();

        }
    }
}
