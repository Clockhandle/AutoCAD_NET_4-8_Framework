using System;
using System.IO; 
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AutoCAD_NET_4_8_Framework
{
    public partial class Form1 : Form
    {
        private List<ObjectId> _selectedObjectIds = new List<ObjectId>();

        public Form1()
        {
            InitializeComponent();
        }

        private void OnSelectObject_Click(object sender, EventArgs e)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            this.Hide();

            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = "\nSelect objects (drag to window select): ";

            PromptSelectionResult res = ed.GetSelection(opt);

            this.Show();

            if (res.Status == PromptStatus.OK)
            {
                SelectionSet ss = res.Value;
                _selectedObjectIds = new List<ObjectId>(ss.GetObjectIds());

                MessageBox.Show($"Selected {_selectedObjectIds.Count} objects.");
            }
            else
            {
                _selectedObjectIds.Clear();
                MessageBox.Show("Selection cancelled.");
            }
        }

        private void OnConfirmSelectedObject_Click(object sender, EventArgs e)
        {
            if (_selectedObjectIds.Count == 0)
            {
                MessageBox.Show("Please select objects first.");
                return;
            }

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (DocumentLock docLock = doc.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    foreach (ObjectId id in _selectedObjectIds)
                    {
                        if (id.IsErased) continue;

                        Entity ent = tr.GetObject(id, OpenMode.ForWrite) as Entity;

                        if (ent != null)
                        {
                            // Apply changes to ALL selected objects
                            ent.ColorIndex = 1; // Red
                            ent.TransformBy(Matrix3d.Displacement(new Vector3d(0, 0, 10)));
                        }
                    }

                    tr.Commit();
                }
            }

            // Refresh only once at the end
            doc.Editor.Regen();

            _selectedObjectIds.Clear();
            MessageBox.Show("Processing Complete.");
        }
        private void OnExportToCSV_Click(object sender, EventArgs e)
        {
            if (_selectedObjectIds.Count == 0)
            {
                MessageBox.Show("No objects selected.");
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "CSV File|*.csv";
            saveDialog.FileName = "Detailed_Export_" + DateTime.Now.ToString("yyyyMMdd");
            if (saveDialog.ShowDialog() != DialogResult.OK) return;

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            try
            {
                using (StreamWriter sw = new StreamWriter(saveDialog.FileName))
                {
                    // 1. Write Header
                    sw.WriteLine("Handle,Layer,Type,BlockName,Attribute_Data,Vertex_Index,X,Y,Z,Bulge,SegmentType");

                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        foreach (ObjectId id in _selectedObjectIds)
                        {
                            if (id.IsErased) continue;
                            Entity ent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                            if (ent == null) continue;

                            string handle = ent.Handle.ToString();
                            string layer = ent.Layer;
                            string type = ent.GetType().Name;

                            // --- CASE 1: POLYLINE (Detailed Geometry) ---
                            if (ent is Polyline pl)
                            {
                                int vertices = pl.NumberOfVertices;
                                for (int i = 0; i < vertices; i++)
                                {
                                    // Geometry details
                                    Point3d pt = pl.GetPoint3dAt(i);
                                    double bulge = pl.GetBulgeAt(i);

                                    // Determine segment type (Line vs Arc) based on bulge
                                    string segType = (bulge != 0) ? "ARC" : "LINE";

                                    // Format: Handle, Layer, Type, BlockName, Attrs, V_Index, X, Y, Z, Bulge, SegType
                                    sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6:F4},{7:F4},{8:F4},{9:F4},{10}",
                                        handle, layer, "Polyline", "N/A", "N/A",
                                        i, pt.X, pt.Y, pt.Z, bulge, segType));
                                }
                            }
                            // --- CASE 2: BLOCK REFERENCE (Attributes/Tags) ---
                            else if (ent is BlockReference blk)
                            {
                                string blockName = blk.Name;
                                if (blk.IsDynamicBlock)
                                {
                                    var btr = tr.GetObject(blk.DynamicBlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                                    blockName = btr.Name;
                                }

                                // Collect Attributes (Critical for Databases!)
                                string attrData = "";
                                if (blk.AttributeCollection.Count > 0)
                                {
                                    foreach (ObjectId attId in blk.AttributeCollection)
                                    {
                                        AttributeReference attRef = tr.GetObject(attId, OpenMode.ForRead) as AttributeReference;
                                        if (attRef != null)
                                        {
                                            // Format: TAG=VALUE | TAG2=VALUE2
                                            attrData += $"{attRef.Tag}={attRef.TextString}|";
                                        }
                                    }
                                }

                                // Write generic row for the block position
                                sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6:F4},{7:F4},{8:F4},{9},{10}",
                                    handle, layer, "Block", blockName, attrData,
                                    0, blk.Position.X, blk.Position.Y, blk.Position.Z, 0, "N/A"));
                            }
                            // --- CASE 3: OTHERS (Lines, Circles) ---
                            else
                            {
                                // Use Bounds (Extents) for generic location
                                double x = 0, y = 0, z = 0;
                                if (ent.Bounds.HasValue)
                                {
                                    x = ent.Bounds.Value.MinPoint.X;
                                    y = ent.Bounds.Value.MinPoint.Y;
                                    z = ent.Bounds.Value.MinPoint.Z;
                                }

                                sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6:F4},{7:F4},{8:F4},{9},{10}",
                                    handle, layer, type, "N/A", "N/A",
                                    0, x, y, z, 0, "N/A"));
                            }
                        }
                        tr.Commit();
                    }
                }
                MessageBox.Show("Detailed Export Complete!");
                System.Diagnostics.Process.Start("explorer.exe", "/select," + saveDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        //Helper functions
        private void Form1_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
        }
    }
}