namespace Web.ontrollers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;

    using QueryModels;
    using Microsoft.AspNetCore.Mvc;

    public partial class RouteController : Controller
    {
        [Route("api/{happening}/[controller]")]
        public virtual IActionResult Get(string happening)
        {
            var geoJson = new GeoJson();

            var h = QueryModelRepository.Dashboard.Happenings[happening];

            if (h != null)
            {
                var points = h.Checkpoints.Select(cp => Feature.Point(cp.Longitude, cp.Latitude, cp.Name));
                geoJson.AddFeatures(points);
                
                // Empty path is ok and ignored on geoJson, do not check
                // const string TestPath = "[24.0619469,60.2098736], [24.0623868, 60.2099429],[24.0627623,60.2098416],[24.0629125,60.209719],[24.0632665,60.2095751],[24.0641999,60.2096018],[24.0651655,60.2098203],[24.0663886,60.2099536] ";
                geoJson.AddFeatures(new[] { Feature.LineString(h.Path, "Reitti") });                    
            }

            return Json(geoJson);
        }
    }

    public class GeoJson
    {
        public string Type { get { return "FeatureCollection"; } }

        public IList<Feature> Features { get; private set; }

        public GeoJson()
        {
            this.Features = new List<Feature>();
        }

        public void AddFeatures(IEnumerable<Feature> features)
        {
            ((List<Feature>)this.Features).AddRange(features.Where(f => f.Geometry != null));
        }
    }

    public class Feature
    {
        public string Type { get { return "Feature"; } }

        public IGeometry Geometry { get; private set; }

        public Properties Properties { get; private set; }

        public static Feature Point(double lon, double lat, string title)
        {
            return new Feature(lon, lat, title);         
        }

        public static Feature LineString(string latLonArrayAsString, string title)
        {
            return new Feature(latLonArrayAsString, title);
        }

        private Feature(double lon, double lat, string title)
        {
            this.Geometry = new PointGeometry(lon, lat);
            this.Properties = new Properties { Title = title };
        }

        private Feature(string lonLatArray, string title)
        {
            if (!string.IsNullOrWhiteSpace(lonLatArray))
            {
                this.Geometry = new LineStringGeometry(lonLatArray);
            }

            this.Properties = new Properties { Title = title };
        }
    }

    public interface IGeometry
    {
        string Type { get; }
    }

    public class PointGeometry : IGeometry
    {
        public string Type { get { return "Point"; } }

        public double[] Coordinates { get; private set; }

        public PointGeometry(double lon, double lat)
        {
            this.Coordinates = new[] { lon, lat };
        }
    }

    public class LineStringGeometry: IGeometry
    {
        public string Type { get { return "LineString"; } }

        public double[][] Coordinates { get; private set; }

        /// <summary>
        /// Creates a new line string geometry.
        /// </summary>
        /// <param name="lonLatArray">
        ///     String representation of latitude and longitude in form '[lon, lat], [lon2, lat2]'.</param>
        public LineStringGeometry(string lonLatArray)
        {
            try
            {
                // If parsing fails, just do nothing as this is not a critical feature but likely to fail
                this.Coordinates =
                    lonLatArray.Replace(" ", string.Empty)
                        .TrimStart("[".ToCharArray())
                        .TrimEnd("]".ToCharArray())
                        .Split(new[] { "],[" }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(
                            pair =>
                                pair.Split(',')
                                    .Select(d => double.Parse(d, CultureInfo.InvariantCulture))
                                    .ToArray())
                        .ToArray();
            }
            catch (ArgumentNullException aex)
            {
                Trace.TraceWarning("Parsing coordinates failed: {0}", aex);
            }
            catch (FormatException fex)
            {
                Trace.TraceWarning("Parsing coordinates failed: {0}", fex);
            }
        }
    }

    public class Properties
    {
        public string Title { get; set; }
    }
}
