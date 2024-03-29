Trie
------
**Trie** (a.k.a. prefix tree)  is an ordered tree data structure that is used to store an associative array where the keys are usually strings. All the descendants of a node have a common prefix of the string associated with that node, and the root is associated with the empty string.  
*Reference*: [Wikipedia](http://en.wikipedia.org/wiki/Trie)

[![CI Build](https://github.com/kpol/trie/workflows/CI%20Build/badge.svg)](https://github.com/kpol/trie/actions?query=workflow%3A%22CI+Build%22)
[![Nuget](https://img.shields.io/nuget/v/KTrie.svg?logo=nuget)](https://www.nuget.org/packages/KTrie)

Advantages
------
 - Looking up keys is faster. Looking up a key of length **key** takes **O(|key|)** time
 - Looking up prefixes is faster. Looking up a prefix takes **O(|prefix|)** time
 - Removing takes **O(|key|)** time

```
Trie trie = ["star", "start", "stack", "stop", "stay", "key"];

          {root}
            /\
           s  k
          /    \
         t      e
        / \      \
      a    o     [y]
    / | \    \
  [r][y] c   [p]
  /       \
[t]       [k]

where [char] -- is end of word
```

The library provides two implementations of the trie data structure:
 - `Trie` : `ICollection<string>`, this is a set which stores unique strings
 - `TrieDictionary<TValue>` : `IDictionary<string, TValue>`, this is a key-value-pair collection

Tutorial
------
TrieDictionary initialization:

    // Initialization
    TrieDictionary<int> trie = [];

    // or using constructor with comparer
    IEqualityComparer<char> comparer = ...; // specify the comparer
    TrieDictionary<int> trieWithComparer = new(comparer);

Adding items to trie

    trie.Add("key", 17);

The `Add` method throws `ArgumentNullException` if a value with the specified key already exists, however setting the `Item` property overwrites the old value. In other words, `TrieDictionary<TValue>` has the same behavior as `Dictionary<TKey, TValue>`.

The main advantage of trie is really fast prefix lookup. To find all items of `TrieDictionary<TValue>` which have keys with given prefix use `StartsWith` method which returns `IEnumerable<KeyValuePair<string, TValue>>`:

    var result = trie.StartsWith("abc");

Another handy method is `Matches(IReadOnlyList<Character> pattern)`

    var result = trie.Matches([Character.Any, 'c', Character.Any, Character.Any, 't']);

which will return all words that match this regex: `^.c.{2}t$`, e.g.: `octet`, `scout`, `scoot`. 

There are two overloads of the `StartsWith` method:
 - `StartsWith(string value)`
 - `StartsWith(IReadOnlyList<Character> pattern)`

Benchmark tests
------
For performance tests I used 370105 English words (from: https://github.com/dwyl/english-words).

| Method                               | Mean          | Error        | StdDev       | Allocated   |
|------------------------------------- |--------------:|-------------:|-------------:|------------:|
| Load_Trie                            | 217,385.15 us | 4,059.770 us | 4,343.909 us | 72741.36 KB |
| Trie_StartsWith                      |  11,394.07 us |   219.067 us |   466.849 us |  3604.64 KB |
| Linq_StartsWith                      | 113,231.21 us |   780.126 us |   729.730 us |  2843.55 KB |
| Linq_GroupedByFirstLetter_StartsWith |  10,244.17 us |    91.502 us |    85.591 us |  2844.41 KB |
| Linq_DictionaryWithAllPrefixes       |   4,194.10 us |    41.829 us |    39.127 us |  2840.66 KB |
| Trie_Matches                         |      15.03 us |     0.287 us |     0.268 us |    18.05 KB |
| Trie_PatternStartsWith               |      62.98 us |     0.482 us |     0.451 us |    65.65 KB |
| String_PatternMatching               |     875.47 us |     7.239 us |     6.045 us |     1.56 KB |
| String_PrefixPatternMatching         |     895.72 us |     3.407 us |     2.660 us |    33.72 KB |
| Regex_PatternMatching                |  26,587.50 us |   206.420 us |   182.986 us |     1.57 KB |
| Regex_PrefixPatternMatching          |  27,545.88 us |   188.291 us |   176.127 us |    33.73 KB |

------
&copy; Kirill Polishchuk
