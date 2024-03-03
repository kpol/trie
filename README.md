Trie
------
**Trie** (a.k.a. prefix tree)  is an ordered tree data structure that is used to store an associative array where the keys are usually strings. All the descendants of a node have a common prefix of the string associated with that node, and the root is associated with the empty string.  
*Reference*: [Wikipedia &ndash; trie](http://en.wikipedia.org/wiki/Trie)

[![CI Build](https://github.com/kpol/trie/workflows/CI%20Build/badge.svg)](https://github.com/kpol/trie/actions?query=workflow%3A%22CI+Build%22)
[![Nuget](https://img.shields.io/nuget/v/KTrie.svg?logo=nuget)](https://www.nuget.org/packages/KTrie)

Advantages
------
 - Looking up keys is faster. Looking up a key of length **key** takes **O(|key|)** time
 - Looking up prefixes is faster. Looking up a prefix takes **O(|prefix|)** time
 - Removing takes **O(|key|)** time

```
Trie trie = ["abc", "abcd", "abx", "xyz"];

        root
         /\
        a  x
       /    \
      b      y
     / \      \
   [c]  [x]   [z]
   /
 [d]

 where [char] -- is end of word
```

The library provides two implementations of the trie data structure:
 - `Trie` : `ICollection<string>`
 - `TrieDictionary<TValue>` : `IDictionary<string, TValue>`

Tutorial
------
Trie initialization:

    // Initialization
    TrieDictionary<int> trie = [];

    // or using constructor with comparer
    IEqualityComparer<char> comparer = ...; // specify the comparer
    TrieDictionary<int> trieWithComparer = new(comparer);

Adding items to trie

    trie.Add("key", 17);

The `Add` method throws `ArgumentNullException` if a value with the specified key already exists, however setting the `Item` property overwrites the old value. In other words, `TrieDictionary<TValue>` has the same behavior as `Dictionary<TKey, TValue>`.

The main advantage of trie is really fast prefix lookup. To find all items of `TrieDictionary<TValue>` which have keys with given prefix use `GetByPrefix` method which returns `IEnumerable<KeyValuePair<string, TValue>>`:

    var result = trie.GetByPrefix("abc");

Another handy method is `GetByPattern(IReadOnlyList<Character> pattern)`

    var result = trie.GetByPattern([Character.Any, 'c', Character.Any, Character.Any, 't']);

which will return all words that match this regex: `^.c.{2}t$`, e.g.: `octet`, `scout`, `scoot`. 

There are two overloads of the `GetByPrefix` method:
 - `GetByPrefix(string prefix)`
 - `GetByPrefix(IReadOnlyList<Character> pattern)`

Benchmark tests
------
For performance tests I used 58110 English words of length from 2 to 22 chars. The table below shows prefix lookup time comparing to the Linq `Where` and `string.StartsWith`. Number of prefixes: 10

| Method                         | Mean          | Error       | StdDev      | Allocated |
|------------------------------- |--------------:|------------:|------------:|----------:|
| Trie_GetByPrefix               |  1,644.170 us |  16.8797 us |  15.7892 us |  782259 B |
| LinqSimple_StartsWith          | 17,401.058 us | 137.4701 us | 128.5896 us |  675940 B |
| Linq_StartsWith                |  1,826.997 us |  14.3963 us |  13.4664 us |  676893 B |
| Linq_DictionaryWithAllPrefixes |    771.180 us |   4.0142 us |   3.5585 us |  673053 B |
| Trie_PatternMatching           |      5.131 us |   0.0448 us |   0.0419 us |    9096 B |
| Trie_PrefixPatternMatching     |     10.128 us |   0.1118 us |   0.0991 us |   14896 B |
| String_PatternMatching         |    108.502 us |   0.7177 us |   0.6362 us |     416 B |
| String_PrefixPatternMatching   |    110.099 us |   0.8502 us |   0.7952 us |    3432 B |
| Regex_PatternMatching          |  4,232.071 us |  31.9178 us |  26.6528 us |     419 B |
| Regex_PrefixPatternMatching    |  4,581.950 us |  40.7450 us |  38.1129 us |    3435 B |


------
&copy; Kirill Polishchuk
