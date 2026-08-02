using System;
using System.Collections.Generic;
using System.Linq;
using AutoCAD_NET_4_8_Framework;
using MyMiningPlugin.Models;

namespace MyMiningPlugin.Services
{
    // -------------------------------------------------------------------------
    // Result types
    // -------------------------------------------------------------------------

    public enum QualityCheckSeverity { Note, Warning, Error }

    public class QualityIssue
    {
        public string Check      { get; set; }
        public QualityCheckSeverity Severity { get; set; }
        public string Handle     { get; set; }
        public string Layer      { get; set; }
        public string EntityType { get; set; }
        public string Message    { get; set; }
        public double[] XY       { get; set; }   // first-vertex XY or point of interest
        public double?  Z        { get; set; }   // entity Z (where relevant)
        public object   Extra    { get; set; }   // check-specific payload
    }

    // -------------------------------------------------------------------------
    // Service
    // -------------------------------------------------------------------------

    public class DataQualityService
    {
        // --- Configurable thresholds (all in drawing units / metres) ---
        private const int    MinLayerSampleForCheck1 = 5;    // minimum entities per layer
        private const double LayerZOffsetThreshold   = 10.0; // m: layer median vs 0.0
        private const double SpikeMinAbsDeviation    = 1.0;  // m: absolute fence floor for check 2
        private const int    SpikeMinVertices        = 4;    // minimum vertices for check 2
        private const double ScatterLocalRadius      = 50.0; // m: spatial grid cell for check 3
        private const int    ScatterNearestK         = 8;    // neighbours for check 3
        private const double ScatterDeviationLimit   = 20.0; // m: max |Z - localMedian|
        private const double SlopeThreshold          = 10.0; // dimensionless: |?Z|/XY_dist
        private const double SlopeMinXYDistance      = 0.5;  // m: skip slope if closer than this
        private const double GapSnapEpsilon          = 0.01; // m: endpoints this close = connected
        private const double GapMaxDistance          = 5.0;  // m: near-miss search radius

        // -------------------------------------------------------------------------
        // Public entry point — run all checks against extracted CADObjectData
        // -------------------------------------------------------------------------

        /// <summary>
        /// Run all five data-quality checks.
        /// Pass the full extracted list for the surface BEFORE the boundary-clip step.
        /// <paramref name="terrainItems"/> = non-boundary, non-breakline, non-point items (V?a/Kh?i lines).
        /// </summary>
        public List<QualityIssue> RunAllChecks(
            List<CADObjectData> allItems,
            List<CADObjectData> terrainItems)
        {
            var issues = new List<QualityIssue>();

            var polylines  = allItems.Where(i => (i.FlattenedVertices?.Count ?? 0) > 1).ToList();
            var points     = allItems.Where(i => (i.FlattenedVertices?.Count ?? 0) == 1).ToList();
            var breaklines = allItems.Where(i => i.IsBreakline).ToList();

            issues.AddRange(Check1_UnsetZ(polylines));
            issues.AddRange(Check2_SpikeVertex(polylines));
            issues.AddRange(Check3_ScatterVsLocalTerrain(points, terrainItems));
            issues.AddRange(Check4_ScatterVsBreaklineSlope(points, breaklines));
            issues.AddRange(Check5_NearMissBreaklineEndpoints(breaklines));

            return issues;
        }

        // -------------------------------------------------------------------------
        // Check 1 — Unset Z (entity at Z = 0 when layer median is far from zero)
        // -------------------------------------------------------------------------

        private List<QualityIssue> Check1_UnsetZ(List<CADObjectData> polylines)
        {
            var issues = new List<QualityIssue>();

            // Pass 1: build per-layer median from entities that have ANY non-zero vertex
            var layerZValues = new Dictionary<string, List<double>>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in polylines)
            {
                var nonZeroZs = item.FlattenedVertices
                    .Where(v => Math.Abs(v[2]) > 1e-9)
                    .Select(v => v[2])
                    .ToList();
                if (nonZeroZs.Count == 0) continue;

                if (!layerZValues.ContainsKey(item.Layer))
                    layerZValues[item.Layer] = new List<double>();
                layerZValues[item.Layer].AddRange(nonZeroZs);
            }

