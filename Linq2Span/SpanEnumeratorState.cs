using System;

namespace Linq2Span
{
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
