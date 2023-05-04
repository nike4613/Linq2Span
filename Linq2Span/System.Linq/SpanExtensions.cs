using Linq2Span;
using System.Runtime.CompilerServices;

namespace System.Linq
{
    public static class SpanExtensions
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static PipelineEnumerable<T, BasePipeline<T>> AsPipeline<T>(this Span<T> span)
            => new(span, new());

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static PipelineEnumerable<T, BasePipeline<T>> AsPipeline<T>(this ReadOnlySpan<T> span)
            => new(span, new());

        // most of the combinators are in PipelineEnumerable.Combinators, because being instance methods helps immensely with generic type inference
    }
}
