using Linq2Span;

namespace System.Linq
{
    public static class SpanExtensions
    {
        public static PipelineEnumerable<T, BasePipeline<T>> AsPipeline<T>(this Span<T> span)
            => new(span, new());

        public static PipelineEnumerable<T, BasePipeline<T>> AsPipeline<T>(this ReadOnlySpan<T> span)
            => new(span, new());

        // most of the combinators are in PipelineEnumerable.Combinators, because being instance methods helps immensely with generic type inference

        #region Entry Combinators
        #region ReadOnlySpan
        /****************** SELECT ******************/

        // same-span-type
        public static PipelineEnumerable<TResult, SelectPipelineSameSpanType<TTransform, TResult, BasePipeline<TResult>>>
            Select<TTransform, TResult>(this ReadOnlySpan<TResult> span, TTransform transform)
            where TTransform : IStructFunc<TResult, TResult>
            => span.AsPipeline().Select(transform);

        public static PipelineEnumerable<TResult, SelectPipelineSameSpanType<DelegateStructFuncSameType<TResult>, TResult, BasePipeline<TResult>>>
            Select<TResult>(this ReadOnlySpan<TResult> span, Func<TResult, TResult> transform)
            => span.AsPipeline().Select(transform);

        // in-span-type
        public static PipelineEnumerable<TOut, SelectPipelineInSpanType<TTransform, TOut, TSpan, BasePipeline<TSpan>>, TSpan>
            Select<TTransform, TOut, TSpan>(this ReadOnlySpan<TSpan> span, TTransform transform)
            where TTransform : IStructFunc<TSpan, TOut>
            => span.AsPipeline().Select<TTransform, TOut>(transform);

        public static PipelineEnumerable<TOut, SelectPipelineInSpanType<DelegateStructFunc<TSpan, TOut>, TOut, TSpan, BasePipeline<TSpan>>, TSpan>
            Select<TOut, TSpan>(this ReadOnlySpan<TSpan> span, Func<TSpan, TOut> transform)
            => span.AsPipeline().Select<TOut>(transform);

        /****************** WHERE ******************/

        public static PipelineEnumerable<TResult, WherePipelineSpanType<TFilter, TResult, BasePipeline<TResult>>>
            Where<TFilter, TResult>(this ReadOnlySpan<TResult> span, TFilter filter)
            where TFilter : IStructFunc<TResult, bool>
            => span.AsPipeline().Where(filter);

        public static PipelineEnumerable<TResult, WherePipelineSpanType<DelegateStructPredicate<TResult>, TResult, BasePipeline<TResult>>>
            Where<TResult>(this ReadOnlySpan<TResult> span, Func<TResult, bool> filter)
            => span.AsPipeline().Where(filter);

        #endregion
        #region Span
        /****************** SELECT ******************/

        // same-span-type
        public static PipelineEnumerable<TResult, SelectPipelineSameSpanType<TTransform, TResult, BasePipeline<TResult>>>
            Select<TTransform, TResult>(this Span<TResult> span, TTransform transform)
            where TTransform : IStructFunc<TResult, TResult>
            => span.AsPipeline().Select(transform);

        public static PipelineEnumerable<TResult, SelectPipelineSameSpanType<DelegateStructFuncSameType<TResult>, TResult, BasePipeline<TResult>>>
            Select<TResult>(this Span<TResult> span, Func<TResult, TResult> transform)
            => span.AsPipeline().Select(transform);

        // in-span-type
        public static PipelineEnumerable<TOut, SelectPipelineInSpanType<TTransform, TOut, TSpan, BasePipeline<TSpan>>, TSpan>
            Select<TTransform, TOut, TSpan>(this Span<TSpan> span, TTransform transform)
            where TTransform : IStructFunc<TSpan, TOut>
            => span.AsPipeline().Select<TTransform, TOut>(transform);

        public static PipelineEnumerable<TOut, SelectPipelineInSpanType<DelegateStructFunc<TSpan, TOut>, TOut, TSpan, BasePipeline<TSpan>>, TSpan>
            Select<TOut, TSpan>(this Span<TSpan> span, Func<TSpan, TOut> transform)
            => span.AsPipeline().Select<TOut>(transform);

        /****************** WHERE ******************/

        public static PipelineEnumerable<TResult, WherePipelineSpanType<TFilter, TResult, BasePipeline<TResult>>>
            Where<TFilter, TResult>(this Span<TResult> span, TFilter filter)
            where TFilter : IStructFunc<TResult, bool>
            => span.AsPipeline().Where(filter);

        public static PipelineEnumerable<TResult, WherePipelineSpanType<DelegateStructPredicate<TResult>, TResult, BasePipeline<TResult>>>
            Where<TResult>(this Span<TResult> span, Func<TResult, bool> filter)
            => span.AsPipeline().Where(filter);

        #endregion
        #endregion
    }
}
