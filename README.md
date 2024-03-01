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

    TrieDictionary<int> trie = [];

or using constructor which accepts `IEqualityComparer<char> comparer` interface:

    TrieDictionary<int> trie = new(comparer);

To add items to trie:

    trie.Add("key", 17);

The `Add` method throws `ArgumentNullException` if a value with the specified key already exists, however setting the `Item` property overwrites the old value. In other words, `TrieDictionary<TValue>` has the same behavior as `Dictionary<TKey, TValue>`.

The main advantage of trie is really fast prefix lookup. To find all items of `TrieDictionary<TValue>` which have keys with given prefix use `GetByPrefix` method which returns `IEnumerable<KeyValuePair<string, TValue>>`:

    var result = trie.GetByPrefix("ABC");

Benchmark tests
------
For performance tests I used 58110 English words of length from 2 to 22 chars. The table below shows prefix lookup time comparing to the Linq `Where` and `string.StartsWith`. Number of prefixes: 10

| Method                         | Mean       | Error    | StdDev   | Allocated |
|------------------------------- |-----------:|---------:|---------:|----------:|
| Trie_GetByPrefix               |   737.5 us |  8.74 us |  8.18 us | 327.01 KB |
| Linq_StartsWith                | 1,414.9 us | 28.28 us | 38.71 us | 318.81 KB |
| TrieDictionary_GetByPrefix     |   981.7 us | 19.42 us | 34.52 us | 452.47 KB |


------
&copy; Kirill Polishchuk
