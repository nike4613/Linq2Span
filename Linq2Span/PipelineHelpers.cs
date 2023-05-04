using CommunityToolkit.Diagnostics;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
                Debug.Assert(typeof(TSpan) == typeof(TResult));
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
                return ToArraySlow(pipeline, state);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static TResult[] ToArraySlow(TPipeline pipeline, SpanEnumeratorState<TSpan> state)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCopyTo(TPipeline pipeline, ReadOnlySpan<TSpan> span, Span<TResult> dest, out int copied)
        {
            if (typeof(TPipeline) == typeof(BasePipeline<TSpan>))
            {
                Debug.Assert(typeof(TSpan) == typeof(TResult));
                var castSpan = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<TSpan, TResult>(ref MemoryMarshal.GetReference(span)), span.Length);
                copied = castSpan.Length;
                return castSpan.TryCopyTo(dest);
            }

            var state = new SpanEnumeratorState<TSpan>(span);

            if (pipeline.TryGetCount(in state, out var count))
            {
                copied = count;
                if (dest.Length < count)
                {
                    return false;
                }
                else
                {
                    var i = 0;
                    while (pipeline.MoveNext(ref state, out var e))
                    {
                        dest[i++] = e;
                    }
                    Debug.Assert(i == count);
                    return true;
                }
            }
            else
            {
                var i = 0;
                while (pipeline.MoveNext(ref state, out var e))
                {
                    if (unchecked((uint)i < dest.Length))
                    {
                        dest[i++] = e;
                    }
                    else
                    {
                        Unsafe.SkipInit(out copied);
                        return false;
                    }
                }
                copied = i;
                return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CopyTo(TPipeline pipeline, ReadOnlySpan<TSpan> span, Span<TResult> dest)
        {
            if (typeof(TPipeline) == typeof(BasePipeline<TSpan>))
            {
                Debug.Assert(typeof(TSpan) == typeof(TResult));
                var castSpan = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<TSpan, TResult>(ref MemoryMarshal.GetReference(span)), span.Length);
                castSpan.CopyTo(dest);
                return castSpan.Length;
            }

            var state = new SpanEnumeratorState<TSpan>(span);

            if (pipeline.TryGetCount(in state, out var count))
            {
                var i = 0;
                if (unchecked((uint)count <= dest.Length))
                {
                    while (pipeline.MoveNext(ref state, out var e))
                    {
                        dest[i++] = e;
                    }
                }
                else
                {
                    ThrowArgumentException_SpanTooShort();
                }
                Debug.Assert(i == count);
                return count;
            }
            else
            {
                var i = 0;
                while (pipeline.MoveNext(ref state, out var e))
                {
                    if (unchecked((uint)i < dest.Length))
                    {
                        dest[i++] = e;
                    }
                    else
                    {
                        ThrowArgumentException_SpanTooShort();
                    }
                }
                return i;
            }
        }

        private static void ThrowArgumentException_SpanTooShort() => ThrowHelper.ThrowArgumentException("Destination span is too short for source", "dest");
    }
}
