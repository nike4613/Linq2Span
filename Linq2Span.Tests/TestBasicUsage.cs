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
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15
            };

            DoEnumerate(span);
            DoEnumerateLinq(span);
            ToArray1(span);
            ToArray2(span);
            ToArray3(span);
        }

        private static void DoEnumerate(ReadOnlySpan<int> span)
        {
            var enumerable = span
                .AsPipeline()
                .Select(new Minus1())
                .Where(new IsEven())
                .Select(new Div2())
                .Select<AsByte, byte>(new AsByte())
                .Select(new Minus1())
                .Select<AsInt, int>(new AsInt());

            foreach (var element in enumerable)
            {
                Console.WriteLine(element);
            }
        }

        private static void DoEnumerateLinq(ReadOnlySpan<int> span)
        {
            var e2 =
                from i in span.AsPipeline()
                where i > 5
                select i + 1;

            foreach (var element in e2)
            {
                Console.WriteLine(element);
            }
        }

        private static void ToArray1(ReadOnlySpan<int> span)
        {
            var a1 = span.AsPipeline().ToArray();
        }
        private static void ToArray2(ReadOnlySpan<int> span)
        {
            var a2 = span.AsPipeline().Select(new Minus1()).ToArray();
        }
        private static void ToArray3(ReadOnlySpan<int> span)
        {
            var a3 = span.AsPipeline().Where(new IsEven()).ToArray();
        }

        private readonly struct Minus1 : IStructFunc<int, int>, IStructFunc<byte, byte>
        {
            public readonly int Invoke(int arg0) => arg0 - 1;
            public readonly byte Invoke(byte arg0) => unchecked((byte)(arg0 - 1));
        }
        private readonly struct IsEven : IStructFunc<int, bool>
        {
            public readonly bool Invoke(int arg0) => (arg0 % 2) == 0;
        }
        private readonly struct Div2 : IStructFunc<int, int>
        {
            public readonly int Invoke(int arg0) => arg0 / 2;
        }
        private readonly struct AsByte : IStructFunc<int, byte>
        {
            public readonly byte Invoke(int arg0) => (byte)arg0;
        }
        private readonly struct AsInt : IStructFunc<byte, int>
        {
            public readonly int Invoke(byte arg0) => arg0;
        }
    }
}
