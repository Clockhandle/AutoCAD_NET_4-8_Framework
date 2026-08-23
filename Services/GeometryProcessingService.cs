using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using AutoCAD_NET_4_8_Framework;
using MyMiningPlugin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AcApp = Autodesk.AutoCAD.ApplicationServices.Application;

namespace MyMiningPlugin.Services
{
    /// <summary>
    /// Processes CAD geometry and extracts vertex/color data
    /// </summary>
    public class GeometryProcessingService
    {
        public readonly AutoCADSelectionService _selectionService;
        private readonly DataQualityService _qualityService = new DataQualityService();

        public GeometryProcessingService(AutoCADSelectionService selectionService)
        {
            _selectionService = selectionService;
        }

        /// <summary>
        /// Process geometry from SurfaceData and return CADObjectData list
        /// </summary>
        public Task<List<CADObjectData>> ProcessGeometryWithSmartZ(SurfaceData surface)
        {
            var raw = ExtractRawGeometry(surface);
            ApplyBeMatClipping(surface, raw);
            return Task.FromResult(raw);
        }

        /// <summary>
        /// Process a raw GeometryReference list (not wrapped in SurfaceData) and return
        /// CADObjectData. Used for mine topology layers (Nền, Nóc, Biên).
        /// </summary>
        public Task<List<CADObjectData>> ProcessGeometryList(
            List<GeometryReference> refs, string groupName, string objectType)
        {
            // Wrap in a temporary SurfaceData so we can reuse ExtractRawGeometry
            var surface = new SurfaceData
            {
                Type        = objectType,
                ParentName  = groupName,
                SelectedGeometry = refs
            };
            var raw = ExtractRawGeometry(surface);
            return Task.FromResult(raw);
        }

        /// <summary>
        /// For each vertex in every Biên polyline, replaces its Z with the Z of the
        /// nearest Nền vertex in XY space. This ensures Biên inherits the floor
        /// elevation at the closest measured point on the tunnel centreline.
        /// </summary>
        public void AssignBienZFromNen(
            List<CADObjectData> bienList,
            List<CADObjectData> nenList)
        {
            if (bienList == null || bienList.Count == 0) return;
            if (nenList  == null || nenList.Count  == 0) return;

            // Flatten all Nền vertices into a single searchable list.
            var nenVertices = nenList
                .SelectMany(n => n.FlattenedVertices)
                .ToList();

            if (nenVertices.Count == 0) return;

            foreach (var bien in bienList)
            {
                for (int i = 0; i < bien.FlattenedVertices.Count; i++)
                {
                    double[] bv = bien.FlattenedVertices[i];
                    double bx = bv[0], by = bv[1];

                    // Find closest Nền vertex by XY distance.
                    double bestDist = double.MaxValue;
                    double bestZ    = bv[2]; // keep original as fallback

                    foreach (var nv in nenVertices)
                    {
                        double dx = nv[0] - bx;
                        double dy = nv[1] - by;
                        double dist = dx * dx + dy * dy; // squared — no need for sqrt
                        if (dist < bestDist)
                        {
                            bestDist = dist;
                            bestZ    = nv[2];
                        }
                    }

                    // Overwrite Z in both vertex arrays.
                    bien.FlattenedVertices[i] = new double[] { bx, by, bestZ };
                    if (i < bien.Vertices.Count)
                        bien.Vertices[i] = new double[] { bx, by, bestZ };
                }
            }
        }

