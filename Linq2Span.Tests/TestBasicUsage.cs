using System;
using System.Linq;
using Xunit;

namespace Linq2Span.Tests
{
    public class TestBasicUsage
    {
        [Fact]
        public void BasicUsage()
        {
            Span<int> span = stackalloc int[15]
            {
                1, 2, 3, 5, 6, 7, 8, 8, 9, 10, 11, 12, 13, 14, 15
            };

            var enumerable = span
                .AsPipeline()
                .Select(new Minus1())
                .Where(new IsEven())
                .Select(new Div2());

            foreach (var element in enumerable)
            {
                Console.WriteLine(element);
            }
        }

        private readonly struct Minus1 : IStructFunc<int, int>
        {
            public readonly int Invoke(int arg0) => arg0 - 1;
        }
        private readonly struct IsEven : IStructFunc<int, bool>
        {
            public readonly bool Invoke(int arg0) => (arg0 % 2) == 0;
        }
        private readonly struct Div2 : IStructFunc<int, int>
        {
            public readonly int Invoke(int arg0) => arg0 / 2;
        }
    }
}
