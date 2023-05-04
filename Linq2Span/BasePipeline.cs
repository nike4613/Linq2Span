using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Linq2Span
{
    public readonly struct BasePipeline<T> : ISpanPipeline<T, T>
    {
#pragma warning disable CS0618 // Type or member is obsolete
        // These members we use are specifically supposed to be used from this callsite only
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly bool ISpanPipeline<T, T>.MoveNext(ref SpanEnumeratorState<T> state, [MaybeNullWhen(false)] out T result)
            => state.MoveNext(out result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly bool ISpanPipeline<T, T>.TryGetCount(in SpanEnumeratorState<T> state, out int count)
        {
            count = state.Count;
            return true;
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
