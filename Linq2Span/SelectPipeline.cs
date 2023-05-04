using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Linq2Span
{
    [StructLayout(LayoutKind.Auto)]
    public struct SelectPipeline<TTransform, TIn, TOut, TSpan, TPipeline> : ISpanPipeline<TSpan, TOut>
        where TTransform : IStructFunc<TIn, TOut>
        where TPipeline : ISpanPipeline<TSpan, TIn>
    {
        private TPipeline pipeline; // pipeline may be mutable
        private TTransform transform; // transform may by mutable

        public SelectPipeline(TTransform transform, TPipeline pipeline)
        {
            this.pipeline = pipeline;
            this.transform = transform;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool ISpanPipeline<TSpan, TOut>.MoveNext(ref SpanEnumeratorState<TSpan> state, out TOut result)
        {
            if (pipeline.MoveNext(ref state, out var pResult))
            {
                result = transform.Invoke(pResult);
                return true;
            }
            else
            {
                Unsafe.SkipInit(out result);
                return false;
            }
        }

        // a Select pipeline has the same count as its inner pipeline
        readonly bool ISpanPipeline<TSpan, TOut>.TryGetCount(in SpanEnumeratorState<TSpan> state, out int count)
            => pipeline.TryGetCount(in state, out count);
    }

    [StructLayout(LayoutKind.Auto)]
    public struct SelectPipelineInSpanType<TTransform, TOut, TSpan, TPipeline> : ISpanPipeline<TSpan, TOut>
        where TTransform : IStructFunc<TSpan, TOut>
        where TPipeline : ISpanPipeline<TSpan, TSpan>
    {
        private TPipeline pipeline; // pipeline may be mutable
        private TTransform transform; // transform may by mutable

        public SelectPipelineInSpanType(TTransform transform, TPipeline pipeline)
        {
            this.pipeline = pipeline;
            this.transform = transform;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool ISpanPipeline<TSpan, TOut>.MoveNext(ref SpanEnumeratorState<TSpan> state, out TOut result)
        {
            if (pipeline.MoveNext(ref state, out var pResult))
            {
                result = transform.Invoke(pResult);
                return true;
            }
            else
            {
                Unsafe.SkipInit(out result);
                return false;
            }
        }

        // a Select pipeline has the same count as its inner pipeline
        readonly bool ISpanPipeline<TSpan, TOut>.TryGetCount(in SpanEnumeratorState<TSpan> state, out int count)
            => pipeline.TryGetCount(in state, out count);
    }

    [StructLayout(LayoutKind.Auto)]
    public struct SelectPipelineSameType<TTransform, TResult, TSpan, TPipeline> : ISpanPipeline<TSpan, TResult>
        where TTransform : IStructFunc<TResult, TResult>
        where TPipeline : ISpanPipeline<TSpan, TResult>
    {
        private TPipeline pipeline; // pipeline may be mutable
        private TTransform transform; // transform may by mutable

        public SelectPipelineSameType(TTransform transform, TPipeline pipeline)
        {
            this.pipeline = pipeline;
            this.transform = transform;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool ISpanPipeline<TSpan, TResult>.MoveNext(ref SpanEnumeratorState<TSpan> state, out TResult result)
        {
            if (pipeline.MoveNext(ref state, out var pResult))
            {
                result = transform.Invoke(pResult);
                return true;
            }
            else
            {
                Unsafe.SkipInit(out result);
                return false;
            }
        }

        // a Select pipeline has the same count as its inner pipeline
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly bool ISpanPipeline<TSpan, TResult>.TryGetCount(in SpanEnumeratorState<TSpan> state, out int count)
            => pipeline.TryGetCount(in state, out count);
    }

    [StructLayout(LayoutKind.Auto)]
    public struct SelectPipelineSameSpanType<TTransform, TSpan, TPipeline> : ISpanPipeline<TSpan, TSpan>
        where TTransform : IStructFunc<TSpan, TSpan>
        where TPipeline : ISpanPipeline<TSpan, TSpan>
    {
        private TPipeline pipeline; // pipeline may be mutable
        private TTransform transform; // transform may by mutable

        public SelectPipelineSameSpanType(TTransform transform, TPipeline pipeline)
        {
            this.pipeline = pipeline;
            this.transform = transform;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool ISpanPipeline<TSpan, TSpan>.MoveNext(ref SpanEnumeratorState<TSpan> state, out TSpan result)
        {
            if (pipeline.MoveNext(ref state, out var pResult))
            {
                result = transform.Invoke(pResult);
                return true;
            }
            else
            {
                Unsafe.SkipInit(out result);
                return false;
            }
        }

        // a Select pipeline has the same count as its inner pipeline
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly bool ISpanPipeline<TSpan, TSpan>.TryGetCount(in SpanEnumeratorState<TSpan> state, out int count)
            => pipeline.TryGetCount(in state, out count);
    }
}
