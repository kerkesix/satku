namespace KsxEventTracker.Domain.Messages
{
    using System;
    using System.Collections.Generic;

    public class CommandTypes
    {
        private readonly Dictionary<string, Type> commandTypes = new Dictionary<string, Type>();

        public Type CreateCommandTypeFromCommandName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            if (this.commandTypes.ContainsKey(name))
            {
                return this.commandTypes[name];
            }
            
            var cached = Type.GetType(this.GetType().Namespace + ".Commands." + name, false, true);
            this.commandTypes.Add(name, cached);

            return cached;
        }

        public Type CreateCommandTypeFromFullName(string name)
        {
            var t = Type.GetType(name);

            if (t == null)
            {
                throw new InvalidOperationException("Command type " + name + " not found.");
            }

            return t;
        }
    }
}
