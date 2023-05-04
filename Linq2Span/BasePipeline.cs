using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Linq2Span
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct BasePipeline<T> : ISpanPipeline<T, T>
    {
        // These members we use are specifically supposed to be used from this callsite only
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly bool ISpanPipeline<T, T>.MoveNext(ref SpanEnumeratorState<T> state, [MaybeNullWhen(false)] out T result)
        {
            if (unchecked((uint)state.Index < state.Span.Length))
            {
                result = state.Span[state.Index++];
                return true;
            }
            else
            {
                Unsafe.SkipInit(out result);
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly bool ISpanPipeline<T, T>.TryGetCount(in SpanEnumeratorState<T> state, out int count)
        {
            count = state.Span.Length;
            return true;
        }
    }
}