        /// <summary>
        /// Runs data-quality checks on the raw pre-clip geometry, then returns
        /// the clipped geometry alongside any issues found.
        /// </summary>
        public Task<(List<CADObjectData> Geometry, List<QualityIssue> Issues)>
            ProcessGeometryWithQualityChecks(
                SurfaceData surface,
                List<CADObjectData> terrainItems = null)
        {
            // 1. Extract entities — NO clipping yet
            var raw = ExtractRawGeometry(surface);

            // 2. Run all checks on raw data
            //    terrainItems for Check 3: the caller can pass Vỉa/Khối lines;
            //    if null we fall back to the surface's own non-boundary, non-breakline items.
            var checkTerrain = terrainItems
                ?? raw.Where(r => !r.IsBoundary && !r.IsHole && !r.IsBreakline).ToList();

            var issues = surface.Type == "Bề mặt"
                ? _qualityService.RunAllChecks(raw, checkTerrain)
                : new List<QualityIssue>();

            // 3. Now apply the boundary clip in-place
            ApplyBeMatClipping(surface, raw);

            return Task.FromResult((raw, issues));
        }

        // -------------------------------------------------------------------------
        // Private: entity extraction (no clipping)
        // -------------------------------------------------------------------------

        private List<CADObjectData> ExtractRawGeometry(SurfaceData surface)
        {
            List<CADObjectData> result = new List<CADObjectData>();

            Document doc = AcApp.DocumentManager.MdiActiveDocument;
            if (doc == null) return result;

            Database db = doc.Database;

            int skippedCount = 0;
            int processedCount = 0;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                var combinedList = surface.SelectedGeometry.Select(g => new { GeoRef = g, IsBoundary = false, IsHole = false, IsBreakline = false })
                    .Concat(surface.BoundaryGeometry.Select(g => new { GeoRef = g, IsBoundary = true, IsHole = false, IsBreakline = false }))
                    .Concat(surface.HoleGeometry.Select(g => new { GeoRef = g, IsBoundary = false, IsHole = true, IsBreakline = false }))
                    .Concat(surface.BreaklineGeometry.Select(g => new { GeoRef = g, IsBoundary = false, IsHole = false, IsBreakline = true }));

                foreach (var item in combinedList)
                {
                    var geoRef = item.GeoRef;
                    bool hasLiveId = geoRef.CurrentObjectId.HasValue && !geoRef.CurrentObjectId.Value.IsNull;

                    // If the entity is not in the current drawing but we have cached vertices, build
                    // a CADObjectData directly from the cache (used for cross-section shapes from
                    // a different map file).
                    if (!hasLiveId)
                    {
                        if (geoRef.CachedVertices != null && geoRef.CachedVertices.Count > 0)
                        {
                            var cadDataCached = new CADObjectData
                            {
                                GroupName         = surface.ParentName,
                                ObjectType        = surface.Type,
                                Layer             = geoRef.Layer ?? string.Empty,
                                Handle            = geoRef.Handle,
                                IsBoundary        = item.IsBoundary,
                                IsHole            = item.IsHole,
                                IsBreakline       = item.IsBreakline,
                                IsClosed          = geoRef.IsClosed,
                                Vertices          = geoRef.CachedVertices.Select(v => new double[] { v[0], v[1], v[2] }).ToList(),
                                FlattenedVertices = geoRef.CachedVertices.Select(v => new double[] { v[0], v[1], v[2] }).ToList()
                            };
                            result.Add(cadDataCached);
                            processedCount++;
                        }
                        else
                        {
                            skippedCount++;
                        }
                        continue;
                    }

                    try
                    {
                        Entity ent = tr.GetObject(geoRef.CurrentObjectId.Value, OpenMode.ForRead) as Entity;
                        if (ent == null) { skippedCount++; continue; }

                        CADObjectData cadData = new CADObjectData
                        {
                            GroupName  = surface.ParentName,
                            ObjectType = surface.Type,
                            Layer      = ent.Layer,
                            Handle     = ent.Handle.ToString(),
                            IsBoundary = item.IsBoundary,
                            IsHole     = item.IsHole,
                            IsBreakline = item.IsBreakline,
                            Vertices          = new List<double[]>(),
                            FlattenedVertices = new List<double[]>()
                        };

                        ExtractColorInformation(cadData, ent, db, tr);

                        List<Point3d> rawPoints = ExtractGeometry(cadData, ent);
                        foreach (var pt in rawPoints)
                        {
                            cadData.Vertices.Add(new double[] { pt.X, pt.Y, pt.Z });
                            cadData.FlattenedVertices.Add(new double[] { pt.X, pt.Y, pt.Z });
                        }

                        result.Add(cadData);
                        processedCount++;
                    }
                    catch (Exception) { skippedCount++; }
                }
                tr.Commit();
            }

