using System.Diagnostics.CodeAnalysis;

namespace Linq2Span
{
    public interface ISpanPipeline<TSpan, TResult>
    {
        bool TryGetCount(in SpanEnumeratorState<TSpan> state, out int count);
        bool MoveNext(ref SpanEnumeratorState<TSpan> state, [MaybeNullWhen(false)] out TResult result);
    }
}
