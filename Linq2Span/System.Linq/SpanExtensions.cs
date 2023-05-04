using Linq2Span;

namespace System.Linq
{
    public static class SpanExtensions
    {
        public static PipelineEnumerable<T, BasePipeline<T>, T> AsPipeline<T>(this Span<T> span)
            => new(span, new());
        public static PipelineEnumerable<T, BasePipeline<T>, T> AsPipeline<T>(this ReadOnlySpan<T> span)
            => new(span, new());

        // StructFunc
        public static PipelineEnumerable<TOut, SelectPipeline<TTransform, TIn, TOut, TSpan, TPipeline>, TSpan>
            Select<TTransform, TIn, TOut, TSpan, TPipeline>(this PipelineEnumerable<TIn, TPipeline, TSpan> source, TTransform transform)
            where TPipeline : ISpanPipeline<TSpan, TIn>
            where TTransform : IStructFunc<TIn, TOut>
            => new(source.Span, new(transform, source.Pipeline));

        // DelegateStructFunc
        public static PipelineEnumerable<TOut, SelectPipeline<DelegateStructFunc<TIn, TOut>, TIn, TOut, TSpan, TPipeline>, TSpan>
            Select<TIn, TOut, TSpan, TPipeline>(this PipelineEnumerable<TIn, TPipeline, TSpan> source, Func<TIn, TOut> transform)
            where TPipeline : ISpanPipeline<TSpan, TIn>
            => Select<DelegateStructFunc<TIn, TOut>, TIn, TOut, TSpan, TPipeline>(source, new(transform));

        // StructFunc
        public static PipelineEnumerable<TResult, WherePipeline<TResult, TPipeline, TSpan, TFilter>, TSpan>
            Where<TFilter, TResult, TSpan, TPipeline>(this PipelineEnumerable<TResult, TPipeline, TSpan> source, TFilter filter)
            where TPipeline : ISpanPipeline<TSpan, TResult>
            where TFilter : IStructFunc<TResult, bool>
            => new(source.Span, new(filter, source.Pipeline));

        // DelegateStructFunc
        public static PipelineEnumerable<TResult, WherePipeline<TResult, TPipeline, TSpan, DelegateStructPredicate<TResult>>, TSpan>
            Where<TResult, TSpan, TPipeline>(this PipelineEnumerable<TResult, TPipeline, TSpan> source, Func<TResult, bool> filter)
            where TPipeline : ISpanPipeline<TSpan, TResult>
            => Where<DelegateStructPredicate<TResult>, TResult, TSpan, TPipeline>(source, new(filter));
    }
}