            // Pass 2: for each entity where ALL vertices are Z=0, check against layer median
            foreach (var item in polylines)
            {
                bool allZero = item.FlattenedVertices.All(v => Math.Abs(v[2]) < 1e-9);
                if (!allZero) continue;

                if (!layerZValues.TryGetValue(item.Layer, out var layerZs))
                {
                    // Layer has no non-zero entities at all — skip silently
                    continue;
                }

                if (layerZs.Count < MinLayerSampleForCheck1)
                {
                    issues.Add(new QualityIssue
                    {
                        Check    = "Check1_UnsetZ",
                        Severity = QualityCheckSeverity.Note,
                        Handle   = item.Handle,
                        Layer    = item.Layer,
                        EntityType = item.ObjectType,
                        Message  = $"Lop '{item.Layer}' it hon {MinLayerSampleForCheck1} thuc the - bo qua kiem tra Z=0.",
                        XY = FirstXY(item)
                    });
                    continue;
                }

                double layerMedian = Median(layerZs);
                if (Math.Abs(layerMedian) > LayerZOffsetThreshold)
                {
                    issues.Add(new QualityIssue
                    {
                        Check    = "Check1_UnsetZ",
                        Severity = QualityCheckSeverity.Warning,
                        Handle   = item.Handle,
                        Layer    = item.Layer,
                        EntityType = item.ObjectType,
                        Message  = $"Z=0 nhung trung vi lop={layerMedian:F1}m. Co the cao do chua duoc gan.",
                        XY    = FirstXY(item),
                        Z     = 0.0,
                        Extra = new { LayerMedianZ = layerMedian }
                    });
                }
            }

            return issues;
        }

        // -------------------------------------------------------------------------
        // Check 2 — Spike vertex within a polyline
        // -------------------------------------------------------------------------

        private List<QualityIssue> Check2_SpikeVertex(List<CADObjectData> polylines)
        {
            var issues = new List<QualityIssue>();

            foreach (var item in polylines)
            {
                var verts = item.FlattenedVertices;
                if (verts.Count < SpikeMinVertices) continue;

                var zValues = verts.Select(v => v[2]).ToList();
                double median = Median(zValues);
                double iqr    = IQR(zValues);
                double fence  = Math.Max(3.0 * iqr, SpikeMinAbsDeviation);

                for (int i = 0; i < verts.Count; i++)
                {
                    double deviation = Math.Abs(verts[i][2] - median);
                    if (deviation > fence)
                    {
                        issues.Add(new QualityIssue
                        {
                            Check    = "Check2_SpikeVertex",
                            Severity = QualityCheckSeverity.Error,
                            Handle   = item.Handle,
                            Layer    = item.Layer,
                            EntityType = item.ObjectType,
                            Message  = $"Dinh {i}: Z={verts[i][2]:F2} lech {deviation:F1}m (trung vi {median:F2})",
                            XY    = new[] { verts[i][0], verts[i][1] },
                            Z     = verts[i][2],
                            Extra = new { VertexIndex = i, MedianZ = median, IQR = iqr, Fence = fence }
                        });
                    }
                }
            }

            return issues;
        }

        // -------------------------------------------------------------------------
        // Check 3 — Scatter point Z vs local terrain (B? m?t points)
        // -------------------------------------------------------------------------

        private List<QualityIssue> Check3_ScatterVsLocalTerrain(
            List<CADObjectData> scatterPoints,
            List<CADObjectData> terrainItems)
        {
            var issues = new List<QualityIssue>();
            if (scatterPoints.Count == 0 || terrainItems.Count == 0) return issues;

            // Build a flat list of terrain vertices for neighbour search
            var terrainVerts = terrainItems
                .Where(t => !t.IsBreakline)
                .SelectMany(t => t.FlattenedVertices)
                .Select(v => new double[] { v[0], v[1], v[2] })
                .ToList();

            if (terrainVerts.Count == 0) return issues;

            // Spatial grid: bucket terrain vertices into cells of ScatterLocalRadius × ScatterLocalRadius
            var grid = BuildSpatialGrid(terrainVerts, ScatterLocalRadius);

            foreach (var pt in scatterPoints)
            {
                if ((pt.FlattenedVertices?.Count ?? 0) == 0) continue;
                double px = pt.FlattenedVertices[0][0];
                double py = pt.FlattenedVertices[0][1];
                double pz = pt.FlattenedVertices[0][2];

                var neighbours = GetNearestFromGrid(grid, px, py, ScatterLocalRadius, ScatterNearestK);
                if (neighbours.Count < ScatterNearestK) continue; // not enough context

                double localMedian = Median(neighbours.Select(n => n[2]).ToList());
                double deviation   = Math.Abs(pz - localMedian);

                if (deviation > ScatterDeviationLimit)
                {
                    issues.Add(new QualityIssue
                    {
                        Check    = "Check3_ScatterVsTerrain",
                        Severity = QualityCheckSeverity.Error,
                        Handle   = pt.Handle,
                        Layer    = pt.Layer,
                        EntityType = pt.ObjectType,
                        Message  = $"Z={pz:F2} lech {deviation:F1}m so dia hinh lan can (trung vi {localMedian:F2})",
                        XY    = new[] { px, py },
                        Z     = pz,
                        Extra = new { LocalMedianZ = localMedian, Deviation = deviation }
                    });
                }
            }

            return issues;
        }

