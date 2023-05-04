using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Linq2Span
{
    [StructLayout(LayoutKind.Auto)]
    public readonly ref partial struct PipelineEnumerable<TResult, TPipeline>
        where TPipeline : ISpanPipeline<TResult, TResult>
    {
        internal readonly ReadOnlySpan<TResult> Span;

        internal readonly TPipeline Pipeline;

        public PipelineEnumerable(ReadOnlySpan<TResult> span, TPipeline pipeline)
        {
            Span = span;
            Pipeline = pipeline;
        }

        public bool TryGetCount(out int count) => Pipeline.TryGetCount(new SpanEnumeratorState<TResult>(Span), out count);

        public PipelineEnumerator<TResult, TPipeline, TResult> GetEnumerator() => new(Span, Pipeline);

        public static implicit operator PipelineEnumerable<TResult, TPipeline>(PipelineEnumerable<TResult, TPipeline, TResult> other)
            => new(other.Span, other.Pipeline);

        public TResult[] ToArray() => PipelineHelpers<TResult, TPipeline, TResult>.ToArray(Pipeline, Span);
        public List<TResult> ToList() => PipelineHelpers<TResult, TPipeline, TResult>.ToList(Pipeline, Span);

        public int CopyTo(Span<TResult> dest) => PipelineHelpers<TResult, TPipeline, TResult>.CopyTo(Pipeline, Span, dest);
        public bool TryCopyTo(Span<TResult> dest) => PipelineHelpers<TResult, TPipeline, TResult>.TryCopyTo(Pipeline, Span, dest, out _);
        public bool TryCopyTo(Span<TResult> dest, out int wrote) => PipelineHelpers<TResult, TPipeline, TResult>.TryCopyTo(Pipeline, Span, dest, out wrote);
    }

    [StructLayout(LayoutKind.Auto)]
    public readonly ref partial struct PipelineEnumerable<TResult, TPipeline, TSpan>
        where TPipeline : ISpanPipeline<TSpan, TResult>
    {
        internal readonly ReadOnlySpan<TSpan> Span;

        internal readonly TPipeline Pipeline;

        public PipelineEnumerable(ReadOnlySpan<TSpan> span, TPipeline pipeline)
        {
            Span = span;
            Pipeline = pipeline;
        }

        public bool TryGetCount(out int count) => Pipeline.TryGetCount(new SpanEnumeratorState<TSpan>(Span), out count);

        public PipelineEnumerator<TResult, TPipeline, TSpan> GetEnumerator() => new(Span, Pipeline);

        public TResult[] ToArray() => PipelineHelpers<TResult, TPipeline, TSpan>.ToArray(Pipeline, Span);
        public List<TResult> ToList() => PipelineHelpers<TResult, TPipeline, TSpan>.ToList(Pipeline, Span);

        public int CopyTo(Span<TResult> dest) => PipelineHelpers<TResult, TPipeline, TSpan>.CopyTo(Pipeline, Span, dest);
        public bool TryCopyTo(Span<TResult> dest) => PipelineHelpers<TResult, TPipeline, TSpan>.TryCopyTo(Pipeline, Span, dest, out _);
        public bool TryCopyTo(Span<TResult> dest, out int wrote) => PipelineHelpers<TResult, TPipeline, TSpan>.TryCopyTo(Pipeline, Span, dest, out wrote);
    }

    [StructLayout(LayoutKind.Auto)]
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
        public TResult Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => current!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => pipeline.MoveNext(ref state, out current);
    }
}
