using System.Runtime.CompilerServices;

namespace Linq2Span
{
    public struct WherePipeline<TResult, TPipeline, TSpan, TFilter> : ISpanPipeline<TSpan, TResult>
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

        bool ISpanPipeline<TSpan, TResult>.TryGetCount(in SpanEnumeratorState<TSpan> state, out int count)
        {
            // we can never know the count of a Where pipeline without enumerating
            Unsafe.SkipInit(out count);
            return false;
        }
    }
}
