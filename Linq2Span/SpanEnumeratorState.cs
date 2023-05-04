using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Linq2Span
{
    public ref struct SpanEnumeratorState<T>
    {
        internal readonly ReadOnlySpan<T> Span;
        private int curIdx;

        internal SpanEnumeratorState(ReadOnlySpan<T> span)
        {
            Span = span;
            curIdx = 0;
        }

        [Obsolete("This member must only be used by BasePipelineElement")]
        internal readonly int Count => Span.Length;

        [Obsolete("This member must only be used by BasePipelineElement")]
        internal bool MoveNext([MaybeNullWhen(false)] out T value)
        {
            if (unchecked((uint)curIdx < Span.Length))
            {
                value = Span[curIdx++];
                return true;
            }
            else
            {
                Unsafe.SkipInit(out value);
                return false;
            }
        }
    }
}
