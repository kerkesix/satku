namespace KsxEventTracker.Domain.Messages.Commands
{
    using System.Collections.Generic;

    public class CommandContext : Dictionary<string, object>
    {
        public string User
        {
            get
            {
                return (string)this["User"];
            }
            set
            {
                this["User"] = value;
            }
        }   
    }
}