using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Linq2Span
{
    [StructLayout(LayoutKind.Auto)]
    public struct TakePipeline<TResult, TPipeline, TSpan> : ISpanPipeline<TSpan, TResult>
        where TPipeline : ISpanPipeline<TSpan, TResult>
    {
        internal TPipeline Pipeline; // pipeline may be mutable
        internal int Amount;

        public TakePipeline(int amount, TPipeline pipeline)
        {
            Amount = amount;
            Pipeline = pipeline;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool ISpanPipeline<TSpan, TResult>.MoveNext(ref SpanEnumeratorState<TSpan> state, [MaybeNullWhen(false)] out TResult result)
        {
            if (Amount > 0)
            {
                Amount--;
                return Pipeline.MoveNext(ref state, out result);
            }
            else
            {
                Unsafe.SkipInit(out result);
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly bool ISpanPipeline<TSpan, TResult>.TryGetCount(in SpanEnumeratorState<TSpan> state, out int count)
        {
            if (Pipeline.TryGetCount(in state, out var cnt))
            {
                count = Math.Min(cnt, Amount);
                return true;
            }
            else
            {
                Unsafe.SkipInit(out count);
                return false;
            }
        }
    }

    [StructLayout(LayoutKind.Auto)]
    public struct TakePipelineSpanType<TResult, TPipeline> : ISpanPipeline<TResult, TResult>
        where TPipeline : ISpanPipeline<TResult, TResult>
    {
        internal TPipeline Pipeline; // pipeline may be mutable
        internal int Amount;

        public TakePipelineSpanType(int amount, TPipeline pipeline)
        {
            Amount = amount;
            Pipeline = pipeline;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool ISpanPipeline<TResult, TResult>.MoveNext(ref SpanEnumeratorState<TResult> state, [MaybeNullWhen(false)] out TResult result)
        {
            if (Amount > 0)
            {
                Amount--;
                return Pipeline.MoveNext(ref state, out result);
            }
            else
            {
                Unsafe.SkipInit(out result);
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly bool ISpanPipeline<TResult, TResult>.TryGetCount(in SpanEnumeratorState<TResult> state, out int count)
        {
            if (Pipeline.TryGetCount(in state, out var cnt))
            {
                count = Math.Min(cnt, Amount);
                return true;
            }
            else
            {
                Unsafe.SkipInit(out count);
                return false;
            }
        }
    }
}