        // -------------------------------------------------------------------------
        // Check 4 — Scatter point Z vs nearest breakline slope
        // -------------------------------------------------------------------------

        private List<QualityIssue> Check4_ScatterVsBreaklineSlope(
            List<CADObjectData> scatterPoints,
            List<CADObjectData> breaklines)
        {
            var issues = new List<QualityIssue>();
            if (scatterPoints.Count == 0 || breaklines.Count == 0) return issues;

            // Flat list of breakline vertices
            var blVerts = breaklines
                .SelectMany(b => b.FlattenedVertices)
                .Select(v => new double[] { v[0], v[1], v[2] })
                .ToList();

            if (blVerts.Count == 0) return issues;

            foreach (var pt in scatterPoints)
            {
                if ((pt.FlattenedVertices?.Count ?? 0) == 0) continue;
                double px = pt.FlattenedVertices[0][0];
                double py = pt.FlattenedVertices[0][1];
                double pz = pt.FlattenedVertices[0][2];

                // Find nearest breakline vertex in XY
                double[] nearest   = null;
                double   nearestXY = double.MaxValue;
                foreach (var bv in blVerts)
                {
                    double d = XYDistance(px, py, bv[0], bv[1]);
                    if (d < nearestXY) { nearestXY = d; nearest = bv; }
                }

                if (nearest == null || nearestXY < SlopeMinXYDistance) continue;

                double slope = Math.Abs(pz - nearest[2]) / nearestXY;
                if (slope > SlopeThreshold)
                {
                    issues.Add(new QualityIssue
                    {
                        Check    = "Check4_ScatterVsBreaklineSlope",
                        Severity = QualityCheckSeverity.Error,
                        Handle   = pt.Handle,
                        Layer    = pt.Layer,
                        EntityType = pt.ObjectType,
                        Message  = $"Do doc={slope:F1} > {SlopeThreshold} den duong de gan nhat ({nearestXY:F1}m, DZ={Math.Abs(pz - nearest[2]):F1}m)",
                        XY    = new[] { px, py },
                        Z     = pz,
                        Extra = new
                        {
                            NearestBreaklineXY = new[] { nearest[0], nearest[1] },
                            NearestBreaklineZ  = nearest[2],
                            XYDistance         = nearestXY,
                            Slope              = slope,
                            Threshold          = SlopeThreshold
                        }
                    });
                }
            }

            return issues;
        }

        // -------------------------------------------------------------------------
        // Check 5 — Near-miss breakline endpoints
        // -------------------------------------------------------------------------

