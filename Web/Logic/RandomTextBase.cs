namespace Web.Logic
{
    using System;
    using System.Collections.Generic;

    public abstract class RandomTextBase : List<string>
    {
        protected RandomTextBase(string[] texts)
        {
            if (texts == null || texts.Length == 0)
            {
                throw new ArgumentNullException(nameof(texts));
            }

            this.AddRange(texts);
        }

        public int RandomIndex()
        {
            var r = new Random(DateTime.UtcNow.Millisecond);
            int index = r.Next(0, this.Count - 1);

            return index;            
        }
    }
}