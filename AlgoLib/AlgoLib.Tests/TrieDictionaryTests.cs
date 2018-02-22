using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlgoLib.Tests
{
    [TestClass]
    public class TrieDictionaryTests
    {
        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void AddWithSameKey()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new TrieDictionary<char, bool> { { "a", false }, { "a", true } };
        }

        [TestMethod]
        public void Clear()
        {
            var trie = new TrieDictionary<char, bool> { { "ABC", false }, { "AB", false }, { "ADE", false }, { "ABCDE", false } };

            trie.Clear();

            Assert.AreEqual(0, trie.Count);
        }

        [TestMethod]
        public void Contains()
        {
            var trie = new TrieDictionary<char, bool> { { "ABC", false }, { "AB", false }, { "ADE", true }, { "ABCDE", false } };

            var t = (IDictionary<IEnumerable<char>, bool>)trie;

            Assert.IsTrue(t.Contains(new KeyValuePair<IEnumerable<char>, bool>("ABC", false)));
            Assert.IsTrue(t.Contains(new KeyValuePair<IEnumerable<char>, bool>("AB", false)));
            Assert.IsTrue(t.Contains(new KeyValuePair<IEnumerable<char>, bool>("ADE", true)));
            Assert.IsTrue(t.Contains(new KeyValuePair<IEnumerable<char>, bool>("ABCDE", false)));

            Assert.IsFalse(t.Contains(new KeyValuePair<IEnumerable<char>, bool>("X", false)));
            Assert.IsFalse(t.Contains(new KeyValuePair<IEnumerable<char>, bool>("ADE", false)));
            Assert.IsFalse(t.Contains(new KeyValuePair<IEnumerable<char>, bool>("ABCD", false)));
        }

        [TestMethod]
        public void ContainsKey()
        {
            var trie = new TrieDictionary<char, bool> { { "ABC", false }, { "AB", false }, { "ADE", false }, { "ABCDE", false } };

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
            var trie = new TrieDictionary<char, bool> { { "ABC", false }, { "AB", false }, { "ADE", false }, { "ABCDE", false } };

            trie.Clear();

            Assert.IsFalse(trie.ContainsKey("ABC"));
            Assert.IsFalse(trie.ContainsKey("AB"));
            Assert.IsFalse(trie.ContainsKey("ADE"));
            Assert.IsFalse(trie.ContainsKey("ABCDE"));
        }

        [TestMethod]
        public void CopyTo()
        {
            var trie = new Trie<bool> { { "ABC", true }, { "AB", false }, { "ADE", true }, { "ABCDE", false } };


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
            var trie = new TrieDictionary<char, int> { { "ABC", 1 }, { "AB", 2 }, { "ABCDE", 3 } };
            ((IDictionary<IEnumerable<char>, int>)trie).Add(new KeyValuePair<IEnumerable<char>, int>("ADE", 4));


            var result = trie.GetByPrefix("ABC").ToArray();
            var keys = result.Select(i => new string(i.Key.ToArray()));
            var values = result.Select(i => i.Value);

            string[] expectedResultKeys = { "ABC", "ABCDE" };
            int[] expectedResultValues = { 1, 3 };

            Assert.AreEqual(4, trie.Count);
            Assert.IsTrue(expectedResultKeys.SequenceEqual(keys));
            Assert.IsTrue(expectedResultValues.SequenceEqual(values));
        }

        [TestMethod]
        public void GetEnumerator()
        {
            var trie = new TrieDictionary<char, bool> { { "ABC", false }, { "AB", false }, { "ADE", false }, { "ABCDE", false } };


            var result = trie.Select(kvp => new string(kvp.Key.ToArray())).OrderBy(w => w).ToArray();
            var resultEnumerator =
                trie.OfType<KeyValuePair<IEnumerable<char>, bool>>().Select(k => new string(k.Key.ToArray())).OrderBy(w => w).ToArray();

            CollectionAssert.AreEqual(new[] { "AB", "ABC", "ABCDE", "ADE" }, result);
            CollectionAssert.AreEqual(new[] { "AB", "ABC", "ABCDE", "ADE" }, resultEnumerator);
        }

        [TestMethod]
        public void IsReadOnly()
        {
            var trie = new TrieDictionary<char, bool>();

            Assert.IsFalse(((IDictionary<IEnumerable<char>, bool>)trie).IsReadOnly);
        }

        [TestMethod]
        public void ItemsGet()
        {
            var trie = new TrieDictionary<char, bool> { { "ABC", false }, { "AB", false }, { "ADE", true }, { "ABCDE", false } };

            Assert.AreEqual(false, trie["ABC"]);
            Assert.AreEqual(false, trie["AB"]);
            Assert.AreEqual(true, trie["ADE"]);
            Assert.AreEqual(false, trie["ABCDE"]);
        }

        [ExpectedException(typeof(KeyNotFoundException))]
        [TestMethod]
        public void ItemsGetException()
        {
            var trie = new TrieDictionary<char, bool> { { "ABC", false }, { "AB", false }, { "ADE", true }, { "ABCDE", false } };

            // ReSharper disable once UnusedVariable
            var result = trie["A"];
        }

        [TestMethod]
        public void ItemsSet()
        {
            var trie = new TrieDictionary<char, bool> { ["ABC"] = true };

            Assert.AreEqual(true, trie["ABC"]);

            trie["AB"] = true;

            Assert.AreEqual(true, trie["AB"]);

            trie["AB"] = false;

            Assert.AreEqual(false, trie["AB"]);
        }

        [TestMethod]
        public void KeysValues()
        {
            var trie = new TrieDictionary<char, bool> { { "ABC", false }, { "AB", true }, { "ADE", false }, { "ABCDE", true } };

            Assert.IsTrue(new[] {"AB", "ABC", "ABCDE", "ADE"}.SequenceEqual(trie.Keys.Select(i => new string(i.ToArray())).OrderBy(s => s)));
            Assert.IsTrue(new[] { false, false, true, true }.SequenceEqual(trie.Values.OrderBy(s => s)));
        }

        [TestMethod]
        public void Remove()
        {
            const int initialCount = 5;

            var trie = new TrieDictionary<char, bool>
                {
                    { "ABC", false }, { "AB", false }, { "ADE", false }, { "ABCDE", false }, { "X", false }
                };

            Assert.IsFalse(((IDictionary<IEnumerable<char>, bool>)trie).Remove(new KeyValuePair<IEnumerable<char>, bool>("XY", true)));
            Assert.IsFalse(((IDictionary<IEnumerable<char>, bool>)trie).Remove(new KeyValuePair<IEnumerable<char>, bool>("ABCD", true)));
            Assert.IsFalse(((IDictionary<IEnumerable<char>, bool>)trie).Remove(new KeyValuePair<IEnumerable<char>, bool>("ABCDE", true)));
            Assert.AreEqual(initialCount, trie.Count);
            Assert.IsTrue(((IDictionary<IEnumerable<char>, bool>)trie).Remove(new KeyValuePair<IEnumerable<char>, bool>("ABCDE", false)));
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
            var trie = new TrieDictionary<char, bool> { { "ABC", false } };

            Assert.IsFalse(trie.Remove("A"));
            Assert.IsFalse(trie.Remove("X"));
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void RemoveNullKey()
        {
            // ReSharper disable once CollectionNeverQueried.Local
            var trie = new TrieDictionary<char, bool> { { "ABC", false } };

            // ReSharper disable once AssignNullToNotNullAttribute
            trie.Remove(null);
        }

        [TestMethod]
        public void TryGetValue()
        {
            const string expectedValue = "value";

            var trie = new TrieDictionary<char, string> {{"ABC", expectedValue}};

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
            var trie = new TrieDictionary<char, bool> { { "ABC", false } };

            bool value;
            // ReSharper disable once AssignNullToNotNullAttribute
            trie.TryGetValue(null, out value);
        }
    }
}