        private List<QualityIssue> Check5_NearMissBreaklineEndpoints(List<CADObjectData> breaklines)
        {
            var issues = new List<QualityIssue>();
            if (breaklines.Count < 2) return issues;

            // Collect all endpoints: (x, y, z, handle)  — start and end vertex of each breakline
            var endpoints = new List<(double x, double y, double z, string handle)>();
            foreach (var bl in breaklines)
            {
                var v = bl.FlattenedVertices;
                if (v.Count < 2) continue;
                endpoints.Add((v[0][0],             v[0][1],             v[0][2],             bl.Handle));
                endpoints.Add((v[v.Count - 1][0],   v[v.Count - 1][1],   v[v.Count - 1][2],   bl.Handle));
            }

            // O(n²) over endpoints — acceptable; breakline count is small
            for (int i = 0; i < endpoints.Count; i++)
            {
                for (int j = i + 1; j < endpoints.Count; j++)
                {
                    var a = endpoints[i];
                    var b = endpoints[j];

                    // Must belong to different polylines
                    if (a.handle == b.handle) continue;

                    double xyDist = XYDistance(a.x, a.y, b.x, b.y);

                    // Already connected (snapped) — not a gap
                    if (xyDist < GapSnapEpsilon) continue;

                    // Outside near-miss search radius
                    if (xyDist > GapMaxDistance) continue;

                    double dZ    = Math.Abs(a.z - b.z);
                    double slope = xyDist > 1e-12 ? dZ / xyDist : double.MaxValue;

                    bool severe = slope > SlopeThreshold;
                    issues.Add(new QualityIssue
                    {
                        Check    = "Check5_NearMissEndpoints",
                        Severity = severe ? QualityCheckSeverity.Error : QualityCheckSeverity.Warning,
                        Handle   = a.handle,
                        Layer    = null,
                        EntityType = "BREAKLINE",
                        Message  = severe
                            ? $"Ho duong de: cach {xyDist:F2}m, DZ={dZ:F2}m, doc={slope:F1} - gay lo CDT"
                            : $"Ho duong de: cach {xyDist:F2}m, DZ={dZ:F2}m, doc={slope:F1} - my quan",
                        XY    = new[] { a.x, a.y },
                        Z     = a.z,
                        Extra = new
                        {
                            HandleA      = a.handle,
                            HandleB      = b.handle,
                            EndpointA    = new[] { a.x, a.y, a.z },
                            EndpointB    = new[] { b.x, b.y, b.z },
                            XYGap        = xyDist,
                            DeltaZ       = dZ,
                            Slope        = slope,
                            IsSevere     = severe,
                            GapThreshold = GapMaxDistance,
                            SnapEpsilon  = GapSnapEpsilon
                        }
                    });
                }
            }

            return issues;
        }

        // -------------------------------------------------------------------------
        // Spatial grid helpers (for Check 3)
        // -------------------------------------------------------------------------

        private static Dictionary<(int, int), List<double[]>> BuildSpatialGrid(
            List<double[]> verts, double cellSize)
        {
            var grid = new Dictionary<(int, int), List<double[]>>();
            foreach (var v in verts)
            {
                var key = GridKey(v[0], v[1], cellSize);
                if (!grid.ContainsKey(key)) grid[key] = new List<double[]>();
                grid[key].Add(v);
            }
            return grid;
        }

        private static List<double[]> GetNearestFromGrid(
            Dictionary<(int, int), List<double[]>> grid,
            double px, double py, double cellSize, int k)
        {
            // Search the 3×3 neighbourhood of cells around the query point
            int cx = (int)Math.Floor(px / cellSize);
            int cy = (int)Math.Floor(py / cellSize);

            var candidates = new List<(double dist, double[] v)>();
            for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                if (!grid.TryGetValue((cx + dx, cy + dy), out var cell)) continue;
                foreach (var v in cell)
                    candidates.Add((XYDistance(px, py, v[0], v[1]), v));
            }

            return candidates
                .OrderBy(c => c.dist)
                .Take(k)
                .Select(c => c.v)
                .ToList();
        }

        private static (int, int) GridKey(double x, double y, double cellSize)
            => ((int)Math.Floor(x / cellSize), (int)Math.Floor(y / cellSize));

        // -------------------------------------------------------------------------
        // Statistical helpers
        // -------------------------------------------------------------------------

        private static double Median(List<double> values)
        {
            if (values.Count == 0) return 0;
            var sorted = values.OrderBy(v => v).ToList();
            int mid = sorted.Count / 2;
            return sorted.Count % 2 == 0
                ? (sorted[mid - 1] + sorted[mid]) / 2.0
                : sorted[mid];
        }

        private static double IQR(List<double> values)
        {
            if (values.Count < 4) return 0;
            var sorted = values.OrderBy(v => v).ToList();
            int n = sorted.Count;
            double q1 = Percentile(sorted, 25);
            double q3 = Percentile(sorted, 75);
            return q3 - q1;
        }

        private static double Percentile(List<double> sorted, double p)
        {
            double idx = (p / 100.0) * (sorted.Count - 1);
            int lo = (int)Math.Floor(idx);
            int hi = Math.Min(lo + 1, sorted.Count - 1);
            return sorted[lo] + (idx - lo) * (sorted[hi] - sorted[lo]);
        }

        // -------------------------------------------------------------------------
        // Geometry helpers
        // -------------------------------------------------------------------------

        private static double XYDistance(double x1, double y1, double x2, double y2)
        {
            double dx = x2 - x1, dy = y2 - y1;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private static double[] FirstXY(CADObjectData item)
        {
            var v = item.FlattenedVertices;
            return v != null && v.Count > 0
                ? new[] { v[0][0], v[0][1] }
                : new[] { 0.0, 0.0 };
        }
    }
}
