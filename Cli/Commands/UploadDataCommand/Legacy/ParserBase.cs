namespace KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    public abstract class ParserBase<T> : IParser<T>
        where T : class
    {
        // All the supported conversion functions
        private readonly Dictionary<Type, Func<string, object>> converters = new Dictionary<Type, Func<string, object>> {
             { typeof(int), s => int.Parse(s, NumberStyles.Number, CultureInfo.InvariantCulture) },
             { typeof(decimal), s => decimal.Parse(s, CultureInfo.InvariantCulture) } ,
             { typeof(bool), s => s.Equals("1", StringComparison.OrdinalIgnoreCase) },
             { typeof(DateTime), s => DateTime.Parse(s)},
             { typeof(Guid), s => Guid.Parse(s)} 
        };

        private readonly int rowLength;
        private int currentIndex;

        protected ParserBase(string csvRaw, int rowLength)
        {
            this.rowLength = rowLength;
            var csvRows = csvRaw.Split("\r\n".ToCharArray());

            this.Rows = csvRows.Skip(1)
                               .Select(this.ParseRowInternal)
                               .Where(parsed => parsed != null)
                               .ToList();
        }

        public IEnumerable<T> Rows { get; private set; }

        public string[] Current { get; private set; }

        public abstract T ParseRow();

        protected TP Next<TP>()
        {
            return this.Get<TP>(this.currentIndex++);
        }

        protected string Next()
        {
            string value = this.Get(this.currentIndex++);

            if (value == "NULL")
            {
                value = null;
            }

            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        protected TP Get<TP>(int index)
        {
            if (this.Current == null || index > this.Current.Length)
            {
                return default(TP);
            }

            string value = this.Current[index];
            Type requestedType = typeof(TP);

            // Default to string if no other conversion specified
            object converted = value;
            
            // Special handling for enums
            if (requestedType.GetTypeInfo().IsEnum)
            {
                converted = Enum.Parse(requestedType, value);
            }

            // Use any of the registered converters
            if (this.converters.ContainsKey(requestedType))
            {
                converted = this.converters[requestedType](value);
            }

            return (TP)Convert.ChangeType(converted, requestedType);
        }

        protected string Get(int index)
        {
            return this.Get<string>(index);
        }

        private T ParseRowInternal(string row)
        {
            this.currentIndex = 0;

            if (string.IsNullOrWhiteSpace(row))
            {
                return null;
            }

            this.Current = row.Split(";".ToCharArray());

            if (this.Current.Length != this.rowLength)
            {
                return null;
            }

            return this.ParseRow();
        }
    }
}