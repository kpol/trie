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
Trie trie = ["star", "start", "stack", "stop", "elf"];

         {root}
           /\
          s  e
         /    \
        t      l
       / \      \
     a    o     [f]
    / \    \
  [r]  c   [p]
  /     \
[t]     [k]

 where [char] -- is end of word
```

The library provides two implementations of the trie data structure:
 - `Trie` : `ICollection<string>`, this is a set which stores unique string
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
For performance tests I used 58110 English words of length from 2 to 22 chars. The table below shows prefix lookup time comparing to the Linq `Where` and `string.StartsWith`. Number of prefixes: 10

| Method                         | Mean          | Error       | StdDev      | Allocated |
|------------------------------- |--------------:|------------:|------------:|----------:|
| Trie_StartsWith                |  1,663.334 us |  25.0298 us |  22.1883 us |  782258 B |
| LinqSimple_StartsWith          | 17,899.727 us | 178.2255 us | 157.9923 us |  675940 B |
| Linq_StartsWith                |  1,880.081 us |  22.4351 us |  20.9858 us |  676893 B |
| Linq_DictionaryWithAllPrefixes |    775.352 us |   7.5212 us |   6.6673 us |  673053 B |
| Trie_Matches                   |      5.389 us |   0.0623 us |   0.0583 us |    9096 B |
| Trie_PatternStartsWith         |     10.924 us |   0.2181 us |   0.4455 us |   14896 B |
| String_PatternMatching         |    116.097 us |   2.0039 us |   2.6057 us |     416 B |
| String_PrefixPatternMatching   |    108.479 us |   1.8731 us |   1.7521 us |    3432 B |
| Regex_PatternMatching          |  4,410.587 us |  87.8454 us |  90.2107 us |     419 B |
| Regex_PrefixPatternMatching    |  4,309.215 us |  54.2987 us |  50.7910 us |    3435 B |

------
&copy; Kirill Polishchuk