            if (skippedCount > 0)
                System.Diagnostics.Debug.WriteLine(
                    $"ExtractRawGeometry: Processed {processedCount}, Skipped {skippedCount}");

            return result;
        }

        // -------------------------------------------------------------------------
        // Private: Bề mặt boundary clip (mutates the list in-place)
        // -------------------------------------------------------------------------

        private void ApplyBeMatClipping(SurfaceData surface, List<CADObjectData> result)
        {
            if (surface.Type != "Bề mặt" || !result.Any(r => r.IsBoundary)) return;

            var boundaryRings = result
                .Where(r => r.IsBoundary)
                .Select(r => r.FlattenedVertices.Select(v => new Point2D(v[0], v[1])).ToList())
                .Where(ring => ring.Count >= 3)
                .ToList();

            if (boundaryRings.Count == 0) return;

            // 1. Remove terrain points outside every boundary ring
            var terrainItems = result.Where(r => !r.IsBoundary && !r.IsHole && !r.IsBreakline).ToList();
            foreach (var cadData in terrainItems)
            {
                cadData.Vertices = cadData.Vertices
                    .Where(v => boundaryRings.Any(ring => PointInPolygon(v[0], v[1], ring)))
                    .ToList();
                cadData.FlattenedVertices = cadData.FlattenedVertices
                    .Where(v => boundaryRings.Any(ring => PointInPolygon(v[0], v[1], ring)))
                    .ToList();
            }

            // 2. Clip breaklines; capture originals BEFORE clipping
            var breaklineItems = result.Where(r => r.IsBreakline).ToList();
            var originalBreaklineChains = breaklineItems
                .Select(b => b.FlattenedVertices.ToList())
                .ToList();
            var extraBreaklines = new List<CADObjectData>();

            foreach (var cadData in breaklineItems)
            {
                var subSegments = ClipPolylineSegmentsToPolygon(cadData.FlattenedVertices, boundaryRings);

                if (subSegments.Count == 0)
                {
                    cadData.Vertices = new List<double[]>();
                    cadData.FlattenedVertices = new List<double[]>();
                }
                else
                {
                    cadData.Vertices = subSegments[0];
                    cadData.FlattenedVertices = subSegments[0];

                    for (int k = 1; k < subSegments.Count; k++)
                    {
                        extraBreaklines.Add(new CADObjectData
                        {
                            GroupName  = cadData.GroupName,
                            ObjectType = cadData.ObjectType,
                            Layer      = cadData.Layer,
                            Handle     = cadData.Handle + $"_seg{k}",
                            IsBoundary = false,
                            IsHole     = false,
                            IsBreakline = true,
                            ColorIndex = cadData.ColorIndex,
                            ColorName  = cadData.ColorName,
                            TrueColor  = cadData.TrueColor,
                            IsClosed   = false,
                            Vertices          = subSegments[k],
                            FlattenedVertices = subSegments[k]
                        });
                    }
                }
            }

            result.AddRange(extraBreaklines);

            // 3. Assign real Z to boundary vertices from original breakline crossings
            foreach (var cadData in result.Where(r => r.IsBoundary))
                AssignBoundaryZ(cadData, originalBreaklineChains, boundaryRings);
        }

