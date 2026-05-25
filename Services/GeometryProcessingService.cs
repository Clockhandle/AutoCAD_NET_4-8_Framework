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

        public GeometryProcessingService(AutoCADSelectionService selectionService)
        {
            _selectionService = selectionService;
        }

        /// <summary>
        /// Process geometry from SurfaceData and return CADObjectData list
        /// </summary>
        public Task<List<CADObjectData>> ProcessGeometryWithSmartZ(SurfaceData surface)
        {
            List<CADObjectData> result = new List<CADObjectData>();

            Document doc = AcApp.DocumentManager.MdiActiveDocument;
            if (doc == null) return Task.FromResult(result);

            Database db = doc.Database;

            // Note: References should be resolved before calling this method
            // This method assumes CurrentObjectId has already been populated

            int skippedCount = 0;
            int processedCount = 0;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // Iterate through both, while keeping track of which list they come from
                var combinedList = surface.SelectedGeometry.Select(g => new { GeoRef = g, IsBoundary = false, IsHole = false })
                    .Concat(surface.BoundaryGeometry.Select(g => new { GeoRef = g, IsBoundary = true, IsHole = false }))
                    .Concat(surface.HoleGeometry.Select(g => new { GeoRef = g, IsBoundary = false, IsHole = true }));

                foreach (var item in combinedList)
                {
                    var geoRef = item.GeoRef;

                    // Skip if not in current drawing
                    if (!geoRef.CurrentObjectId.HasValue || geoRef.CurrentObjectId.Value.IsNull)
                    {
                        skippedCount++;
                        continue;
                    }

                    try
                    {
                        Entity ent = tr.GetObject(geoRef.CurrentObjectId.Value, OpenMode.ForRead) as Entity;
                        if (ent == null)
                        {
                            skippedCount++;
                            continue;
                        }

                        CADObjectData cadData = new CADObjectData
                        {
                            GroupName = surface.ParentName,
                            ObjectType = surface.Type,
                            Layer = ent.Layer,
                            Handle = ent.Handle.ToString(),
                            IsBoundary = item.IsBoundary,
                            IsHole = item.IsHole,
                            Vertices = new List<double[]>(),
                            FlattenedVertices = new List<double[]>()
                        };

                        // Extract Color Information
                        ExtractColorInformation(cadData, ent, db, tr);

                        // Extract Geometry
                        List<Point3d> rawPoints = ExtractGeometry(cadData, ent);

                        // Populate Vertices
                        foreach (var pt in rawPoints)
                        {
                            cadData.Vertices.Add(new double[] { pt.X, pt.Y, pt.Z });
                            cadData.FlattenedVertices.Add(new double[] { pt.X, pt.Y, pt.Z });
                        }

                        result.Add(cadData);
                        processedCount++;
                    }
                    catch (Exception)
                    {
                        skippedCount++;
                    }
                }
                tr.Commit();
            }

            // Debug output
            if (skippedCount > 0)
            {
                System.Diagnostics.Debug.WriteLine($"ProcessGeometryWithSmartZ: Processed {processedCount}, Skipped {skippedCount} out of {surface.SelectedGeometry.Count + surface.BoundaryGeometry.Count} total");
            }

            return Task.FromResult(result);
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

        private List<Point3d> ExtractGeometry(CADObjectData cadData, Entity ent)
        {
            List<Point3d> rawPoints = new List<Point3d>();

            if (ent is Polyline pl)
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
    }
}
