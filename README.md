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

The library provides four implementations of the trie data structure:
 - `TrieSet<T>`
 - `Trie<TKey, TValue>`
 - `StringTrieSet`
 - `StringTrie<TValue>`

Tutorial
------
`Trie<TValue>` implements `IDictionary<string, TValue>` interface.

Trie initialization:

    var trie = new StringTrie<TValue>();

or using constructor which accepts `IEqualityComparer<char> comparer` interface:

    var trie = new StringTrie<TValue>(comparer);

To add items to trie:

    trie.Add("key", value);
    trie.AddRange(trieEntries);

The `Add`, `AddRange` methods throw `ArgumentNullException` if a value with the specified key already exists, however setting the `Item` property overwrites the old value. In other words, `StringTrie<TValue>` has the same behavior as `Dictionary<TKey, TValue>`.

The main advantage of trie is really fast prefix lookup. To find all items of `StringTrie<TValue>` which have keys with given prefix use `GetByPrefix` method which returns `IEnumerable<StringEntry<TValue>>`:

    var result = trie.GetByPrefix("ABC");

Benchmark tests
------
For performance tests I used 58110 English words of length from 2 to 22 chars. The table below shows prefix lookup time comparing to the Linq `Where` and `string.StartsWith`. Number of prefixes: 10

|           Method |      Mean |     Error |    StdDev |
|----------------- |----------:|----------:|----------:|
| Trie_GetByPrefix |  1.879 ms | 0.0258 ms | 0.0229 ms |
|  Linq_StartsWith | 42.685 ms | 0.5857 ms | 0.5192 ms |


------
&copy; Kirill Polishchuk