        private void ExtractColorInformation(CADObjectData cadData, Entity ent, Database db, Transaction tr)
        {
            LayerTable layerTable = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
            if (layerTable != null && layerTable.Has(ent.Layer))
            {
                LayerTableRecord layerRecord = tr.GetObject(layerTable[ent.Layer], OpenMode.ForRead) as LayerTableRecord;
                if (layerRecord != null)
                {
                    var color = layerRecord.Color;
                    cadData.ColorIndex = color.ColorIndex;
                    
                    // Get color name
                    if (color.IsByLayer)
                        cadData.ColorName = "ByLayer";
                    else if (color.IsByBlock)
                        cadData.ColorName = "ByBlock";
                    else if (color.ColorIndex >= 0 && color.ColorIndex <= 255)
                        cadData.ColorName = $"Index{color.ColorIndex}";
                    else
                        cadData.ColorName = "TrueColor";
                    
                    // Get RGB values
                    try
                    {
                        cadData.TrueColor = new int[] { color.Red, color.Green, color.Blue };
                    }
                    catch
                    {
                        cadData.TrueColor = new int[] { 255, 255, 255 };
                    }
                }
            }
            else
            {
                // Default color if layer not found
                cadData.ColorIndex = 7; // White
                cadData.ColorName = "ByLayer";
                cadData.TrueColor = new int[] { 255, 255, 255 };
            }
        }

        /// <summary>
        /// Strips all MText inline formatting codes and returns the plain numeric text.
        /// Handles: \fFont|b0|i0|c0|p34; \H; \W; \T; \Q; \A; \C; \P; \S; {}
        /// </summary>
        private static string StripMTextFormatting(string contents)
        {
            if (string.IsNullOrEmpty(contents)) return string.Empty;
            // Remove \f...;  \H...;  \W...;  etc. (codes with pipe-delimited parameters)
            string s = System.Text.RegularExpressions.Regex.Replace(
                contents, @"\\[A-Za-z][^;]*;", "");
            // Remove remaining backslash sequences without semicolons (e.g. \P \~ \\)
            s = System.Text.RegularExpressions.Regex.Replace(s, @"\\.", "");
            // Remove braces
            s = s.Replace("{", "").Replace("}", "");
            return s.Trim();
        }

        private List<Point3d> ExtractGeometry(CADObjectData cadData, Entity ent)
        {
            List<Point3d> rawPoints = new List<Point3d>();

            if (ent is DBPoint dbpt)
            {
                cadData.IsClosed = false;
                rawPoints.Add(dbpt.Position);
            }
            else if (ent is DBText dbText)
            {
                cadData.IsClosed = false;
                double z;
                if (!double.TryParse(dbText.TextString.Trim(),
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out z))
                {
                    z = dbText.Position.Z;
                }
                rawPoints.Add(new Point3d(dbText.Position.X, dbText.Position.Y, z));
            }
            else if (ent is MText mText)
            {
                cadData.IsClosed = false;
                string raw = StripMTextFormatting(mText.Contents);
                double z;
                if (!double.TryParse(raw,
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out z))
                {
                    z = mText.Location.Z;
                }
                rawPoints.Add(new Point3d(mText.Location.X, mText.Location.Y, z));
            }
            else if (ent is Polyline pl)
            {
                cadData.IsClosed = pl.Closed;
                for (int i = 0; i < pl.NumberOfVertices; i++)
                {
                    rawPoints.Add(pl.GetPoint3dAt(i));
                }
            }
            else if (ent is Polyline3d poly3d)
            {
                cadData.IsClosed = poly3d.Closed;
                foreach (ObjectId vertexId in poly3d)
                {
                    var v3d = ent.Database.TransactionManager.TopTransaction.GetObject(vertexId, OpenMode.ForRead) as PolylineVertex3d;
                    if (v3d != null)
                        rawPoints.Add(v3d.Position);
                }
            }
            else if (ent is Polyline2d poly2d)
            {
                cadData.IsClosed = poly2d.Closed;
                foreach (ObjectId vertexId in poly2d)
                {
                    var v2d = ent.Database.TransactionManager.TopTransaction.GetObject(vertexId, OpenMode.ForRead) as Vertex2d;
                    if (v2d != null)
                        rawPoints.Add(v2d.Position);
                }
            }
            else if (ent is Spline spline)
            {
                cadData.IsClosed = spline.Closed;
                if (spline.NumControlPoints > 0)
                {
                    for (int i = 0; i < spline.NumControlPoints; i++)
                    {
                        rawPoints.Add(spline.GetControlPointAt(i));
                    }
                }
            }
            else if (ent is Line line)
            {
                cadData.IsClosed = false;
                rawPoints.Add(line.StartPoint);
                rawPoints.Add(line.EndPoint);
            }

            return rawPoints;
        }

