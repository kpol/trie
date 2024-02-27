using System;
using System.Collections.Generic;
using BenchmarkDotNet.Running;

namespace KTrie.TestBenchmark;

class Program
{
    static void Main(string[] args)
    {
        var _ = BenchmarkRunner.Run<StringTrieTest>();
        
        //var x = new StringTrieTest();

        //var r1 = x.Linq_StartsWith();
        //var r4 = x.Trie_GetByPrefixList();


        //Console.WriteLine(r1.Count);
        //Console.WriteLine(r4.Count);
    }
}