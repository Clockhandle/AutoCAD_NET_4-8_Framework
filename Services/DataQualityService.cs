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
        // Below this, two lines are considered parallel (sin of the angle between them).
        private const double ParallelSinTolerance = 1e-6;
        // Below this perpendicular distance (metres), a "parallel" pair is treated as collinear.
        private const double CollinearDistanceTolerance = 1e-6;
        // Parametric slack (0..1) so touching exactly at an endpoint isn't lost to float noise.
        private const double ParamEpsilon = 1e-9;
        // Minimum segment length (metres) to bother testing — drops degenerate/zero-length edges.
        private const double MinSegmentLength = 1e-9;

        // -------------------------------------------------------------------------
        // Public entry point — detect XY crossings between polylines (top-view only)
        // -------------------------------------------------------------------------

        /// <summary>
        /// Detects where polyline edges cross each other in the XY plane (Z ignored —
        /// i.e. looking straight down from the top view). This is the failure mode that
        /// most consistently breaks a constrained Delaunay triangulation: an edge that
        /// was meant to meet the next polyline instead slightly overshoots/undershoots
        /// and cuts across it instead of landing exactly on a shared vertex.
        /// A true end-to-end connection (two segment endpoints coinciding) is NOT an
        /// error; anything else where two edges touch or overlap in XY is.
        /// </summary>
        public List<QualityIssue> RunAllChecks(List<CADObjectData> allItems)
        {
            var polylines = allItems.Where(i => (i.FlattenedVertices?.Count ?? 0) > 1).ToList();
            return CheckXYCrossings(polylines);
        }

        private List<QualityIssue> CheckXYCrossings(List<CADObjectData> polylines)
        {
            var issues = new List<QualityIssue>();

            // Flatten every polyline into its individual XY edges.
            var segments = new List<(CADObjectData item, int index, double[] p1, double[] p2)>();
            foreach (var item in polylines)
            {
                var v = item.FlattenedVertices;
                int count = v.Count;
                int edgeCount = item.IsClosed == true ? count : count - 1;

                for (int i = 0; i < edgeCount; i++)
                {
                    double[] p1 = v[i];
                    double[] p2 = v[(i + 1) % count];
                    double dx = p2[0] - p1[0], dy = p2[1] - p1[1];
                    if (dx * dx + dy * dy < MinSegmentLength * MinSegmentLength) continue;

                    segments.Add((item, i, p1, p2));
                }
            }

            for (int a = 0; a < segments.Count; a++)
            {
                for (int b = a + 1; b < segments.Count; b++)
                {
                    var segA = segments[a];
                    var segB = segments[b];

                    if (!SegmentsIntersectXY(segA.p1, segA.p2, segB.p1, segB.p2,
                            out double ix, out double iy, out bool atSharedEndpoint))
                        continue;

                    if (atSharedEndpoint) continue; // valid end-to-end connection

                    issues.Add(new QualityIssue
                    {
                        Check    = "CheckXYCrossing",
                        Severity = QualityCheckSeverity.Error,
                        Handle   = segA.item.Handle,
                        Layer    = segA.item.Layer,
                        EntityType = segA.item.ObjectType,
                        Message  = $"Giao cat XY voi {segB.item.Handle} [{segB.item.Layer}] " +
                                   $"(canh {segA.index}<->canh {segB.index}) - co the vo Constrained Delaunay Triangulation.",
                        XY    = new[] { ix, iy },
                        Extra = new
                        {
                            OtherHandle    = segB.item.Handle,
                            OtherLayer     = segB.item.Layer,
                            SegmentIndexA  = segA.index,
                            SegmentIndexB  = segB.index
                        }
                    });
                }
            }

            return issues;
        }

        // -------------------------------------------------------------------------
        // Geometry: 2D segment intersection (Z ignored)
        // -------------------------------------------------------------------------

        /// <summary>
        /// Tests whether segments (p1-p2) and (p3-p4) intersect in the XY plane.
        /// <paramref name="atSharedEndpoint"/> is true only when the crossing point is
        /// exactly an endpoint of BOTH segments — i.e. two polylines legitimately
        /// meeting end-to-end. Any other touch/overlap (a T-intersection, an edge
        /// punching through the interior of another, or a collinear overlap) is a
        /// real crossing.
        /// </summary>
        private static bool SegmentsIntersectXY(
            double[] p1, double[] p2, double[] p3, double[] p4,
            out double ix, out double iy, out bool atSharedEndpoint)
        {
            ix = 0; iy = 0; atSharedEndpoint = false;

            double d1x = p2[0] - p1[0], d1y = p2[1] - p1[1];
            double d2x = p4[0] - p3[0], d2y = p4[1] - p3[1];

            double len1 = Math.Sqrt(d1x * d1x + d1y * d1y);
            double len2 = Math.Sqrt(d2x * d2x + d2y * d2y);
            if (len1 < MinSegmentLength || len2 < MinSegmentLength) return false;

            double ex = p3[0] - p1[0], ey = p3[1] - p1[1];

            double denom = d1x * d2y - d1y * d2x;
            double denomNorm = denom / (len1 * len2); // sin of angle between the two lines

            if (Math.Abs(denomNorm) < ParallelSinTolerance)
            {
                // Parallel lines — only relevant if they're also collinear and overlap.
                double perpDist = (ex * d1y - ey * d1x) / len1;
                if (Math.Abs(perpDist) > CollinearDistanceTolerance) return false; // parallel, offset

                double len1Sq = len1 * len1;
                double t3 = (ex * d1x + ey * d1y) / len1Sq;
                double fx = p4[0] - p1[0], fy = p4[1] - p1[1];
                double t4 = (fx * d1x + fy * d1y) / len1Sq;

                double tLo = Math.Min(t3, t4), tHi = Math.Max(t3, t4);
                double oLo = Math.Max(0.0, tLo), oHi = Math.Min(1.0, tHi);

                // A pure point-touch (oHi ~ oLo) is just a shared endpoint on a collinear
                // run — not a crossing. A genuine overlap span always counts as an error.
                if (oHi - oLo < ParamEpsilon) return false;

                double tm = (oLo + oHi) / 2.0;
                ix = p1[0] + tm * d1x;
                iy = p1[1] + tm * d1y;
                atSharedEndpoint = false;
                return true;
            }

            double t = (ex * d2y - ey * d2x) / denom;
            double u = (ex * d1y - ey * d1x) / denom;

            if (t < -ParamEpsilon || t > 1 + ParamEpsilon || u < -ParamEpsilon || u > 1 + ParamEpsilon)
                return false;

            ix = p1[0] + t * d1x;
            iy = p1[1] + t * d1y;

            bool tAtEnd = t <= ParamEpsilon || t >= 1 - ParamEpsilon;
            bool uAtEnd = u <= ParamEpsilon || u >= 1 - ParamEpsilon;
            atSharedEndpoint = tAtEnd && uAtEnd;

            return true;
        }
    }
}
