using System;
using System.Runtime.InteropServices;

namespace Linq2Span
{
    public interface IStructFunc<in TIn, out TOut>
    {
        TOut Invoke(TIn arg0);
    }

    [StructLayout(LayoutKind.Auto)]
    public readonly struct DelegateStructFunc<TIn, TOut> : IStructFunc<TIn, TOut>
    {
        private readonly Func<TIn, TOut> del;

        public DelegateStructFunc(Func<TIn, TOut> del)
        {
            this.del = del;
        }

        public TOut Invoke(TIn arg0) => del.Invoke(arg0);
    }

    [StructLayout(LayoutKind.Auto)]
    public readonly struct DelegateStructFuncSameType<T> : IStructFunc<T, T>
    {
        private readonly Func<T, T> del;

        public DelegateStructFuncSameType(Func<T, T> del)
        {
            this.del = del;
        }

        public T Invoke(T arg0) => del.Invoke(arg0);
    }

    [StructLayout(LayoutKind.Auto)]
    public readonly struct DelegateStructPredicate<T> : IStructFunc<T, bool>
    {
        private readonly Func<T, bool> del;

        public DelegateStructPredicate(Func<T, bool> del)
        {
            this.del = del;
        }

        public bool Invoke(T arg0) => del.Invoke(arg0);
    }
}
