using System;
using System.Runtime.InteropServices;

namespace Linq2Span
{
    [StructLayout(LayoutKind.Auto)]
    public ref struct SpanEnumeratorState<T>
    {
        internal readonly ReadOnlySpan<T> Span;
        internal int Index;

        internal SpanEnumeratorState(ReadOnlySpan<T> span)
        {
            Span = span;
            Index = 0;
        }
    }
}
