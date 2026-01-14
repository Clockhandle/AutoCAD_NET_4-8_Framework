using Newtonsoft.Json;
using System.Collections.Generic;

namespace AutoCAD_NET_4_8_Framework
{
    public class CADObjectData
    {
        // --- Base Properties ---
        public string GroupName { get; set; }
        public string Handle { get; set; }
        public string ObjectType { get; set; }
        public string Layer { get; set; }

        // --- Linear (Line, Polyline) ---
        public double[] StartPoint { get; set; }
        public double[] EndPoint { get; set; }
        public List<double[]> Vertices { get; set; }
        public bool? IsClosed { get; set; }

        // --- Curved Polyline Support ---
        // A "Bulge" is how AutoCAD defines a curve between two polyline points.
        // 0 = Straight, != 0 means curved.
        public List<double> Bulges { get; set; }

        // --- Circular/Arc (Circle, Arc) ---
        public double[] CenterPoint { get; set; }
        public double? Radius { get; set; }

        // Specific to Arcs
        public double? StartAngle { get; set; }
        public double? EndAngle { get; set; }
        public double? TotalAngle { get; set; }

        // --- Elliptical (Ellipse) ---
        public double[] MajorAxis { get; set; } // Vector direction of long axis
        public double[] MinorAxis { get; set; } // Vector direction of short axis
        public double? RadiusRatio { get; set; } // Short axis / Long axis ratio

        // --- Complex Curves (Spline) ---
        public List<double[]> ControlPoints { get; set; } // The "magnet" points pulling the curve
        public List<double[]> FitPoints { get; set; }     // The points the curve actually passes through
        public int? Degree { get; set; }                  // Smoothness level
        public bool? IsRational { get; set; }             // True if it has Weights

        // --- Generic ---
        public double? Rotation { get; set; } // Useful for Text, Blocks, etc.
    }
}
