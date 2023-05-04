using Linq2Span;
using System.Runtime.CompilerServices;

namespace System.Linq
{
    public static class SpanExtensions
    {
        // note: all of these methods are marked NoInlining because if they aren't they use up all the inlining budget, and so parts of the iteration don't get inlined

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static PipelineEnumerable<T, BasePipeline<T>, T> AsPipeline<T>(this Span<T> span)
            => new(span, new());
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static PipelineEnumerable<T, BasePipeline<T>, T> AsPipeline<T>(this ReadOnlySpan<T> span)
            => new(span, new());

        // StructFunc
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static PipelineEnumerable<TOut, SelectPipeline<TTransform, TIn, TOut, TSpan, TPipeline>, TSpan>
            Select<TTransform, TIn, TOut, TSpan, TPipeline>(this PipelineEnumerable<TIn, TPipeline, TSpan> source, TTransform transform)
            where TPipeline : ISpanPipeline<TSpan, TIn>
            where TTransform : IStructFunc<TIn, TOut>
            => new(source.Span, new(transform, source.Pipeline));

        // DelegateStructFunc
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static PipelineEnumerable<TOut, SelectPipeline<DelegateStructFunc<TIn, TOut>, TIn, TOut, TSpan, TPipeline>, TSpan>
            Select<TIn, TOut, TSpan, TPipeline>(this PipelineEnumerable<TIn, TPipeline, TSpan> source, Func<TIn, TOut> transform)
            where TPipeline : ISpanPipeline<TSpan, TIn>
            => Select<DelegateStructFunc<TIn, TOut>, TIn, TOut, TSpan, TPipeline>(source, new(transform));

        // same-type select
        // StructFunc
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static PipelineEnumerable<TResult, SelectPipelineSameType<TTransform, TResult, TSpan, TPipeline>, TSpan>
            Select<TTransform, TResult, TSpan, TPipeline>(this PipelineEnumerable<TResult, TPipeline, TSpan> source, TTransform transform)
            where TPipeline : ISpanPipeline<TSpan, TResult>
            where TTransform : IStructFunc<TResult, TResult>
            => new(source.Span, new(transform, source.Pipeline));

        // DelegateStructFunc
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static PipelineEnumerable<TResult, SelectPipelineSameType<DelegateStructFuncSameType<TResult>, TResult, TSpan, TPipeline>, TSpan>
            Select<TResult, TSpan, TPipeline>(this PipelineEnumerable<TResult, TPipeline, TSpan> source, Func<TResult, TResult> transform)
            where TPipeline : ISpanPipeline<TSpan, TResult>
            => Select<DelegateStructFuncSameType<TResult>, TResult, TSpan, TPipeline>(source, new(transform));

        // StructFunc
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static PipelineEnumerable<TResult, WherePipeline<TFilter, TResult, TSpan, TPipeline>, TSpan>
            Where<TFilter, TResult, TSpan, TPipeline>(this PipelineEnumerable<TResult, TPipeline, TSpan> source, TFilter filter)
            where TPipeline : ISpanPipeline<TSpan, TResult>
            where TFilter : IStructFunc<TResult, bool>
            => new(source.Span, new(filter, source.Pipeline));

        // DelegateStructFunc
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static PipelineEnumerable<TResult, WherePipeline<DelegateStructPredicate<TResult>, TResult, TSpan, TPipeline>, TSpan>
            Where<TResult, TSpan, TPipeline>(this PipelineEnumerable<TResult, TPipeline, TSpan> source, Func<TResult, bool> filter)
            where TPipeline : ISpanPipeline<TSpan, TResult>
            => Where<DelegateStructPredicate<TResult>, TResult, TSpan, TPipeline>(source, new(filter));
    }
}
