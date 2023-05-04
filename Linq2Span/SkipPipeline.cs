using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Linq2Span
{
    [StructLayout(LayoutKind.Auto)]
    public struct SkipPipeline<TResult, TPipeline, TSpan> : ISpanPipeline<TSpan, TResult>
        where TPipeline : ISpanPipeline<TSpan, TResult>
    {
        internal TPipeline Pipeline; // pipeline may be mutable
        internal int Amount;

        public SkipPipeline(int amount, TPipeline pipeline)
        {
            Amount = amount;
            Pipeline = pipeline;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool ISpanPipeline<TSpan, TResult>.MoveNext(ref SpanEnumeratorState<TSpan> state, [MaybeNullWhen(false)] out TResult result)
        {
            while (Amount > 0 && Pipeline.MoveNext(ref state, out _))
                Amount--;
            return Pipeline.MoveNext(ref state, out result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly bool ISpanPipeline<TSpan, TResult>.TryGetCount(in SpanEnumeratorState<TSpan> state, out int count)
        {
            if (Pipeline.TryGetCount(in state, out var cnt))
            {
                count = Math.Max(0, cnt - Amount);
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
    public struct SkipPipelineSpanType<TResult, TPipeline> : ISpanPipeline<TResult, TResult>
        where TPipeline : ISpanPipeline<TResult, TResult>
    {
        internal TPipeline Pipeline; // pipeline may be mutable
        internal int Amount;

        public SkipPipelineSpanType(int amount, TPipeline pipeline)
        {
            Amount = amount;
            Pipeline = pipeline;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool ISpanPipeline<TResult, TResult>.MoveNext(ref SpanEnumeratorState<TResult> state, [MaybeNullWhen(false)] out TResult result)
        {
            while (Amount > 0 && Pipeline.MoveNext(ref state, out _))
                Amount--;
            return Pipeline.MoveNext(ref state, out result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly bool ISpanPipeline<TResult, TResult>.TryGetCount(in SpanEnumeratorState<TResult> state, out int count)
        {
            if (Pipeline.TryGetCount(in state, out var cnt))
            {
                count = Math.Max(0, cnt - Amount);
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
