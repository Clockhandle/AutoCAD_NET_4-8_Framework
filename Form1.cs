using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO; 
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
namespace AutoCAD_NET_4_8_Framework
{
    public partial class Form1 : Form
    {
        private List<ObjectId> _selectedObjectIds_A = new List<ObjectId>();
        private List<ObjectId> _selectedObjectIds_B = new List<ObjectId>();
        private List<ObjectId> _selectedObjectIds_C = new List<ObjectId>();
        public Form1()
        {
            InitializeComponent();
        }

        private void OnSelectObject_A(object sender, EventArgs e)
        {
            List<ObjectId> res = PromptUserForSelection("Select objects for Group A: ");
            if (res.Count > 0)
            {
                _selectedObjectIds_A = res;
                MessageBox.Show($"Selected {_selectedObjectIds_A.Count} objects for Group A.");
            }
        }
        private void OnSelectObject_B(object sender, EventArgs e) 
        {
            List<ObjectId> res = PromptUserForSelection("Select objects for Group B: ");
            if (res.Count > 0)
            {
                _selectedObjectIds_B = res;
                MessageBox.Show($"Selected {_selectedObjectIds_B.Count} objects for Group B.");
            }
        }
        private void OnSelectObject_C(object sender, EventArgs e) 
        {
            List<ObjectId> res = PromptUserForSelection("Select objects for Group C: ");
            if (res.Count > 0)
            {
                _selectedObjectIds_C = res;
                MessageBox.Show($"Selected {_selectedObjectIds_C.Count} objects for Group C.");
            }
        }
        private void OnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OnExportToJSON_Click(object sender, EventArgs e)
        {
            if (_selectedObjectIds_A.Count == 0 && _selectedObjectIds_B.Count == 0 && _selectedObjectIds_C.Count == 0)
            {
                MessageBox.Show("Please select objects first.");
                return;
            }

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            List<CADObjectData> cadObjectsData_A = new List<CADObjectData>();
            List<CADObjectData> cadObjectsData_B = new List<CADObjectData>();
            List<CADObjectData> cadObjectsData_C = new List<CADObjectData>();
            using (DocumentLock docLock = doc.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    List<CADObjectData> ExtractDataFromLists(List<ObjectId> ids, string groupName)
                    {
                        List<CADObjectData> tempList = new List<CADObjectData>();
                        foreach (ObjectId id in ids)
                        {
                            if (id.IsErased) continue;
                            Entity ent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                            if (ent != null)
                            {
                                CADObjectData cadData = new CADObjectData
                                {
                                    GroupName = groupName,
                                    Handle = ent.Handle.ToString(),
                                    ObjectType = ent.GetType().Name,
                                    Layer = ent.Layer
                                };
                                if (ent is Line line)
                                {
                                    cadData.StartPoint = new double[] { line.StartPoint.X, line.StartPoint.Y, line.StartPoint.Z };
                                    cadData.EndPoint = new double[] { line.EndPoint.X, line.EndPoint.Y, line.EndPoint.Z };
                                }
                                else if (ent is Circle circle)
                                {
                                    cadData.CenterPoint = new double[] { circle.Center.X, circle.Center.Y, circle.Center.Z };
                                    cadData.Radius = circle.Radius;
                                }
                                if (ent is Arc arc)
                                {
                                    cadData.CenterPoint = new double[] { arc.Center.X, arc.Center.Y, arc.Center.Z };
                                    cadData.Radius = arc.Radius;

                                    // AutoCAD angles are in Radians (0 to 2PI)
                                    cadData.StartAngle = arc.StartAngle;
                                    cadData.EndAngle = arc.EndAngle;
                                    cadData.TotalAngle = arc.TotalAngle;
                                }
                                else if (ent is Polyline polyline)
                                {
                                    cadData.Vertices = new List<double[]>();
                                    cadData.Bulges = new List<double>();

                                    int vertCount = polyline.NumberOfVertices;
                                    for (int i = 0; i < vertCount; i++)
                                    {
                                        Point3d pt = polyline.GetPoint3dAt(i);
                                        cadData.Vertices.Add(new double[] { pt.X, pt.Y, pt.Z });

                                        cadData.Bulges.Add(polyline.GetBulgeAt(i));
                                    }
                                    cadData.IsClosed = polyline.Closed;
                                }
                                else if (ent is Spline spline)
                                {
                                    if (spline.NumControlPoints > 0)
                                    {
                                        cadData.ControlPoints = new List<double[]>();
                                        for (int i = 0; i < spline.NumControlPoints; i++)
                                        {
                                            Point3d cp = spline.GetControlPointAt(i);
                                            cadData.ControlPoints.Add(new double[] { cp.X, cp.Y, cp.Z });
                                        }
                                    }

                                    if (spline.NumFitPoints > 0)
                                    {
                                        cadData.FitPoints = new List<double[]>();
                                        for (int i = 0; i < spline.NumFitPoints; i++)
                                        {
                                            Point3d fp = spline.GetFitPointAt(i);
                                            cadData.FitPoints.Add(new double[] { fp.X, fp.Y, fp.Z });
                                        }
                                    }

                                    cadData.Degree = spline.Degree;
                                    cadData.IsRational = spline.IsRational;
                                    cadData.IsClosed = spline.Closed;
                                }
                                else if (ent is Ellipse ellipse)
                                {
                                    cadData.CenterPoint = new double[] { ellipse.Center.X, ellipse.Center.Y, ellipse.Center.Z };

                                    cadData.MajorAxis = new double[] { ellipse.MajorAxis.X, ellipse.MajorAxis.Y, ellipse.MajorAxis.Z };
                                    cadData.RadiusRatio = ellipse.RadiusRatio;

                                    cadData.StartAngle = ellipse.StartAngle;
                                    cadData.EndAngle = ellipse.EndAngle;
                                }

                                tempList.Add(cadData);
                            }
                        }
                        return tempList;
                    }
                    cadObjectsData_A = ExtractDataFromLists(_selectedObjectIds_A, "A");
                    cadObjectsData_B = ExtractDataFromLists(_selectedObjectIds_B, "B");
                    cadObjectsData_C = ExtractDataFromLists(_selectedObjectIds_C, "C");
                    tr.Commit();
                }
            }

