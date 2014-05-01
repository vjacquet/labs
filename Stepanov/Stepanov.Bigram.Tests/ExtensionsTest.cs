using System;
using System.Collections.Generic;
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

        [TestMethod]
        public void CheckMergedWith() {
            int[] data1 = { 1, 2, 2, 3, 7, 8, 8, 10 };
            int[] data2 = { 2, 4, 4, 5, 7, 8, 9, 11, 11 };

            int[] expected = { 1, 2, 2, 2, 3, 4, 4, 5, 7, 7, 8, 8, 8, 9, 10, 11, 11 };
            int[] actual;

            actual = data1.MergeWith(data2, Comparer<int>.Default).ToArray();
            CollectionAssert.AreEqual(expected, actual);

            actual = data2.MergeWith(data1, Comparer<int>.Default).ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
