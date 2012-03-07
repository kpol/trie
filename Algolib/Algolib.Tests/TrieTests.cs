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
            var trie = new Trie<bool>();

            trie.Add(null, false);
        }

        [TestMethod]
        public void AddRange()
        {
            const int Count = 10;

            var trie = new Trie<bool>();

            trie.AddRange(Enumerable.Range(0, Count).Select(i => new TrieEntry<bool>(i.ToString(), false)));

            Assert.AreEqual(Count, trie.Count);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void AddWithSameKey()
        {
            var trie = new Trie<bool>();

            trie.Add("a", false);
            trie.Add("a", true);
        }

        [TestMethod]
        public void Clear()
        {
            var trie = new Trie<bool>();

            trie.Add("ABC", false);
            trie.Add("AB", false);
            trie.Add("ADE", false);
            trie.Add("ABCDE", false);

            trie.Clear();

            Assert.AreEqual(0, trie.Count);
        }

        [TestMethod]
        public void Contains()
        {
            var trie = new Trie<bool>();

            trie.Add("ABC", false);
            trie.Add("AB", false);
            trie.Add("ADE", true);
            trie.Add("ABCDE", false);

            var t = trie as IDictionary<string, bool>;

            Assert.IsTrue(t.Contains(new KeyValuePair<string, bool>("ABC", false)));
            Assert.IsTrue(t.Contains(new KeyValuePair<string, bool>("AB", false)));
            Assert.IsTrue(t.Contains(new KeyValuePair<string, bool>("ADE", true)));
            Assert.IsTrue(t.Contains(new KeyValuePair<string, bool>("ABCDE", false)));

            Assert.IsFalse(t.Contains(new KeyValuePair<string, bool>("ADE", false)));
        }

        [TestMethod]
        public void ContainsKey()
        {
            var trie = new Trie<bool>();

            trie.Add("ABC", false);
            trie.Add("AB", false);
            trie.Add("ADE", false);
            trie.Add("ABCDE", false);

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
            var trie = new Trie<bool>();

            trie.Add("ABC", false);
            trie.Add("AB", false);
            trie.Add("ADE", false);
            trie.Add("ABCDE", false);

            trie.Clear();

            Assert.IsFalse(trie.ContainsKey("ABC"));
            Assert.IsFalse(trie.ContainsKey("AB"));
            Assert.IsFalse(trie.ContainsKey("ADE"));
            Assert.IsFalse(trie.ContainsKey("ABCDE"));
        }

        [TestMethod]
        public void GetByPrefix()
        {
            var trie = new Trie<bool>();

            trie.Add("ABC", false);
            trie.Add("AB", false);
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
            var trie = new Trie<bool>();

            trie.Add("ABC", false);
            trie.Add("AB", false);
            trie.Add("ADE", false);
            trie.Add("ABCDE", false);

            var result = trie.Select(kvp => kvp.Key).OrderBy(w => w).ToArray();

            CollectionAssert.AreEqual(new[] { "AB", "ABC", "ABCDE", "ADE" }, result);
        }

        [TestMethod]
        public void ItemsGet()
        {
            var trie = new Trie<bool>();

            trie.Add("ABC", false);
            trie.Add("AB", false);
            trie.Add("ADE", true);
            trie.Add("ABCDE", false);

            Assert.AreEqual(false, trie["ABC"]);
            Assert.AreEqual(false, trie["AB"]);
            Assert.AreEqual(true, trie["ADE"]);
            Assert.AreEqual(false, trie["ABCDE"]);
        }

        [ExpectedException(typeof(KeyNotFoundException))]
        [TestMethod]
        public void ItemsGetException()
        {
            var trie = new Trie<bool>();

            trie.Add("ABC", false);
            trie.Add("AB", false);
            trie.Add("ADE", true);
            trie.Add("ABCDE", false);

            var result = trie["A"];
        }

        [TestMethod]
        public void ItemsSet()
        {
            var trie = new Trie<bool>();

            trie["ABC"] = true;

            Assert.AreEqual(true, trie["ABC"]);
        }

        [TestMethod]
        public void Remove()
        {
            var trie = new Trie<bool>();

            trie.Add("ABC", false);
            trie.Add("AB", false);
            trie.Add("ADE", false);
            trie.Add("ABCDE", false);

            Assert.IsTrue(trie.Remove("ABCDE"));
            Assert.AreEqual(3, trie.Count);
            Assert.IsTrue(trie.ContainsKey("ABC"));
            Assert.IsTrue(trie.ContainsKey("AB"));
            Assert.IsTrue(trie.ContainsKey("ADE"));
        }

        [TestMethod]
        public void RemoveNotExists()
        {
            var trie = new Trie<bool>();

            trie.Add("ABC", false);

            Assert.IsFalse(trie.Remove("A"));
            Assert.IsFalse(trie.Remove("X"));
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void RemoveNullKey()
        {
            var trie = new Trie<bool>();

            trie.Add("ABC", false);
            trie.Remove(null);
        }
    }
}