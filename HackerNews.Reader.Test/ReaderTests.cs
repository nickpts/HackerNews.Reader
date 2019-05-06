using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;

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
        [ExpectedException(typeof(ArgumentException))]
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
   //         var scraper = new Reader(1, CommentLevel.Full);
			//var post = scraper.GetPostById(19797594, true).Result;
        }
    }
}
