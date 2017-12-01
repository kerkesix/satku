namespace KsxEventTracker.Domain
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class TypeFactory
    {
        public static Func<T> GetCtor<T>() { return Cache<T>.func; }
        public static Func<TArg1, T> GetCtor<TArg1, T>() { return Cache<T, TArg1>.func; }
        public static Func<TArg1, TArg2, T> GetCtor<TArg1, TArg2, T>() { return Cache<T, TArg1, TArg2>.func; }
        private static Delegate CreateConstructor(Type type, params Type[] args)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (args == null) args = Type.EmptyTypes;
            ParameterExpression[] @params = args.AsEnumerable().Select(Expression.Parameter).ToArray();
            return Expression.Lambda(Expression.New(type.GetTypeInfo().GetConstructor(args), @params), @params).Compile();
        }
        private static class Cache<T>
        {
            public static readonly Func<T> func = (Func<T>)TypeFactory.CreateConstructor(typeof(T));
        }
        private static class Cache<T, TArg1>
        {
            public static readonly Func<TArg1, T> func = (Func<TArg1, T>)TypeFactory.CreateConstructor(typeof(T), typeof(TArg1));
        }
        private static class Cache<T, TArg1, TArg2>
        {
            public static readonly Func<TArg1, TArg2, T> func = (Func<TArg1, TArg2, T>)TypeFactory.CreateConstructor(typeof(T), typeof(TArg1), typeof(TArg2));
        }
    }
}