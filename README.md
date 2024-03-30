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

| Method                               | Mean          | Error        | StdDev       | Allocated    |
|------------------------------------- |--------------:|-------------:|-------------:|-------------:|
| Load_Trie                            | 211,557.48 us | 1,981.525 us | 1,756.570 us |  72741.27 KB |
| Load_DictionaryWithAllPrefixes       | 577,935.48 us | 6,096.177 us | 5,090.583 us | 317389.57 KB |
| Trie_StartsWith                      |  11,420.52 us |    78.619 us |    69.693 us |   3604.64 KB |
| Linq_StartsWith                      | 117,671.68 us | 1,777.550 us | 1,662.722 us |   2843.55 KB |
| Linq_GroupedByFirstLetter_StartsWith |  10,544.61 us |   206.705 us |   339.622 us |   2844.41 KB |
| Linq_DictionaryWithAllPrefixes       |   3,593.91 us |    69.920 us |    80.520 us |   2840.66 KB |
| Trie_Matches                         |      15.13 us |     0.298 us |     0.446 us |     18.05 KB |
| Trie_PatternStartsWith               |      66.07 us |     1.306 us |     1.504 us |     65.65 KB |
| String_PatternMatching               |     887.43 us |    13.962 us |    12.377 us |      1.56 KB |
| String_PrefixPatternMatching         |     911.10 us |    14.261 us |    13.340 us |     33.72 KB |
| Regex_PatternMatching                |  27,146.03 us |   232.150 us |   217.153 us |      1.57 KB |
| Regex_PrefixPatternMatching          |  27,414.88 us |   265.306 us |   248.168 us |     33.73 KB |

------
&copy; Kirill Polishchuk
