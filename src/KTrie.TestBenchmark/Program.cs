using System;
using BenchmarkDotNet.Running;

namespace KTrie.TestBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var _ = BenchmarkRunner.Run<TrieTest>();
        }
    }
}