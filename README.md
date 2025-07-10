## Trie
**Trie** (a.k.a. prefix tree)  is an ordered tree data structure that is used to store an associative array where the keys are usually strings. All the descendants of a node have a common prefix of the string associated with that node, and the root is associated with the empty string.  
*Reference*: [Wikipedia](http://en.wikipedia.org/wiki/Trie)

[![CI Build](https://github.com/kpol/trie/workflows/CI%20Build/badge.svg)](https://github.com/kpol/trie/actions?query=workflow%3A%22CI+Build%22)
[![Nuget](https://img.shields.io/nuget/v/KTrie.svg?logo=nuget)](https://www.nuget.org/packages/KTrie)

## Advantages
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
 - `Trie` — implements `ICollection<string>` for storing unique strings.
 - `TrieDictionary<TValue>` — implements `IDictionary<string, TValue>` for key-value pairs.

## Usage

### Initialization
```csharp
// Initialization
TrieDictionary<int> trie = [];

// or using constructor with comparer
IEqualityComparer<char> comparer = ...; // specify the comparer
TrieDictionary<int> trieWithComparer = new(comparer);
```
Adding items to the trie

```csharp
trie.Add("key", 17);
```

- `Add` throws an `ArgumentNullException` if the key is `null`, and `ArgumentException` if the key already exists.
- You can overwrite existing values using the indexer:

```csharp
trie["key"] = 42;
```
(similar to `Dictionary<TKey, TValue>`)

### Prefix Lookup

The main benefit of a Trie is extremely fast prefix lookup.

```csharp
var result = trie.EnumerateByPrefix("abc");
```
This returns all key-value pairs where the key starts with the prefix `"abc"`.


There are multiple overloads of the `EnumerateByPrefix` method.
 - `EnumerateByPrefix(string prefix)`
 - `EnumerateByPrefix(IReadOnlyList<Character> pattern)`
 - `EnumerateByPrefix(ReadOnlySpan<char> prefix)`

### Pattern Matching

The `EnumerateMatches` method supports character pattern-based search:

```csharp
var result = trie.EnumerateMatches([Character.Any, 'c', Character.Any, Character.Any, 't']);
```

This matches words like `octet`, `scout`, or `scoot` using the regex-like pattern: `^.c.{2}t$`.

### API Methods

The API methods support efficient, allocation-free prefix matching with `ReadOnlySpan<char>`

#### `string? LongestPrefixMatch(string input)`
Returns the longest prefix of the specified input that exists as a full key in the trie.
This is useful for IP routing, token parsing, or search suggestion scenarios where the longest valid prefix match is needed.

```csharp
string? matched = trie.LongestPrefixMatch("starting");
```

> For example, if the trie contains "start", "star", and "stack", calling `LongestPrefixMatch("starting")` will return "start".

#### `bool TryAdd(string key, TValue value)`
Attempts to add the specified key and value to the `TrieDictionary`.  
Returns `true` if the key/value pair was added successfully; `false` if the key already exists.

```csharp
bool success = trie.TryAdd("alpha", 1);
```

#### `bool Remove(string key)`
Removes the value with the specified key from the `TrieDictionary`.  
Returns `true` if the element is successfully removed; `false` if the key was not found.

```csharp
bool removed = trie.Remove("key");
```

#### `bool ContainsKey(string key)`
Determines whether the `TrieDictionary` contains the specified key.

```csharp
bool containsKey = trie.ContainsKey("key");
```

#### `bool TryGetValue(string key, out TValue value)`
Attempts to get the value associated with the specified key.  
Returns `true` if the key is found; otherwise, `false`.

```csharp
bool keyFound = trie.TryGetValue("key", out int value);
```

## Benchmark tests
For performance tests I used 370105 English words (from: https://github.com/dwyl/english-words).

| Method                               | Mean          | Error         | StdDev        | Allocated    |
|------------------------------------- |--------------:|--------------:|--------------:|-------------:|
| Load_Trie                            | 274,942.00 us |  5,324.004 us | 13,356.886 us |  88595.87 KB |
| Load_DictionaryWithAllPrefixes       | 714,563.23 us | 14,096.916 us | 28,796.266 us | 315235.58 KB |
| Load_DictionaryGroupedByFirstLetter  |  13,731.29 us |    262.344 us |    257.657 us |      8691 KB |
| EnumerateByPrefix_Trie               |  11,975.54 us |    231.859 us |    367.753 us |   3604.32 KB |
| StartsWith_Linq                      | 120,723.09 us |  2,383.394 us |  4,047.184 us |   2843.47 KB |
| StartsWith_Linq_GroupedByFirstLetter |  10,880.59 us |    108.234 us |     90.380 us |   2844.41 KB |
| StartsWith_DictionaryWithAllPrefixes |   3,957.96 us |     56.184 us |     78.763 us |   2840.66 KB |
| Trie_EnumerateMatches                |      15.56 us |      0.307 us |      0.529 us |     17.99 KB |
| Trie_Pattern_EnumerateByPrefix       |      66.30 us |      1.308 us |      1.876 us |     65.59 KB |
| String_PatternMatching               |   1,014.94 us |     20.714 us |     60.750 us |      1.56 KB |
| String_PrefixPatternMatching         |   1,046.91 us |     20.888 us |     54.661 us |     33.72 KB |
| Regex_PatternMatching                |  29,936.00 us |    597.718 us |    930.576 us |      1.56 KB |
| Regex_PrefixPatternMatching          |  29,145.06 us |    563.901 us |    826.559 us |     33.72 KB |

------
&copy; Kirill Polishchuk
