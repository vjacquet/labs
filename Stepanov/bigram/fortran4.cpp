// Original code and data can be found at <https://github.com/psoberoi/stepanov-conversations-course>.
// Conversations can be watched on <https://www.youtube.com/playlist?list=PLHxtyCq_WDLXFAEA-lYoRNQIezL_vaSX->.
// Code slightly adapted to compile with 2013 without boost and timing function added

#include <cctype>
#include <cstddef>
#include <cstdint>
#include <iostream>
#include <fstream>
#include <algorithm>
#include <vector>
#include <functional>
#include <memory>
#include <chrono>

#include "concepts.h"
#include "type_function.h"

// To fake concepts on the stopwatch
#define Chrono typename 
#define Duration typename 

template<Chrono C>
struct stopwatch {
	typedef typename std::chrono::time_point<C> time_point;
	typedef typename C::duration duration;

	const time_point since;

	stopwatch() : since(C::now()) {}

	template<Duration D>
	D elapsed() const {
		return std::chrono::duration_cast<D>(C::now() - since);
	}
};

using namespace std;
using namespace std::chrono;

typedef uint64_t prefix_t;
typedef pair<prefix_t, prefix_t> bigram_t;

typedef const char* pchar_t;

// Library and STL-like algorithms

template <UnsignedIntegral N>
uint8_t high_byte(N x) {
  return x >> ((sizeof(N) - 1) * 8);
}

template <InputIterator I>
void print_range(I first, I last) {
  while (first != last) {
    cout << *first;
    ++first;
  }
}

template <ForwardIterator I>
pair<typename std::iterator_traits<I>::difference_type, I> adjacent_count_if(I first, I last) {
	typename std::iterator_traits<I>::difference_type n {0};
  if (first == last) return {n, first};
  while (next(first) != last && *first == *next(first)) {
    ++n;
    ++first;
  }
  return make_pair(++n, next(first)); // VJJ: Had to use make_pair otherwise the compiler crashes.
}

template <ForwardIterator I, OutputIterator O>
O unique_copy_with_count(I first, I last, O result) {
  while (true) {
    auto cnt = adjacent_count_if(first, last);
    if (!cnt.first) return result;
    *result = {cnt.first, *first};
    ++result;
    first = cnt.second;
  }
}

// "optimized" versions of library functions

inline
bool myisspace(char x) {
  if (x > ' ') return false;
  return (x == ' ' || x == '\r' || x == '\t' || x == '\n');
}

// Application code

pair<prefix_t, pchar_t> next_prefix(pchar_t first, pchar_t last) {
  prefix_t prefix = 0;
  first = find_if(first, last, [](char x){ return !myisspace(x); });
  if (first == last) return {0, first};
  auto offset = sizeof(prefix_t);

  while (first != last && !myisspace(*first)) {
    if (offset) {
      prefix <<= 8;
      prefix |= static_cast<uint8_t>(*first);
      --offset;
    }
    ++first;
  }

  return {prefix << (offset * 8), first};
}

void print_prefix(prefix_t prefix) {
  auto p = prefix;
  while (p) {
    char c = high_byte(p);
    cout << c;
    p <<= 8;
  }
}

bool is_valid_bigram(const bigram_t& x) {
 return x.second != 0;
}

pchar_t next_bigram(bigram_t& x, pchar_t first, pchar_t last) {
  x.first = x.second;
  auto tmp = next_prefix(first, last);
  x.second = tmp.first;
  return tmp.second;
}

void print_bigram(const bigram_t& x) {
  print_prefix(x.first);
  cout << " ";
  print_prefix(x.second);
}

vector<bigram_t> make_bigram_index(pchar_t first, pchar_t last) {
  vector<bigram_t> vec;
  bigram_t x{0, 0};
  first = next_bigram(x, first, last);
  while (true) {
    first = next_bigram(x, first, last);
    if (!is_valid_bigram(x)) return vec;
    vec.push_back(x);
  }
}

string read_file(const char* filename) {
	ifstream ifs(filename);
	return string(istreambuf_iterator<char>(ifs), istreambuf_iterator<char>());
}

int main(int argc, char** argv) {
  if (argc != 2) {
    cerr << "one argument expected" << endl;
    return 1;
  }

  stopwatch<high_resolution_clock> w;
  auto content = read_file(argv[1]);
  pchar_t data = content.data();

  auto bigram_index = make_bigram_index(data, data + content.size());

  sort(begin(bigram_index), end(bigram_index));

  vector<pair<size_t, bigram_t>> bigram_count_index;
  unique_copy_with_count(begin(bigram_index), end(bigram_index),
                         back_inserter(bigram_count_index));

  partial_sort(begin(bigram_count_index), begin(bigram_count_index) + 250, end(bigram_count_index),
               greater<pair<size_t, bigram_t>>());

  auto first = begin(bigram_count_index);
  auto last = end(bigram_count_index);
  int i = 0;
  while (first != last && i < 250) {
    print_bigram(first->second);
    cout << " " << first->first << endl;
    ++first;
    ++i;
  }

  cout << "Ran in " << w.elapsed<milliseconds>().count() << "ms." << endl;
}
