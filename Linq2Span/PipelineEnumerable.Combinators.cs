using System;
using System.Runtime.CompilerServices;

namespace Linq2Span
{
    public readonly ref partial struct PipelineEnumerable<TResult, TPipeline, TSpan>
    {

        /****************** SELECT ******************/

        [MethodImpl(MethodImplOptions.NoInlining)]
        public PipelineEnumerable<TOut, SelectPipeline<TTransform, TResult, TOut, TSpan, TPipeline>, TSpan>
            Select<TTransform, TOut>(TTransform transform)
            where TTransform : IStructFunc<TResult, TOut>
            => new(Span, new(transform, Pipeline));

        [MethodImpl(MethodImplOptions.NoInlining)]
        public PipelineEnumerable<TOut, SelectPipeline<DelegateStructFunc<TResult, TOut>, TResult, TOut, TSpan, TPipeline>, TSpan>
            Select<TOut>(Func<TResult, TOut> transform)
            => Select<DelegateStructFunc<TResult, TOut>, TOut>(new(transform));

        // same-type select

        [MethodImpl(MethodImplOptions.NoInlining)]
        public PipelineEnumerable<TResult, SelectPipelineSameType<TTransform, TResult, TSpan, TPipeline>, TSpan>
            Select<TTransform>(TTransform transform)
            where TTransform : IStructFunc<TResult, TResult>
            => new(Span, new(transform, Pipeline));

        [MethodImpl(MethodImplOptions.NoInlining)]
        public PipelineEnumerable<TResult, SelectPipelineSameType<DelegateStructFuncSameType<TResult>, TResult, TSpan, TPipeline>, TSpan>
            Select(Func<TResult, TResult> transform)
            => Select(new DelegateStructFuncSameType<TResult>(transform));

        /****************** WHERE ******************/

        [MethodImpl(MethodImplOptions.NoInlining)]
        public PipelineEnumerable<TResult, WherePipeline<TFilter, TResult, TSpan, TPipeline>, TSpan>
            Where<TFilter>(TFilter filter)
            where TFilter : IStructFunc<TResult, bool>
            => new(Span, new(filter, Pipeline));

        [MethodImpl(MethodImplOptions.NoInlining)]
        public PipelineEnumerable<TResult, WherePipeline<DelegateStructPredicate<TResult>, TResult, TSpan, TPipeline>, TSpan>
            Where(Func<TResult, bool> filter)
            => Where<DelegateStructPredicate<TResult>>(new(filter));
    }

    // same-span-type pipeline
    public readonly ref partial struct PipelineEnumerable<TResult, TPipeline>
    {
        /****************** SELECT ******************/

        // in-span-type
        [MethodImpl(MethodImplOptions.NoInlining)]
        public PipelineEnumerable<TOut, SelectPipelineInSpanType<TTransform, TOut, TResult, TPipeline>, TResult>
            Select<TTransform, TOut>(TTransform transform)
            where TTransform : IStructFunc<TResult, TOut>
            => new(Span, new(transform, Pipeline));

        [MethodImpl(MethodImplOptions.NoInlining)]
        public PipelineEnumerable<TOut, SelectPipelineInSpanType<DelegateStructFunc<TResult, TOut>, TOut, TResult, TPipeline>, TResult>
            Select<TOut>(Func<TResult, TOut> transform)
            => Select<DelegateStructFunc<TResult, TOut>, TOut>(new(transform));

        // same-span-type
        [MethodImpl(MethodImplOptions.NoInlining)]
        public PipelineEnumerable<TResult, SelectPipelineSameSpanType<TTransform, TResult, TPipeline>>
            Select<TTransform>(TTransform transform)
            where TTransform : IStructFunc<TResult, TResult>
            => new(Span, new(transform, Pipeline));

        [MethodImpl(MethodImplOptions.NoInlining)]
        public PipelineEnumerable<TResult, SelectPipelineSameSpanType<DelegateStructFuncSameType<TResult>, TResult, TPipeline>>
            Select(Func<TResult, TResult> transform)
            => Select(new DelegateStructFuncSameType<TResult>(transform));

        /****************** WHERE ******************/

        [MethodImpl(MethodImplOptions.NoInlining)]
        public PipelineEnumerable<TResult, WherePipelineSpanType<TFilter, TResult, TPipeline>>
            Where<TFilter>(TFilter filter)
            where TFilter : IStructFunc<TResult, bool>
            => new(Span, new(filter, Pipeline));

        [MethodImpl(MethodImplOptions.NoInlining)]
        public PipelineEnumerable<TResult, WherePipelineSpanType<DelegateStructPredicate<TResult>, TResult, TPipeline>>
            Where(Func<TResult, bool> filter)
            => Where<DelegateStructPredicate<TResult>>(new(filter));
    }
}
