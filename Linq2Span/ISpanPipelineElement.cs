using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linq2Span
{
    internal interface ISpanPipelineElement<T>
    {
        bool MoveNext(ref SpanEnumeratorState<T> state, [NotNullWhen(true)] out T? result);
    }
}
