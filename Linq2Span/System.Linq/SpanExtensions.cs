using Linq2Span;
using System.ComponentModel;

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

        #region Other Combinators
        // These are combinators which can be implemented for a span without the general pipeline fluff.
        // Here, we also want to take the opportunity to optimize what we can, where we can.

        /****************** SKIP ******************/

        public static Span<T> Skip<T>(this Span<T> span, int amount) => unchecked((uint)amount >= span.Length) ? default : span.Slice(amount);
        public static ReadOnlySpan<T> Skip<T>(this ReadOnlySpan<T> span, int amount) => unchecked((uint)amount >= span.Length) ? default : span.Slice(amount);

        public static PipelineEnumerable<TResult, SkipPipelineSpanType<TResult, TPipeline>>
            Skip<TResult, TPipeline>(this PipelineEnumerable<TResult, TPipeline> pipe, int amount)
            where TPipeline : ISpanPipeline<TResult, TResult>
            => new(pipe.Span, new(amount, pipe.Pipeline));

        public static PipelineEnumerable<TResult, SkipPipeline<TResult, TPipeline, TSpan>, TSpan>
            Skip<TResult, TPipeline, TSpan>(this PipelineEnumerable<TResult, TPipeline, TSpan> pipe, int amount)
            where TPipeline : ISpanPipeline<TSpan, TResult>
            => new(pipe.Span, new(amount, pipe.Pipeline));

        public static PipelineEnumerable<TResult, SkipPipelineSpanType<TResult, TPipeline>>
            Skip<TResult, TPipeline>(this PipelineEnumerable<TResult, TPipeline, TResult> pipe, int amount)
            where TPipeline : ISpanPipeline<TResult, TResult>
            => new(pipe.Span, new(amount, pipe.Pipeline));

        // optimize for when the Skip is directly wrapping a BasePipeline
        public static PipelineEnumerable<TResult, BasePipeline<TResult>>
            Skip<TResult>(this PipelineEnumerable<TResult, BasePipeline<TResult>> pipe, int amount)
            => new(pipe.Span.Skip(amount), pipe.Pipeline);

        public static PipelineEnumerable<TResult, BasePipeline<TResult>>
            Skip<TResult>(this PipelineEnumerable<TResult, BasePipeline<TResult>, TResult> pipe, int amount)
            => new(pipe.Span.Skip(amount), pipe.Pipeline);

        /****************** TAKE ******************/

        public static Span<T> Take<T>(this Span<T> span, int amount) => span.Slice(0, Math.Min(span.Length, amount));
        public static ReadOnlySpan<T> Take<T>(this ReadOnlySpan<T> span, int amount) => span.Slice(0, Math.Min(span.Length, amount));

        public static PipelineEnumerable<TResult, TakePipelineSpanType<TResult, TPipeline>>
            Take<TResult, TPipeline>(this PipelineEnumerable<TResult, TPipeline> pipe, int amount)
            where TPipeline : ISpanPipeline<TResult, TResult>
            => new(pipe.Span, new(amount, pipe.Pipeline));

        public static PipelineEnumerable<TResult, TakePipeline<TResult, TPipeline, TSpan>, TSpan>
            Take<TResult, TPipeline, TSpan>(this PipelineEnumerable<TResult, TPipeline, TSpan> pipe, int amount)
            where TPipeline : ISpanPipeline<TSpan, TResult>
            => new(pipe.Span, new(amount, pipe.Pipeline));

        public static PipelineEnumerable<TResult, TakePipelineSpanType<TResult, TPipeline>>
            Take<TResult, TPipeline>(this PipelineEnumerable<TResult, TPipeline, TResult> pipe, int amount)
            where TPipeline : ISpanPipeline<TResult, TResult>
            => new(pipe.Span, new(amount, pipe.Pipeline));

        // optimize for when the Take is directly wrapping a BasePipeline
        public static PipelineEnumerable<TResult, BasePipeline<TResult>>
            Take<TResult>(this PipelineEnumerable<TResult, BasePipeline<TResult>> pipe, int amount)
            => new(pipe.Span.Take(amount), pipe.Pipeline);

        public static PipelineEnumerable<TResult, BasePipeline<TResult>>
            Take<TResult>(this PipelineEnumerable<TResult, BasePipeline<TResult>, TResult> pipe, int amount)
            => new(pipe.Span.Take(amount), pipe.Pipeline);

        #endregion

        // To simplifies the type, if possible
        public static PipelineEnumerable<TResult, TPipeline> To<TResult, TPipeline>(this PipelineEnumerable<TResult, TPipeline, TResult> pipe)
            where TPipeline : ISpanPipeline<TResult, TResult>
            => new(pipe.Span, pipe.Pipeline);
        // identity versions of To
        public static PipelineEnumerable<TResult, TPipeline> To<TResult, TPipeline>(this PipelineEnumerable<TResult, TPipeline> pipe)
            where TPipeline : ISpanPipeline<TResult, TResult>
            => pipe;
    }
}
