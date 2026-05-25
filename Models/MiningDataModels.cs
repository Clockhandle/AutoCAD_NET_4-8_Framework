using Autodesk.AutoCAD.DatabaseServices;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MyMiningPlugin.Models
{
    // --- DATA STRUCTURES ---
    public class ViaData
    {
        public string Name { get; set; }
        public List<KhoiData> Blocks { get; set; } = new List<KhoiData>();
    }

    public class KhoiData
    {
        public string Name { get; set; }
        public SurfaceData Vach { get; set; }
        public SurfaceData Tru { get; set; }
    }

    public class SurfaceData
    {
        public string Type { get; set; } // "Vach", "Tru", or "Dut gay"
        public string ParentName { get; set; } // For context (e.g., "Via 8 - Khoi 1")
        public List<GeometryReference> SelectedGeometry { get; set; } = new List<GeometryReference>();
        public List<GeometryReference> BoundaryGeometry { get; set; } = new List<GeometryReference>();
        // Hole polygons: closed polylines that punch a void through the mesh interior
        public List<GeometryReference> HoleGeometry { get; set; } = new List<GeometryReference>();
    }

    // Persistent reference to CAD geometry across different drawings
    public class GeometryReference
    {
        public string Handle { get; set; }           // "1A4F" (unique ID)
        public string SourceDwgPath { get; set; }    // "C:\\Projects\\vach.dwg"
        public string SourceDwgName { get; set; }    // "vach.dwg" (for display)
        public string Layer { get; set; }            // Layer name
        public string EntityType { get; set; }       // "LWPOLYLINE", "LINE"
        public int VertexCount { get; set; }         // Number of vertices
        
        // Runtime property: resolved ObjectId (null if not in current drawing)
        [JsonIgnore]
        public ObjectId? CurrentObjectId { get; set; }
    }

    public class PointData
    {
        public string Handle { get; set; }
        public string Layer { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }

    public class BeMatData
    {
        public string Name { get; set; }
        public List<PointData> Points { get; set; } = new List<PointData>();
    }

    public class FaultData
    {
        public string Name { get; set; }
        public SurfaceData Surface { get; set; }
    }

    public class RockData
    {
        public string Name { get; set; }
        public SurfaceData Surface { get; set; }
    }

    public class BoreholeData
    {
        public string Name { get; set; }
        public string ExcelFilePath { get; set; }
        
        // Coordinates from THIET DO (X, Y, Z)
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        
        public List<DepthInterval> Intervals { get; set; } = new List<DepthInterval>();
        public List<SurveyReading> Trajectory { get; set; } = new List<SurveyReading>();
    }

    public class DepthInterval
    {
        public double From { get; set; }
        public double To { get; set; }
        public string SeamName { get; set; }
    }

    public class SurveyReading
    {
        public double DepthRange { get; set; } // CS®o
        public double DO { get; set; }
        public double PVI { get; set; } // PHAN VI
    }

    // Project container for all mining data
    public class MiningProject
    {
        public List<ViaData> Vias { get; set; } = new List<ViaData>();
        public List<BeMatData> BeMats { get; set; } = new List<BeMatData>();
        public List<FaultData> Faults { get; set; } = new List<FaultData>();
        public List<RockData> Rocks { get; set; } = new List<RockData>();
        public List<BoreholeData> Boreholes { get; set; } = new List<BoreholeData>();
    }
}
