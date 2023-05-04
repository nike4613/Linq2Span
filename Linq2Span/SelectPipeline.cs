using System.Runtime.CompilerServices;

namespace Linq2Span
{
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
}
