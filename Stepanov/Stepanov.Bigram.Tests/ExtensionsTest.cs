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
        public void CheckRunLengthEncode() {
            int[] data = { 1, 2, 2, 3, 3, 3, 4, 4, 4, 4 };
            Tuple<int, int>[] expected = { 
                Tuple.Create(1, 1), 
                Tuple.Create( 2, 2 ), 
                Tuple.Create( 3, 3 ), 
                Tuple.Create( 4, 4 ), 
            };
            var actual = data.RunLengthEncode().ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CheckMergedWith() {
            int[] data1 = { 1, 2, 2, 3, 7, 8, 8, 10 };
            int[] data2 = { 2, 4, 4, 5, 7, 8, 9, 11, 11 };

            var encoded1 =data1.RunLengthEncode();
            var encoded2 =data2.RunLengthEncode();

            var expected = new []{ Tuple.Create(1,1), Tuple.Create(3, 2), Tuple.Create(1, 3) , Tuple.Create(2,4), Tuple.Create(1,5), Tuple.Create(2,7), Tuple.Create(3,8), Tuple.Create(1,9), Tuple.Create(1,10), Tuple.Create(2, 11) };
            var actual = encoded1.CombineWith(encoded2
                , Comparer<Tuple<int, int>>.Create((x, y) => x.Item2 - y.Item2)
                , (x, y) => Tuple.Create(x.Item1 + y.Item1, x.Item2))
                .ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        public void CheckCombineWith() {

        }
    }
}
