﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Linq2Span
{
    [StructLayout(LayoutKind.Auto)]
    public struct WherePipeline<TFilter, TResult, TSpan, TPipeline> : ISpanPipeline<TSpan, TResult>
        where TPipeline : ISpanPipeline<TSpan, TResult>
        where TFilter : IStructFunc<TResult, bool>
    {
        private TPipeline pipeline; // pipeline may be mutable
        private TFilter filter; // filter may be mutable

        public WherePipeline(TFilter filter, TPipeline pipeline)
        {
            this.pipeline = pipeline;
            this.filter = filter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool ISpanPipeline<TSpan, TResult>.MoveNext(ref SpanEnumeratorState<TSpan> state, out TResult result)
        {
            while (pipeline.MoveNext(ref state, out var value))
            {
                if (filter.Invoke(value))
                {
                    result = value;
                    return true;
                }
            }

            Unsafe.SkipInit(out result);
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly bool ISpanPipeline<TSpan, TResult>.TryGetCount(in SpanEnumeratorState<TSpan> state, out int count)
        {
            // we can never know the count of a Where pipeline without enumerating
            Unsafe.SkipInit(out count);
            return false;
        }
    }

    [StructLayout(LayoutKind.Auto)]
    public struct WherePipelineSpanType<TFilter, TSpan, TPipeline> : ISpanPipeline<TSpan, TSpan>
        where TPipeline : ISpanPipeline<TSpan, TSpan>
        where TFilter : IStructFunc<TSpan, bool>
    {
        private TPipeline pipeline; // pipeline may be mutable
        private TFilter filter; // filter may be mutable

        public WherePipelineSpanType(TFilter filter, TPipeline pipeline)
        {
            this.pipeline = pipeline;
            this.filter = filter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool ISpanPipeline<TSpan, TSpan>.MoveNext(ref SpanEnumeratorState<TSpan> state, out TSpan result)
        {
            while (pipeline.MoveNext(ref state, out var value))
            {
                if (filter.Invoke(value))
                {
                    result = value;
                    return true;
                }
            }

            Unsafe.SkipInit(out result);
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly bool ISpanPipeline<TSpan, TSpan>.TryGetCount(in SpanEnumeratorState<TSpan> state, out int count)
        {
            // we can never know the count of a Where pipeline without enumerating
            Unsafe.SkipInit(out count);
            return false;
        }
    }
}
