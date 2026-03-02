//using NetTopologySuite;
//using NetTopologySuite.Geometries;
//using SharpKml.Dom;
//using SharpKml.Base;
//using SharpKml.Engine;
//using System.Numerics;

//namespace Masofa.BusinessLogic.GeoIo
//{
//    /// <summary>
//    /// Универсальный ридер для извлечения Placemarks c Polygon и их атрибутов из KML.
//    /// </summary>
//    public sealed class SharpKmlReader
//    {
//        private readonly GeometryFactory _gf =
//        NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

//        public sealed record KmlPlacemark(NetTopologySuite.Geometries.Geometry? Geometry, IReadOnlyDictionary<string, string> Attributes);

//        public Task<List<KmlPlacemark>> ReadPlacemarksAsync(Stream kmlStream, CancellationToken ct = default)
//        {
//            var list = new List<KmlPlacemark>();

//            var kmlFile = KmlFile.Load(kmlStream);
//            if (kmlFile?.Root == null) return Task.FromResult(list);

//            foreach (var placemark in kmlFile.Root.Flatten().OfType<Placemark>())
//            {
//                var geom = ExtractPolygon(placemark.Geometry);
//                var attrs = ExtractAttributes(placemark);
//                list.Add(new KmlPlacemark(geom, attrs));
//            }

//            return Task.FromResult(list);
//        }

//        private NetTopologySuite.Geometries.Geometry? ExtractPolygon(SharpKml.Dom.Geometry? sharpGeom)
//        {
            
//            if (sharpGeom is SharpKml.Dom.Polygon p)
//            {
//                return ToNtsPolygon(p);
//            }

//            if (sharpGeom is MultipleGeometry mg)
//            {
                
//                var polys = mg.Geometry?.OfType<SharpKml.Dom.Polygon>()
//                               .Select(ToNtsPolygon)
//                               .Where(g => g != null)
//                               .Cast<NetTopologySuite.Geometries.Polygon>()
//                               .ToArray();

//                return polys is { Length: > 0 } ? _gf.CreateMultiPolygon(polys) : null;
//            }

//            return null;
//        }

//        private NetTopologySuite.Geometries.Polygon? ToNtsPolygon(SharpKml.Dom.Polygon kmlPoly)
//        {
//            var outer = kmlPoly.OuterBoundary?.LinearRing?.Coordinates;
//            if (outer == null) return null;

//            var outerRing = _gf.CreateLinearRing(ToNts(outer));

//            var innerRings = new List<NetTopologySuite.Geometries.LinearRing>();
//            if (kmlPoly.InnerBoundary != null)
//            {
//                foreach (var ib in kmlPoly.InnerBoundary)
//                {
//                    var coords = ib.LinearRing?.Coordinates;
//                    if (coords == null) continue;
//                    innerRings.Add(_gf.CreateLinearRing(ToNts(coords)));
//                }
//            }

//            return _gf.CreatePolygon(outerRing, innerRings.ToArray());
//        }

//        private static Coordinate[] ToNts(IEnumerable<SharpKml.Base.Vector> coords)
//        {
//            var list = new List<Coordinate>();
//            foreach (var v in coords)
//            {
                
//                list.Add(new Coordinate(v.Longitude, v.Latitude));
//            }
            
//            if (list.Count > 0 && !list[0].Equals2D(list[^1]))
//                list.Add(new Coordinate(list[0].X, list[0].Y));

//            return list.ToArray();
//        }

//        private static IReadOnlyDictionary<string, string> ExtractAttributes(Placemark pm)
//        {
//            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

//            if (!string.IsNullOrWhiteSpace(pm.Name))
//                map["name"] = pm.Name!;

//            var ext = pm.ExtendedData;
//            if (ext != null)
//            {
                
//                foreach (var d in ext.Data)
//                {
//                    if (!string.IsNullOrWhiteSpace(d.Name))
//                    {
//                        map[d.Name!] = d.Value ?? string.Empty;
//                    }
//                }

                
//                foreach (var sd in ext.SchemaData)
//                {
//                    foreach (var s in sd.SimpleData)
//                    {
//                        if (!string.IsNullOrWhiteSpace(s.Name))
//                            map[s.Name!] = s.Text ?? string.Empty;
//                    }
//                }
//            }

//            return map;
//        }
//    }
//}