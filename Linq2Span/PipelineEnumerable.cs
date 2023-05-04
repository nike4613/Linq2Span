using System;
using System.Diagnostics.CodeAnalysis;

namespace Linq2Span
{
    public readonly ref struct PipelineEnumerable<TResult, TPipeline, TSpan>
        where TPipeline : ISpanPipeline<TSpan, TResult>
    {
        internal readonly ReadOnlySpan<TSpan> Span;

        internal readonly TPipeline Pipeline;

        public PipelineEnumerable(ReadOnlySpan<TSpan> span, TPipeline pipeline)
        {
            Span = span;
            Pipeline = pipeline;
        }

        public bool TryGetCount(out int count)
            => Pipeline.TryGetCount(new SpanEnumeratorState<TSpan>(Span), out count);

        public PipelineEnumerator<TResult, TPipeline, TSpan> GetEnumerator() => new(Span, Pipeline);
    }

    public ref struct PipelineEnumerator<TResult, TPipeline, TSpan>
        where TPipeline : ISpanPipeline<TSpan, TResult>
    {
        private SpanEnumeratorState<TSpan> state;
        private TPipeline pipeline;

        internal PipelineEnumerator(ReadOnlySpan<TSpan> span, TPipeline pipeline)
        {
            state = new(span);
            this.pipeline = pipeline;
            current = default!;
        }

        private TResult? current;
        public TResult Current => current!;

        public bool MoveNext() => pipeline.MoveNext(ref state, out current);
    }
}
