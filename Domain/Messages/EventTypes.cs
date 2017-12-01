namespace KsxEventTracker.Domain.Messages
{
    using System;

    public class EventTypes
    {
        public Type CreateEventTypeFromFullName(string name)
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