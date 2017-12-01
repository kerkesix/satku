namespace KsxEventTracker.Domain.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using KsxEventTracker.Domain.Messages.Commands;
    using KsxEventTracker.Domain.Messages.Events;

    /// <summary>
    /// simple helper, that looks up and calls the proper overload of 
    /// When(SpecificEventType event). Reflection information is cached statically
    /// once per type. 
    /// </summary>
    public static class RedirectToWhen
    {
        static class Cache<T>
        {
            // ReSharper disable StaticFieldInGenericType
            public static readonly IDictionary<Type, MethodInfo> DictWhen = typeof(T)
                // ReSharper restore StaticFieldInGenericType
                .GetTypeInfo()
                .GetMethods()
                .Where(m => m.Name == "When")
                .Where(m => m.GetParameters().Length == 1)
                .ToDictionary(m => m.GetParameters().First().ParameterType, m => m);

            // ReSharper disable StaticFieldInGenericType
            public static readonly IDictionary<Type, MethodInfo> DictExecute = typeof(T)
                // ReSharper restore StaticFieldInGenericType    
                            .GetTypeInfo()
                            .GetMethods()
                            .Where(m => m.Name == "Execute")
                            .Where(m => m.GetParameters().Length == 1)
                            .ToDictionary(m => m.GetParameters().First().ParameterType, m => m);
        }

        public static void InvokeEvent<T>(T instance, IEvent command)
        {
            MethodInfo info;
            var type = command.GetType();
            if (!Cache<T>.DictWhen.TryGetValue(type, out info))
            {
                var s = String.Format("Failed to locate {0}.When({1})", typeof(T).Name, type.Name);
                throw new InvalidOperationException(s);
            }

            // Since target methods are async methods, need to await to be able to 
            // properly catch errors. Better would be to make this method also 
            // async or return a Task of bool.
            var result = (Task)info.Invoke(instance, new object[] { command });
            if (result != null) 
            {
                result.Wait();
            }
        }

        public static bool InvokeEventIfHandlerFound<T>(T instance, IEvent command)
        {
            MethodInfo info;
            var type = command.GetType();
            if (Cache<T>.DictWhen.TryGetValue(type, out info))
            {
                // Since target methods are async methods, need to await to be able to 
                // properly catch errors. Better would be to make this method also 
                // async or return a Task of bool.
                var result = (Task)info.Invoke(instance, new object[] { command });
                if (result != null && !result.IsCanceled && !result.IsFaulted)
                {
                    result.Wait();
                }
                
                return true;
            }

            return false;
        }

        public static void InvokeCommand<T>(T instance, ICommand command)
        {
            MethodInfo info;
            var type = command.GetType();
            if (!Cache<T>.DictExecute.TryGetValue(type, out info))
            {
                var s = String.Format("Failed to locate {0}.When({1})", typeof(T).Name, type.Name);
                throw new InvalidOperationException(s);
            }

            info.Invoke(instance, new object[] { command });
        }

        public static bool InvokeCommandIfHandlerFound<T>(T instance, ICommand command) => InvokeCommandIfHandlerFoundCore(instance, command);

        private static bool InvokeCommandIfHandlerFoundCore<T>(T instance, object command)
        {
            MethodInfo info;
            var type = command.GetType();
            if (!Cache<T>.DictExecute.TryGetValue(type, out info))
            {
                return false;
            }

            // Since target methods are async methods, need to await to be able to 
            // properly catch errors. Better would be to make this method also 
            // async or return a Task of bool.
            var result = (Task)info.Invoke(instance, new[] { command });
            if (result != null && !result.IsCanceled && !result.IsFaulted)
            {
                result.Wait();
            }
            return true;
        }
    }
}