        // -------------------------------------------------------------------------
        // 2-D clipping helpers (no external library required)
        // -------------------------------------------------------------------------

        private struct Point2D
        {
            public double X, Y;
            public Point2D(double x, double y) { X = x; Y = y; }
        }

        /// <summary>
        /// Assigns Z values to a boundary ring's vertices.
        /// Each vertex where a breakline crosses the boundary receives the breakline's
        /// interpolated Z at that crossing. All other vertices are Z-interpolated by
        /// cumulative arc-length between the nearest known-Z anchors (wrapping around
        /// the ring so there are no unresolved gaps).
        /// </summary>
        private static void AssignBoundaryZ(CADObjectData boundary,
            List<List<double[]>> breaklineChains, List<List<Point2D>> rings)
        {
            var verts = boundary.FlattenedVertices;
            if (verts.Count < 2) return;

            int n = verts.Count;

            // Use the Z values already drawn on the boundary — every original vertex is known.
            var ring = new List<(double x, double y, double z, bool known)>();
            for (int i = 0; i < n; i++)
                ring.Add((verts[i][0], verts[i][1], verts[i][2], true));

            // Find where breaklines cross boundary edges and insert a vertex at each
            // crossing point whose Z is interpolated from the breakline segment.
            var insertions = new List<(int afterIndex, double s, double z)>();

            foreach (var chain in breaklineChains)
            {
                for (int bi = 0; bi < chain.Count - 1; bi++)
                {
                    double[] A = chain[bi];
                    double[] B = chain[bi + 1];

                    for (int ei = 0; ei < ring.Count; ei++)
                    {
                        int ej = (ei + 1) % ring.Count;
                        var E0 = ring[ei];
                        var E1 = ring[ej];

                        double t, s;
                        if (!SegmentIntersectParam(
                                A[0], A[1], B[0], B[1],
                                E0.x, E0.y, E1.x, E1.y,
                                out t, out s))
                            continue;

                        if (t < 0 || t > 1 || s <= 0 || s >= 1) continue;

                        double zCross = A[2] + t * (B[2] - A[2]);
                        insertions.Add((ei, s, zCross));
                    }
                }
            }

            // Insert crossing vertices in reverse edge order so earlier indices stay valid
            var insertsByEdge = insertions
                .GroupBy(x => x.afterIndex)
                .OrderByDescending(g => g.Key);

            foreach (var edgeGroup in insertsByEdge)
            {
                int edgeIdx = edgeGroup.Key;
                var sorted = edgeGroup.OrderBy(x => x.s).ToList();
                var E0 = ring[edgeIdx];
                var E1 = ring[(edgeIdx + 1) % ring.Count];

                for (int k = sorted.Count - 1; k >= 0; k--)
                {
                    double sVal = sorted[k].s;
                    double ix = E0.x + sVal * (E1.x - E0.x);
                    double iy = E0.y + sVal * (E1.y - E0.y);
                    ring.Insert(edgeIdx + 1, (ix, iy, sorted[k].z, true));
                }
            }

            // All vertices are now known (original drawn Z or breakline crossing Z).
            // No interpolation pass needed.
            var newVerts = ring.Select(v => new double[] { v.x, v.y, v.z }).ToList();
            boundary.Vertices = newVerts;
            boundary.FlattenedVertices = newVerts;
        }