            //Serialize to JSON
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                Title = "Save CAD Object Data as JSON",
                FileName = "CADObjectData.json"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fullPath = saveFileDialog.FileName;
                string directory = Path.GetDirectoryName(fullPath);
                string fileNameNoExt = Path.GetFileNameWithoutExtension(fullPath);

                var jsonSettings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                };

                int filesCreated = 0;

                if (cadObjectsData_A.Count > 0)
                {
                    string pathA = Path.Combine(directory, $"{fileNameNoExt}_GroupA.json");
                    string jsonA = JsonConvert.SerializeObject(cadObjectsData_A, jsonSettings);
                    File.WriteAllText(pathA, jsonA);
                    filesCreated++;
                }

                if (cadObjectsData_B.Count > 0)
                {
                    string pathB = Path.Combine(directory, $"{fileNameNoExt}_GroupB.json");
                    string jsonB = JsonConvert.SerializeObject(cadObjectsData_B, jsonSettings);
                    File.WriteAllText(pathB, jsonB);
                    filesCreated++;
                }

                if (cadObjectsData_C.Count > 0)
                {
                    string pathC = Path.Combine(directory, $"{fileNameNoExt}_GroupC.json");
                    string jsonC = JsonConvert.SerializeObject(cadObjectsData_C, jsonSettings);
                    File.WriteAllText(pathC, jsonC);
                    filesCreated++;
                }

                MessageBox.Show($"Export Complete!\nGenerated {filesCreated} separate files in:\n{directory}");
            }
        }

        //Helper functions
        private void Form1_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
        }

        private List<ObjectId> PromptUserForSelection(string promptMessage)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            this.Hide();

            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = promptMessage;

            PromptSelectionResult res = ed.GetSelection(opt);

            this.Show();

            if (res.Status == PromptStatus.OK)
            {
                return new List<ObjectId>(res.Value.GetObjectIds());
            }

            return new List<ObjectId>();
        }

    }
}