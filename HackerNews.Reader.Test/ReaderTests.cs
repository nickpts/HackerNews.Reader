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
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            var reader = new PostReader(0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReaderThrowsExceptionIfNegativePostsSpecified()
        {
            var reader = new PostReader(-1);
        }

        [TestMethod]
        public void ReaderRetrievesPostsCorrectly()
        {
            var reader = new PostReader(1, CommentLevel.Full);
			var ids = new int[] { 19797594 };
			var post = reader.GetById(ids, new CancellationToken());
        }
    }
}
