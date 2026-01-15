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
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AutoCAD_NET_4_8_Framework
{
    public partial class Form1 : Form
    {
        private List<ObjectId> _selectedObjectIds_A = new List<ObjectId>();
        private List<ObjectId> _selectedObjectIds_B = new List<ObjectId>();
        private List<ObjectId> _selectedObjectIds_C = new List<ObjectId>();
        private static readonly HttpClient _client = new HttpClient();
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

            // Create empty lists
            List<CADObjectData> cadObjectsData_A = new List<CADObjectData>();
            List<CADObjectData> cadObjectsData_B = new List<CADObjectData>();
            List<CADObjectData> cadObjectsData_C = new List<CADObjectData>();

            using (DocumentLock docLock = doc.LockDocument())
            using (Transaction tr = doc.Database.TransactionManager.StartTransaction())
            {
                // LOOK HOW CLEAN THIS IS NOW:
                // We use the same 'GetCadData' helper as the Upload button!
                cadObjectsData_A = GetCadData(tr, _selectedObjectIds_A, "Group_A");
                cadObjectsData_B = GetCadData(tr, _selectedObjectIds_B, "Group_B");
                cadObjectsData_C = GetCadData(tr, _selectedObjectIds_C, "Group_C");

                tr.Commit();
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

        private async void OnSendToServer_Click(object sender, EventArgs e)
        {
            if (_selectedObjectIds_A.Count == 0 && _selectedObjectIds_B.Count == 0 && _selectedObjectIds_C.Count == 0)
            {
                MessageBox.Show("Please select objects first.");
                return;
            }

            List<CADObjectData> masterUploadList = new List<CADObjectData>();

            Document doc = Application.DocumentManager.MdiActiveDocument;

            using (DocumentLock docLock = doc.LockDocument())
            using (Transaction tr = doc.Database.TransactionManager.StartTransaction())
            {
                masterUploadList.AddRange(GetCadData(tr, _selectedObjectIds_A, "Group_A"));
                masterUploadList.AddRange(GetCadData(tr, _selectedObjectIds_B, "Group_B"));
                masterUploadList.AddRange(GetCadData(tr, _selectedObjectIds_C, "Group_C"));

                tr.Commit();
            }

            if (masterUploadList.Count > 0)
            {
                System.Windows.Forms.Button btn = sender as System.Windows.Forms.Button;
                string originalText = btn.Text;
                btn.Text = "Uploading...";
                btn.Enabled = false; // Prevent double-clicking

                await UploadJsonDataAsync(masterUploadList);

                btn.Text = originalText;
                btn.Enabled = true;
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

        private async Task UploadJsonDataAsync(List<CADObjectData> payload)
        {
            string url = "http://10.60.161.35:3000/api/cad-upload";

            try
            {
                string json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Send POST request
                HttpResponseMessage response = await _client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Upload Success!\nServer replied: {responseBody}");
                }
                else
                {
                    MessageBox.Show($"Server Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Upload Failed: {ex.Message}");
            }
        }

        private List<CADObjectData> GetCadData(Transaction tr, List<ObjectId> ids, string groupName)
        {
            List<CADObjectData> tempList = new List<CADObjectData>();

            foreach (ObjectId id in ids)
            {
                if (id.IsErased) continue;
                Entity ent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                if (ent == null) continue;

                CADObjectData data = new CADObjectData
                {
                    GroupName = groupName,
                    Handle = ent.Handle.ToString(),
                    ObjectType = ent.GetType().Name,
                    Layer = ent.Layer
                };

                if (ent is Line line)
                {
                    data.StartPoint = new double[] { line.StartPoint.X, line.StartPoint.Y, line.StartPoint.Z };
                    data.EndPoint = new double[] { line.EndPoint.X, line.EndPoint.Y, line.EndPoint.Z };
                }
                else if (ent is Circle circle)
                {
                    data.CenterPoint = new double[] { circle.Center.X, circle.Center.Y, circle.Center.Z };
                    data.Radius = circle.Radius;
                }
                else if (ent is Arc arc)
                {
                    data.CenterPoint = new double[] { arc.Center.X, arc.Center.Y, arc.Center.Z };
                    data.Radius = arc.Radius;
                    data.StartAngle = arc.StartAngle;
                    data.EndAngle = arc.EndAngle;
                    data.TotalAngle = arc.TotalAngle;
                }
                else if (ent is Polyline polyline)
                {
                    data.Vertices = new List<double[]>();
                    data.Bulges = new List<double>();
                    int vertCount = polyline.NumberOfVertices;
                    for (int i = 0; i < vertCount; i++)
                    {
                        Point3d pt = polyline.GetPoint3dAt(i);
                        data.Vertices.Add(new double[] { pt.X, pt.Y, pt.Z });
                        data.Bulges.Add(polyline.GetBulgeAt(i));
                    }
                    data.IsClosed = polyline.Closed;
                }
                else if (ent is Spline spline)
                {
                    if (spline.NumFitPoints > 0)
                    {
                        data.FitPoints = new List<double[]>();
                        for (int i = 0; i < spline.NumFitPoints; i++)
                        {
                            Point3d fp = spline.GetFitPointAt(i);
                            data.FitPoints.Add(new double[] { fp.X, fp.Y, fp.Z });
                        }
                    }
                    data.Degree = spline.Degree;
                    data.IsRational = spline.IsRational;
                    data.IsClosed = spline.Closed;
                }
                else if (ent is Ellipse ellipse)
                {
                    data.CenterPoint = new double[] { ellipse.Center.X, ellipse.Center.Y, ellipse.Center.Z };
                    data.MajorAxis = new double[] { ellipse.MajorAxis.X, ellipse.MajorAxis.Y, ellipse.MajorAxis.Z };
                    data.RadiusRatio = ellipse.RadiusRatio;
                    data.StartAngle = ellipse.StartAngle;
                    data.EndAngle = ellipse.EndAngle;
                }

                tempList.Add(data);
            }
            return tempList;
        }

    }
}