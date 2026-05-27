using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using MyMiningPlugin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AcApp = Autodesk.AutoCAD.ApplicationServices.Application;

namespace MyMiningPlugin.Services
{
    /// <summary>
    /// Handles AutoCAD selection and geometry processing
    /// </summary>
    public class AutoCADSelectionService
    {
        /// <summary>
        /// Select lines/polylines from AutoCAD and add to surface data
        /// </summary>
        public void SelectLinesFromAutoCAD(SurfaceData surface)
        {
            Document doc = AcApp.DocumentManager.MdiActiveDocument;
            if (doc == null)
            {
                MessageBox.Show("No active AutoCAD document.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Database db = doc.Database;
            Editor ed = doc.Editor;

            // Filter: Only allow Polylines and Lines
            SelectionFilter filter = new SelectionFilter(new TypedValue[] {
                new TypedValue((int)DxfCode.Start, "LWPOLYLINE,POLYLINE,LINE")
            });

            PromptSelectionOptions opts = new PromptSelectionOptions();
            opts.MessageForAdding = $"\nChọn các đường địa hình cho {surface.Type}: ";

            PromptSelectionResult res = ed.GetSelection(opts, filter);

            if (res.Status == PromptStatus.OK)
            {
                SelectionSet ss = res.Value;
                int newCount = 0;

                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    foreach (SelectedObject obj in ss)
                    {
                        Entity ent = tr.GetObject(obj.ObjectId, OpenMode.ForRead) as Entity;
                        if (ent == null) continue;

                        // Check if already exists (by Handle)
                        string handle = ent.Handle.ToString();
                        if (surface.SelectedGeometry.Any(g => g.Handle == handle))
                            continue;

                        // Create persistent reference
                        GeometryReference geoRef = new GeometryReference
                        {
                            Handle = handle,
                            SourceDwgPath = db.Filename ?? "Unsaved Drawing",
                            SourceDwgName = string.IsNullOrEmpty(db.Filename) ? "Unsaved" : System.IO.Path.GetFileName(db.Filename),
                            Layer = ent.Layer,
                            EntityType = ent.GetRXClass().Name,
                            CurrentObjectId = obj.ObjectId
                        };

                        // Get vertex count
                        if (ent is Polyline pl)
                            geoRef.VertexCount = pl.NumberOfVertices;
                        else if (ent is Line)
                            geoRef.VertexCount = 2;

                        surface.SelectedGeometry.Add(geoRef);
                        newCount++;
                    }
                    tr.Commit();
                }

                MessageBox.Show($"Đã thêm {newCount} đường vào danh sách.\nTổng: {surface.SelectedGeometry.Count} lines", 
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Select boundary lines/polylines from AutoCAD and add to surface data
        /// </summary>
        public void SelectBorderlinesFromAutoCAD(SurfaceData surface)
        {
            Document doc = AcApp.DocumentManager.MdiActiveDocument;
            if (doc == null)
            {
                MessageBox.Show("No active AutoCAD document.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Database db = doc.Database;
            Editor ed = doc.Editor;

            SelectionFilter filter = new SelectionFilter(new TypedValue[] {
                new TypedValue((int)DxfCode.Start, "LWPOLYLINE,POLYLINE,LINE")
            });

            PromptSelectionOptions opts = new PromptSelectionOptions();
            opts.MessageForAdding = $"\nChọn các đường biên cho {surface.Type}: ";

            PromptSelectionResult res = ed.GetSelection(opts, filter);

            if (res.Status == PromptStatus.OK)
            {
                SelectionSet ss = res.Value;
                int newCount = 0;

                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    foreach (SelectedObject obj in ss)
                    {
                        Entity ent = tr.GetObject(obj.ObjectId, OpenMode.ForRead) as Entity;
                        if (ent == null) continue;

                        string handle = ent.Handle.ToString();
                        if (surface.BoundaryGeometry.Any(g => g.Handle == handle))
                            continue;

                        GeometryReference geoRef = new GeometryReference
                        {
                            Handle = handle,
                            SourceDwgPath = db.Filename ?? "Unsaved Drawing",
                            SourceDwgName = string.IsNullOrEmpty(db.Filename) ? "Unsaved" : System.IO.Path.GetFileName(db.Filename),
                            Layer = ent.Layer,
                            EntityType = ent.GetRXClass().Name,
                            CurrentObjectId = obj.ObjectId
                        };

                        if (ent is Polyline pl)
                            geoRef.VertexCount = pl.NumberOfVertices;
                        else if (ent is Line)
                            geoRef.VertexCount = 2;

                        surface.BoundaryGeometry.Add(geoRef);
                        newCount++;
                    }
                    tr.Commit();
                }

                MessageBox.Show($"Đã thêm {newCount} đường biên vào danh sách.\nTổng: {surface.BoundaryGeometry.Count} boundary lines", 
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Select hole polylines from AutoCAD and add to surface HoleGeometry
        /// </summary>
        public void SelectHolesFromAutoCAD(SurfaceData surface)
        {
            Document doc = AcApp.DocumentManager.MdiActiveDocument;
            if (doc == null)
            {
                MessageBox.Show("No active AutoCAD document.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Database db = doc.Database;
            Editor ed = doc.Editor;

            SelectionFilter filter = new SelectionFilter(new TypedValue[] {
                new TypedValue((int)DxfCode.Start, "LWPOLYLINE,POLYLINE,LINE")
            });

            PromptSelectionOptions opts = new PromptSelectionOptions();
            opts.MessageForAdding = $"\nChọn các đường hố (hole) cho {surface.Type}: ";

            PromptSelectionResult res = ed.GetSelection(opts, filter);

            if (res.Status == PromptStatus.OK)
            {
                SelectionSet ss = res.Value;
                int newCount = 0;

                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    foreach (SelectedObject obj in ss)
                    {
                        Entity ent = tr.GetObject(obj.ObjectId, OpenMode.ForRead) as Entity;
                        if (ent == null) continue;

                        string handle = ent.Handle.ToString();
                        if (surface.HoleGeometry.Any(g => g.Handle == handle))
                            continue;

                        GeometryReference geoRef = new GeometryReference
                        {
                            Handle = handle,
                            SourceDwgPath = db.Filename ?? "Unsaved Drawing",
                            SourceDwgName = string.IsNullOrEmpty(db.Filename) ? "Unsaved" : System.IO.Path.GetFileName(db.Filename),
                            Layer = ent.Layer,
                            EntityType = ent.GetRXClass().Name,
                            CurrentObjectId = obj.ObjectId
                        };

                        if (ent is Polyline pl)
                            geoRef.VertexCount = pl.NumberOfVertices;
                        else if (ent is Line)
                            geoRef.VertexCount = 2;

                        surface.HoleGeometry.Add(geoRef);
                        newCount++;
                    }
                    tr.Commit();
                }

                MessageBox.Show($"Đã thêm {newCount} đường hố vào danh sách.\nTổng: {surface.HoleGeometry.Count} hole lines",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Select Point entities from AutoCAD for Bề mặt surface data
        /// </summary>
        public void SelectPointsFromAutoCAD(SurfaceData surface)
        {
            Document doc = AcApp.DocumentManager.MdiActiveDocument;
            if (doc == null)
            {
                MessageBox.Show("No active AutoCAD document.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Database db = doc.Database;
            Editor ed = doc.Editor;

            SelectionFilter filter = new SelectionFilter(new TypedValue[] {
                new TypedValue((int)DxfCode.Start, "POINT")
            });

            PromptSelectionOptions opts = new PromptSelectionOptions();
            opts.MessageForAdding = $"\nChọn các điểm cho {surface.ParentName}: ";

            PromptSelectionResult res = ed.GetSelection(opts, filter);

            if (res.Status == PromptStatus.OK)
            {
                int newCount = 0;

                // Build a HashSet for O(1) duplicate checking instead of O(n) Any()
                var existingHandles = new System.Collections.Generic.HashSet<string>(
                    surface.SelectedGeometry.Select(g => g.Handle));

                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    foreach (SelectedObject obj in res.Value)
                    {
                        Entity ent = tr.GetObject(obj.ObjectId, OpenMode.ForRead) as Entity;
                        if (ent == null) continue;

                        string handle = ent.Handle.ToString();
                        if (!existingHandles.Add(handle))
                            continue;

                        GeometryReference geoRef = new GeometryReference
                        {
                            Handle = handle,
                            SourceDwgPath = db.Filename ?? "Unsaved Drawing",
                            SourceDwgName = string.IsNullOrEmpty(db.Filename) ? "Unsaved" : System.IO.Path.GetFileName(db.Filename),
                            Layer = ent.Layer,
                            EntityType = "POINT",
                            VertexCount = 1,
                            CurrentObjectId = obj.ObjectId
                        };

                        surface.SelectedGeometry.Add(geoRef);
                        newCount++;
                    }
                    tr.Commit();
                }

                MessageBox.Show($"Đã thêm {newCount} điểm vào danh sách.\nTổng: {surface.SelectedGeometry.Count} điểm",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Select breaklines from AutoCAD and add to BreaklineGeometry
        /// </summary>
        public void SelectBreakLinesFromAutoCAD(SurfaceData surface)
        {
            Document doc = AcApp.DocumentManager.MdiActiveDocument;
            if (doc == null)
            {
                MessageBox.Show("No active AutoCAD document.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Database db = doc.Database;
            Editor ed = doc.Editor;

            SelectionFilter filter = new SelectionFilter(new TypedValue[] {
                new TypedValue((int)DxfCode.Start, "LWPOLYLINE,POLYLINE,LINE")
            });

            PromptSelectionOptions opts = new PromptSelectionOptions();
            opts.MessageForAdding = $"\nChọn các đường đê (breakline) cho {surface.ParentName}: ";

            PromptSelectionResult res = ed.GetSelection(opts, filter);

            if (res.Status == PromptStatus.OK)
            {
                int newCount = 0;

                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    foreach (SelectedObject obj in res.Value)
                    {
                        Entity ent = tr.GetObject(obj.ObjectId, OpenMode.ForRead) as Entity;
                        if (ent == null) continue;

                        string handle = ent.Handle.ToString();
                        if (surface.BreaklineGeometry.Any(g => g.Handle == handle))
                            continue;

                        GeometryReference geoRef = new GeometryReference
                        {
                            Handle = handle,
                            SourceDwgPath = db.Filename ?? "Unsaved Drawing",
                            SourceDwgName = string.IsNullOrEmpty(db.Filename) ? "Unsaved" : System.IO.Path.GetFileName(db.Filename),
                            Layer = ent.Layer,
                            EntityType = ent.GetRXClass().Name,
                            CurrentObjectId = obj.ObjectId
                        };

                        if (ent is Polyline pl)
                            geoRef.VertexCount = pl.NumberOfVertices;
                        else if (ent is Line)
                            geoRef.VertexCount = 2;

                        surface.BreaklineGeometry.Add(geoRef);
                        newCount++;
                    }
                    tr.Commit();
                }

                MessageBox.Show($"Đã thêm {newCount} đường đê vào danh sách.\nTổng: {surface.BreaklineGeometry.Count} breaklines",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Resolve GeometryReferences to ObjectIds in the current drawing
        /// </summary>
        public void ResolveCurrentDrawingReferences(SurfaceData surface)
        {
            try
            {
                Document doc = AcApp.DocumentManager.MdiActiveDocument;
                if (doc == null)
                {
                    System.Diagnostics.Debug.WriteLine("ResolveCurrentDrawingReferences: No active document!");
                    return;
                }

                Database db = doc.Database;
                string currentDwg = string.IsNullOrEmpty(db.Filename) ? "Unsaved" : System.IO.Path.GetFileName(db.Filename);
                
                int successCount = 0;
                int failCount = 0;

                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                foreach (var geoRef in surface.SelectedGeometry.Concat(surface.BoundaryGeometry).Concat(surface.HoleGeometry).Concat(surface.BreaklineGeometry))
                    {
                        try
                        {
                            long handleValue = Convert.ToInt64(geoRef.Handle, 16);
                            Handle handle = new Handle(handleValue);
                            
                            // Use TryGetObjectId to prevent massive performance hits from exceptions
                            if (db.TryGetObjectId(handle, out ObjectId id) && !id.IsNull && id.IsValid)
                            {
                                // Verify the object actually exists and can be opened
                                Entity ent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                                if (ent != null)
                                {
                                    geoRef.CurrentObjectId = id;
                                    successCount++;
                                }
                                else
                                {
                                    geoRef.CurrentObjectId = null;
                                    failCount++;
                                }
                            }
                            else
                            {
                                geoRef.CurrentObjectId = null;
                                failCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            geoRef.CurrentObjectId = null;
                            failCount++;
                            System.Diagnostics.Debug.WriteLine($"Failed to resolve handle {geoRef.Handle}: {ex.Message}");
                        }
                    }
                    tr.Commit();
                }
                
                System.Diagnostics.Debug.WriteLine($"ResolveCurrentDrawingReferences [{surface.Type}]: Success={successCount}, Failed={failCount}, CurrentDwg={currentDwg}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ResolveCurrentDrawingReferences ERROR: {ex.Message}");
            }
        }
    }
}
