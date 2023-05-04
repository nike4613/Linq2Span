using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Linq2Span
{
    internal static class PipelineHelpers<TResult, TPipeline, TSpan>
        where TPipeline : ISpanPipeline<TSpan, TResult>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResult[] ToArray(TPipeline pipeline, ReadOnlySpan<TSpan> span)
        {
            if (typeof(TPipeline) == typeof(BasePipeline<TSpan>))
            {
                return Unsafe.As<TResult[]>(span.ToArray());
            }

            var state = new SpanEnumeratorState<TSpan>(span);

            if (pipeline.TryGetCount(in state, out var count))
            {
                var arr = new TResult[count];
                var i = 0;

                while (pipeline.MoveNext(ref state, out var e))
                {
                    arr[i++] = e;
                }
                return arr;
            }
            else
            {
                return ToArraySlow(ref pipeline, ref state);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static TResult[] ToArraySlow(ref TPipeline pipeline, ref SpanEnumeratorState<TSpan> state)
        {
            var arr = ArrayPool<TResult>.Shared.Rent(8); // 8 seems like a reasonable enough start
            var i = 0;

            while (pipeline.MoveNext(ref state, out var e))
            {
                if (i >= arr.Length)
                {
                    var newArr = ArrayPool<TResult>.Shared.Rent(arr.Length + 1);
                    Array.Copy(arr, newArr, arr.Length);
                    ArrayPool<TResult>.Shared.Return(arr);
                    arr = newArr;
                }

                arr[i++] = e;
            }

            if (i != arr.Length)
            {
                var newArr = new TResult[i];
                Array.Copy(arr, newArr, i);
                ArrayPool<TResult>.Shared.Return(arr);
                arr = newArr;
            }

            return arr;
        }

        public static List<TResult> ToList(TPipeline pipeline, ReadOnlySpan<TSpan> span)
        {
            var state = new SpanEnumeratorState<TSpan>(span);

            List<TResult> result = pipeline.TryGetCount(in state, out var count) ? new(count) : new();

            while (pipeline.MoveNext(ref state, out var e))
            {
                result.Add(e);
            }

            return result;
        }
    }
}
