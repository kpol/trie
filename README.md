Trie
------
**Trie** (a.k.a. prefix tree)  is an ordered tree data structure that is used to store an associative array where the keys are usually strings. All the descendants of a node have a common prefix of the string associated with that node, and the root is associated with the empty string.  
*Reference*: [Wikipedia &ndash; trie](http://en.wikipedia.org/wiki/Trie)

[![Build status](https://ci.appveyor.com/api/projects/status/8rbgoxqio76ynj4h?svg=true)](https://ci.appveyor.com/project/kpol/trie)

Advantages
------
 - Looking up keys is faster. Looking up a key of length **key** takes **O(|key|)** time
 - Looking up prefixes is faster. Looking up a prefix takes **O(|prefix|)** time
 - Removing takes **O(|key|)** time

Tutorial
------
`Trie<TValue>` implements `IDictionary<string, TValue>` interface.

Trie initialization:

    var trie = new Trie<TValue>();

or using constructor which accepts `IEqualityComparer<char> comparer` interface:

    var trie = new Trie<TValue>(comparer);

To add items to trie:

    trie.Add("key", value);
    trie.AddRange(trieEntries);

The `Add`, `AddRange` methods throw `ArgumentNullException` if a value with the specified key already exists, however setting the `Item` property overwrites the old value. In other words, `Trie<TValue>` has the same behavior as `Dictionary<TKey, TValue>`.

The main advantage of trie is really fast prefix lookup. To find all items of `Trie<TValue>` which have keys with given prefix use `GetByPrefix` method which returns `IEnumerable<StringEntry<TValue>>`:

    var result = trie.GetByPrefix("ABC");

Benchmark tests
------
For performance tests I have used vocabulary with 58110 English words with length from 2 to 22 chars. On my PC loading all words to trie takes: 140 ms.  
In following table are presented results of searching items by prefix:
<table>
  <tr>
    <th rowspan="2">Prefixes count:</th>
    <th colspan="2">Time, ms</th>
  </tr>
  <tr>
    <th>LINQ &ndash; StartsWith</th>
    <th>Trie &ndash; GetByPrefix</th>
  </tr>
  <tr>
    <td>30</td>
    <td>338</td>
    <td>57</td>
  </tr>
  <tr>
    <td>60</td>
    <td>669</td>
    <td>100</td>
  </tr>
  <tr>
    <td>90</td>
    <td>1001</td>
    <td>193</td>
  </tr>
  <tr>
    <td>120</td>
    <td>1342</td>
    <td>265</td>
  </tr>
  <tr>
    <td>150</td>
    <td>1678</td>
    <td>251</td>
  </tr>
  <tr>
    <td>180</td>
    <td>2023</td>
    <td>336</td>
  </tr>
  <tr>
    <td>210</td>
    <td>2380</td>
    <td>383</td>
  </tr>
  <tr>
    <td>240</td>
    <td>2724</td>
    <td>408</td>
  </tr>
  <tr>
    <td>270</td>
    <td>2995</td>
    <td>453</td>
  </tr>
  <tr>
    <td>300</td>
    <td>3405</td>
    <td>521</td>
  </tr>
  <tr>
    <td>1500</td>
    <td>16473</td>
    <td>2366</td>
  </tr>
  <tr>
    <td>3000</td>
    <td>33263</td>
    <td>4162</td>
  </tr>
</table>

Tests using brute-force prefix generator. 
<table>
  <tr>
    <th rowspan="2">Prefix length:</th>
    <th rowspan="2">Prefixes count:</th>
    <th colspan="2">Time, ms</th>
  </tr>
  <tr>
    <th>LINQ &ndash; StartsWith</th>
    <th>Trie &ndash; GetByPrefix</th>
  </tr>
  <tr>
    <td>1</td>
    <td>26</td>
    <td>307</td>
    <td>167</td>
  </tr>
  <tr>
    <td>2</td>
    <td>676</td>
    <td>7675</td>
    <td>163</td>
  </tr>
  <tr>
    <td>3</td>
    <td>17576</td>
    <td>197057</td>
    <td>168</td>
  </tr>
  <tr>
    <td>4</td>
    <td>456976</td>
    <td>&mdash;</td>
    <td>267</td>
  </tr>
  <tr>
    <td>5</td>
    <td>11881376</td>
    <td>&mdash;</td>
    <td>4632</td>
  </tr>
</table>

------
&copy; Kirill Polishchuk
