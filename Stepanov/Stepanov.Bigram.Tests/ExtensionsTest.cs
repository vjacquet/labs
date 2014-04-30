using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stepanov.Bigram.Tests
{
    [TestClass]
    public class ExtensionsTest
    {
        [TestMethod]
        public void CheckRunLengthEncoding() {
            int[] data = { 1, 2, 2, 3, 3, 3, 4, 4, 4, 4 };
            Tuple<int, int>[] expected = { 
                Tuple.Create(1, 1), 
                Tuple.Create( 2, 2 ), 
                Tuple.Create( 3, 3 ), 
                Tuple.Create( 4, 4 ), 
            };
            var actual = data.RunLengthEncoding().ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
