using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlgoLib.Tests
{
    [TestClass]
    public class TrieTests
    {
        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void AddNullKey()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Trie<bool> {{null, false}};
        }

        [TestMethod]
        public void AddRange()
        {
            const int count = 10;

            var trie = new Trie<bool>();

            trie.AddRange(Enumerable.Range(0, count).Select(i => new TrieEntry<bool>(i.ToString(), false)));

            Assert.AreEqual(count, trie.Count);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void AddWithSameKey()
        {
            // ReSharper disable once ObjectCreationAsStatement
             new Trie<bool> {{"a", false}, {"a", true}};
        }

        [TestMethod]
        public void Clear()
        {
            var trie = new Trie<bool> {{"ABC", false}, {"AB", false}, {"ADE", false}, {"ABCDE", false}};

            trie.Clear();

            Assert.AreEqual(0, trie.Count);
        }

        [TestMethod]
        public void Contains()
        {
            var trie = new Trie<bool> {{"ABC", false}, {"AB", false}, {"ADE", true}, {"ABCDE", false}};

            var t = (IDictionary<string, bool>)trie;

            Assert.IsTrue(t.Contains(new KeyValuePair<string, bool>("ABC", false)));
            Assert.IsTrue(t.Contains(new KeyValuePair<string, bool>("AB", false)));
            Assert.IsTrue(t.Contains(new KeyValuePair<string, bool>("ADE", true)));
            Assert.IsTrue(t.Contains(new KeyValuePair<string, bool>("ABCDE", false)));

            Assert.IsFalse(t.Contains(new KeyValuePair<string, bool>("X", false)));
            Assert.IsFalse(t.Contains(new KeyValuePair<string, bool>("ADE", false)));
            Assert.IsFalse(t.Contains(new KeyValuePair<string, bool>("ABCD", false)));
        }

        [TestMethod]
        public void ContainsKey()
        {
            var trie = new Trie<bool> {{"ABC", false}, {"AB", false}, {"ADE", false}, {"ABCDE", false}};


            Assert.IsTrue(trie.ContainsKey("ABC"));
            Assert.IsTrue(trie.ContainsKey("AB"));
            Assert.IsTrue(trie.ContainsKey("ADE"));
            Assert.IsTrue(trie.ContainsKey("ABCDE"));

            Assert.IsFalse(trie.ContainsKey("A"));
            Assert.IsFalse(trie.ContainsKey("AC"));
            Assert.IsFalse(trie.ContainsKey("ABCD"));
        }

        [TestMethod]
        public void ContainsKeyClear()
        {
            var trie = new Trie<bool> {{"ABC", false}, {"AB", false}, {"ADE", false}, {"ABCDE", false}};


            trie.Clear();

            Assert.IsFalse(trie.ContainsKey("ABC"));
            Assert.IsFalse(trie.ContainsKey("AB"));
            Assert.IsFalse(trie.ContainsKey("ADE"));
            Assert.IsFalse(trie.ContainsKey("ABCDE"));
        }

        [TestMethod]
        public void CopyTo()
        {
            var trie = new Trie<bool> {{"ABC", true}, {"AB", false}, {"ADE", true}, {"ABCDE", false}};


            var destinationArray = new KeyValuePair<string, bool>[6];

            ((ICollection<KeyValuePair<string, bool>>)trie).CopyTo(destinationArray, 1);

            var result = destinationArray.Where(i => i.Key != null).OrderBy(i => i.Key).ToArray();

            var expected = new[]
                {
                    new KeyValuePair<string, bool>("AB", false), 
                    new KeyValuePair<string, bool>("ABC", true), 
                    new KeyValuePair<string, bool>("ABCDE", false), 
                    new KeyValuePair<string, bool>("ADE", true)
                };

            Assert.AreEqual(new KeyValuePair<string, bool>(), destinationArray[0]);
            Assert.AreEqual(new KeyValuePair<string, bool>(), destinationArray[destinationArray.Length - 1]);
            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestMethod]
        public void GetByPrefix()
        {
            var trie = new Trie<bool> {{"ABC", false}, {"AB", false}};

            (trie as ICollection<KeyValuePair<string, bool>>).Add(new KeyValuePair<string, bool>("ADE", false));
            trie.Add(new TrieEntry<bool>("ABCDE", false));

            var result = trie.GetByPrefix("ABC").Select(t => t.Key).OrderBy(t => t);

            string[] expectedResult = { "ABC", "ABCDE" };

            Assert.AreEqual(4, trie.Count);
            Assert.IsTrue(expectedResult.SequenceEqual(result));
        }

        [TestMethod]
        public void GetEnumerator()
        {
            var trie = new Trie<bool> {{"ABC", false}, {"AB", false}, {"ADE", false}, {"ABCDE", false}};


            var result = trie.Select(kvp => kvp.Key).OrderBy(w => w).ToArray();
            var resultEnumerator =
                trie.OfType<KeyValuePair<string, bool>>().Select(k => k.Key).OrderBy(w => w).ToArray();

            CollectionAssert.AreEqual(new[] { "AB", "ABC", "ABCDE", "ADE" }, result);
            CollectionAssert.AreEqual(new[] { "AB", "ABC", "ABCDE", "ADE" }, resultEnumerator);
        }

        [TestMethod]
        public void IsReadOnly()
        {
            var trie = new Trie<bool>();

            Assert.IsFalse(((IDictionary<string, bool>)trie).IsReadOnly);
        }

        [TestMethod]
        public void ItemsGet()
        {
            var trie = new Trie<bool> {{"ABC", false}, {"AB", false}, {"ADE", true}, {"ABCDE", false}};

            Assert.AreEqual(false, trie["ABC"]);
            Assert.AreEqual(false, trie["AB"]);
            Assert.AreEqual(true, trie["ADE"]);
            Assert.AreEqual(false, trie["ABCDE"]);
        }

        [ExpectedException(typeof(KeyNotFoundException))]
        [TestMethod]
        public void ItemsGetException()
        {
            var trie = new Trie<bool> {{"ABC", false}, {"AB", false}, {"ADE", true}, {"ABCDE", false}};

            // ReSharper disable once UnusedVariable
            var result = trie["A"];
        }

        [TestMethod]
        public void ItemsSet()
        {
            var trie = new Trie<bool> {["ABC"] = true};

            Assert.AreEqual(true, trie["ABC"]);

            trie["AB"] = true;

            Assert.AreEqual(true, trie["AB"]);
        }

        [TestMethod]
        public void KeysValues()
        {
            var trie = new Trie<bool> {{"ABC", false}, {"AB", true}, {"ADE", false}, {"ABCDE", true}};

            Assert.IsTrue(new[] { "AB", "ABC", "ABCDE", "ADE" }.SequenceEqual(trie.Keys.OrderBy(s => s)));
            Assert.IsTrue(new[] { false, false, true, true }.SequenceEqual(trie.Values.OrderBy(s => s)));
        }

        [TestMethod]
        public void Remove()
        {
            const int initialCount = 5;

            var trie = new Trie<bool>
                {
                    { "ABC", false }, { "AB", false }, { "ADE", false }, { "ABCDE", false }, { "X", false } 
                };

            Assert.IsFalse(((IDictionary<string, bool>)trie).Remove(new KeyValuePair<string, bool>("XY", true)));
            Assert.IsFalse(((IDictionary<string, bool>)trie).Remove(new KeyValuePair<string, bool>("ABCD", true)));
            Assert.IsFalse(((IDictionary<string, bool>)trie).Remove(new KeyValuePair<string, bool>("ABCDE", true)));
            Assert.AreEqual(initialCount, trie.Count);
            Assert.IsTrue(((IDictionary<string, bool>)trie).Remove(new KeyValuePair<string, bool>("ABCDE", false)));
            Assert.AreEqual(initialCount - 1, trie.Count);
            Assert.IsTrue(trie.Remove("X"));
            Assert.AreEqual(initialCount - 2, trie.Count);
            Assert.IsTrue(trie.Remove("ABC"));
            Assert.AreEqual(initialCount - 3, trie.Count);
            Assert.IsFalse(trie.ContainsKey("ABC"));
            Assert.IsTrue(trie.ContainsKey("AB"));
            Assert.IsTrue(trie.ContainsKey("ADE"));
        }

        [TestMethod]
        public void RemoveNotExists()
        {
            var trie = new Trie<bool> { { "ABC", false } };

            Assert.IsFalse(trie.Remove("A"));
            Assert.IsFalse(trie.Remove("X"));
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void RemoveNullKey()
        {
            // ReSharper disable once CollectionNeverQueried.Local
            var trie = new Trie<bool> { { "ABC", false } };

            // ReSharper disable once AssignNullToNotNullAttribute
            trie.Remove(null);
        }

        [TestMethod]
        public void TryGetValue()
        {
            const string expectedValue = "value";

            var trie = new Trie<string> { { "ABC", expectedValue } };

            string value;

            Assert.IsTrue(trie.TryGetValue("ABC", out value));
            Assert.AreEqual(expectedValue, value);
            Assert.IsFalse(trie.TryGetValue("A", out value));
            Assert.IsNull(value);
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void TryGetValueKeyIsNull()
        {
            var trie = new Trie<bool> { { "ABC", false } };

            bool value;
            // ReSharper disable once AssignNullToNotNullAttribute
            trie.TryGetValue(null, out value);
        }
    }
}