        /// <summary>
        /// Ray-casting point-in-polygon test (2D, works for convex and concave rings).
        /// </summary>
        private static bool PointInPolygon(double px, double py, List<Point2D> ring)
        {
            bool inside = false;
            int n = ring.Count;
            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                double xi = ring[i].X, yi = ring[i].Y;
                double xj = ring[j].X, yj = ring[j].Y;
                if (((yi > py) != (yj > py)) &&
                    (px < (xj - xi) * (py - yi) / (yj - yi) + xi))
                    inside = !inside;
            }
            return inside;
        }

        /// <summary>
        /// Clips an open polyline (3-D vertices) against boundary rings (2-D test).
        /// Returns one list per continuous inside span; Z is interpolated at each crossing.
        /// Handles re-entry: outside spans are fully discarded and each re-entry starts fresh.
        /// </summary>
        private static List<List<double[]>> ClipPolylineSegmentsToPolygon(
            List<double[]> verts, List<List<Point2D>> rings)
        {
            var result = new List<List<double[]>>();
            if (verts.Count < 2) return result;

            List<double[]> currentSeg = null;
            const double eps = 1e-10;

            for (int i = 0; i < verts.Count - 1; i++)
            {
                double[] A = verts[i];
                double[] B = verts[i + 1];
                bool aInside = rings.Any(ring => PointInPolygon(A[0], A[1], ring));

                // Collect all boundary crossings along A→B, sorted by t
                var crossings = new List<(double t, double[] pt)>();
                foreach (var ring in rings)
                {
                    int n = ring.Count;
                    for (int e = 0; e < n; e++)
                    {
                        Point2D P = ring[e];
                        Point2D Q = ring[(e + 1) % n];
                        double t, s;
                        if (!SegmentIntersectParam(
                                A[0], A[1], B[0], B[1],
                                P.X,  P.Y,  Q.X,  Q.Y,
                                out t, out s))
                            continue;
                        // Only accept crossings strictly inside the segment and on the edge
                        if (t <= eps || t >= 1.0 - eps) continue;
                        if (s < -eps || s > 1.0 + eps) continue;
                        crossings.Add((t, new double[]
                        {
                            A[0] + t * (B[0] - A[0]),
                            A[1] + t * (B[1] - A[1]),
                            A[2] + t * (B[2] - A[2])
                        }));
                    }
                }
                crossings.Sort((x, y) => x.t.CompareTo(y.t));

                // Open a segment at A if we're entering an inside span
                if (aInside && currentSeg == null)
                    currentSeg = new List<double[]> { A };

                bool currentlyInside = aInside;
                foreach (var crossing in crossings)
                {
                    if (currentlyInside)
                    {
                        // Going outside: close the current sub-segment
                        currentSeg.Add(crossing.pt);
                        if (currentSeg.Count >= 2)
                            result.Add(currentSeg);
                        currentSeg = null;
                    }
                    else
                    {
                        // Coming inside: start a new sub-segment
                        currentSeg = new List<double[]> { crossing.pt };
                    }
                    currentlyInside = !currentlyInside;
                }

                // Extend to B if we are still inside
                if (currentlyInside)
                {
                    if (currentSeg == null) currentSeg = new List<double[]> { A };
                    currentSeg.Add(B);
                }
            }

            // Close off any remaining open segment
            if (currentSeg != null && currentSeg.Count >= 2)
                result.Add(currentSeg);

            return result;
        }

        /// <summary>
        /// Computes parameters t (along A→B) and s (along P→Q) for the intersection of two 2-D segments.
        /// Returns false if lines are parallel.
        /// </summary>
        private static bool SegmentIntersectParam(
            double ax, double ay, double bx, double by,
            double px, double py, double qx, double qy,
            out double t, out double s)
        {
            double dx = bx - ax, dy = by - ay;
            double ex = qx - px, ey = qy - py;
            double denom = dx * ey - dy * ex;
            if (Math.Abs(denom) < 1e-12) { t = 0; s = 0; return false; }
            t = ((px - ax) * ey - (py - ay) * ex) / denom;
            s = ((px - ax) * dy - (py - ay) * dx) / denom;
            return true;
        }
    }
}
