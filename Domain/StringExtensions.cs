namespace KsxEventTracker.Domain
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Text;

    public static class StringExtensions
    {
        public static string FromGzip(this string source)
        {
            try
            {
                using (var sourceStream = new MemoryStream(Convert.FromBase64String(source ?? "")))
                using (var decompress = new GZipStream(sourceStream, CompressionMode.Decompress))
                using (var sr = new StreamReader(decompress))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (FormatException formatException)
            {
                // TODO: Re-do all tracing with some other logging mechanism.
                Trace.TraceWarning("Invalid gzip string '{0}', error is: {1}", source, formatException);
                return source;
            }
            catch (InvalidDataException argEx)
            {
                Trace.TraceWarning("Invalid gzip string '{0}', error is: {1}", source, argEx);
                return source;
            }
        }

        public static string ToGzip(this string source)
        {
            using (var msin = new MemoryStream(Encoding.UTF8.GetBytes(source ?? "")))
            using (var msout = new MemoryStream())
            {
                using (var gs = new GZipStream(msout, CompressionMode.Compress))
                {
                    msin.CopyTo(gs);
                }

                return Convert.ToBase64String(msout.ToArray());
            }
        }
    }
}