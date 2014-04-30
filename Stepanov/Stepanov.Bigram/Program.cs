using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Stepanov.Bigram
{
    /// <summary>
    /// C# implementation of bigram program from Stepanov conversations course.
    /// Original code and data can be found at <https://github.com/psoberoi/stepanov-conversations-course>.
    /// Conversations can be watched on <https://www.youtube.com/playlist?list=PLHxtyCq_WDLXFAEA-lYoRNQIezL_vaSX->.
    /// </summary>
    class Program
    {
        static void Main(string[] args) {
            if (args.Length != 1) {
                Console.Error.WriteLine("one argument expected");
                Environment.Exit(1);
            }

            var stopwatch = Stopwatch.StartNew();
            using (var stream = File.OpenRead(args[0]))
            using (var reader = new StreamReader(stream)) {
                var bigrams = new List<Bigram>(reader.ReadWords().AsBigrams());
                bigrams.Sort();

                var bigramCounts = bigrams.RunLengthEncoding()
                    .OrderByDescending(i => i.Item1)
                    .ThenBy(i => i.Item2);

                foreach (var result in bigramCounts.Take(250)) {
                    Console.Out.WriteLine("{0} {1} {2}", result.Item2.First, result.Item2.Second, result.Item1);
                }
            }
            Console.Out.WriteLine("Ran in {0}ms.", stopwatch.ElapsedMilliseconds);
        }
    }

    [DebuggerDisplay("First={First}, Second={Second}")]
    public struct Bigram : IComparable<Bigram>, IEquatable<Bigram>
    {
        private readonly string _first;
        private readonly string _second;

        public Bigram(string first, string second) {
            _first = first;
            _second = second;
        }

        public string First { get { return _first; } }
        public string Second { get { return _second; } }

        #region IComparable<Bigram> Membres

        public int CompareTo(Bigram other) {
            var cmp = String.Compare(_first, other._first);
            if (cmp == 0)
                return String.Compare(_second, other._second);
            return cmp;
        }

        #endregion

        #region IEquatable<Bigram> Membres

        bool IEquatable<Bigram>.Equals(Bigram other) {
            return CompareTo(other) == 0;
        }

        #endregion
    }

    public static class BusinessExtensions
    {
        public static IEnumerable<Bigram> AsBigrams(this IEnumerable<string> words) {
            string previous;
            var enumerator = words.GetEnumerator();
            if (enumerator.MoveNext()) {
                previous = enumerator.Current;
                while (enumerator.MoveNext()) {
                    string current = enumerator.Current;
                    yield return new Bigram(previous, current);
                    previous = current;
                }
            }
        }
    }

    public static class TextReaderExtensions
    {
        private readonly static char[] spaces = { ' ', '\r', '\t', '\n' };

        public static IEnumerable<string> ReadWords(this TextReader reader) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                var words = line.Split(spaces, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < words.Length; i++) {
                    yield return words[i];
                }
            }
        }
    }

    public static class EnumerableExtensions
    {
        public static IEnumerable<Tuple<int, T>> RunLengthEncoding<T>(this IEnumerable<T> self) where T : IEquatable<T> {
            var enumerator = self.GetEnumerator();
            if (enumerator.MoveNext()) {
                int count = 1;
                T current = enumerator.Current;
                while (enumerator.MoveNext()) {
                    if (!enumerator.Current.Equals(current)) {
                        yield return Tuple.Create(count, current);
                        count = 1;
                        current = enumerator.Current;
                    } else {
                        ++count;
                    }
                }
                yield return Tuple.Create(count, current);
            }
        }
    }
}
