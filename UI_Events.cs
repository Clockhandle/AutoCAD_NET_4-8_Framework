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
    public partial class UI_Events : Form
    {
        private List<ObjectId> _selectedObjectIds_A = new List<ObjectId>();
        private List<ObjectId> _selectedObjectIds_B = new List<ObjectId>();
        private List<ObjectId> _selectedObjectIds_C = new List<ObjectId>();
        private Dictionary<string, HashSet<ObjectId>> _seamGroups = new Dictionary<string, HashSet<ObjectId>>();
        private static readonly HttpClient _client = new HttpClient();
        public UI_Events()
        {
            InitializeComponent();
        }

        private void OnAddSeams_Click(object sender, EventArgs e)
        {
            string name = ListOfSeams.Text.Trim();

            if(string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Hãy điền tên vỉa để thêm vào danh sách.");
                return;
            }
            if(_seamGroups.ContainsKey(name))
            {
                MessageBox.Show("Vỉa cùng tên đã tồn tại trong danh sách. Hãy chọn tên khác.");
                return;
            }

            _seamGroups.Add(name, new HashSet<ObjectId>());

            ListOfSeams.Items.Add(name);
            ListOfSeams.SelectedItem = name;

            MessageBox.Show($"Đã thêm vỉa '{name}' vào danh sách.");
        }

        private void OnStoreSeams_Click(object sender, EventArgs e)
        {
            string currentSeam = ListOfSeams.SelectedItem as string;

            if (string.IsNullOrEmpty(currentSeam))
            {
                MessageBox.Show("Hãy chọn vỉa từ danh sách để lưu các đối tượng đã chọn.");
                return;
            }

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            PromptSelectionResult res = ed.SelectImplied();

            if (res.Status != PromptStatus.OK)
            {
                this.Hide();

                PromptSelectionOptions opt = new PromptSelectionOptions();
                opt.MessageForAdding = $"\nChọn đối tượng cho vỉa '{currentSeam}': "; 
                res = ed.GetSelection(opt);

                this.Show();
            }


            if (res.Status == PromptStatus.OK)
            {
                ObjectId[] ids = res.Value.GetObjectIds();

                if (!_seamGroups.ContainsKey(currentSeam))
                {
                    _seamGroups.Add(currentSeam, new HashSet<ObjectId>());
                }

                HashSet<ObjectId> bucket = _seamGroups[currentSeam];

                int before = bucket.Count;
                foreach (ObjectId id in ids)
                {
                    bucket.Add(id);
                }
                int added = bucket.Count - before;

                if (added > 0)
                {
                    MessageBox.Show($"Đã lưu {added} đối tượng mới vào '{currentSeam}'.\nTổng cộng: {bucket.Count}");
                    UpdateSeamInfoLabel();
                }
                else
                {
                    MessageBox.Show($"Các đối tượng này đã tồn tại trong '{currentSeam}'. Không có gì mới được thêm.");
                }
            }
        }

        private void OnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OnExportToJSON_Click(object sender, EventArgs e)
        {
            if (_seamGroups.Count == 0 || _seamGroups.All(k => k.Value.Count == 0))
            {
                MessageBox.Show("Chưa có dữ liệu nào được lưu trong danh sách vỉa.");
                return;
            }

            Document doc = Application.DocumentManager.MdiActiveDocument;

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                Title = "Save CAD Object Data",
                FileName = "MiningData.json"
            };

            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

            string fullPath = saveFileDialog.FileName;
            string directory = Path.GetDirectoryName(fullPath);
            string baseName = Path.GetFileNameWithoutExtension(fullPath);

            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };

            int filesCreated = 0;

            using (DocumentLock docLock = doc.LockDocument())
            using (Transaction tr = doc.Database.TransactionManager.StartTransaction())
            {
                foreach (var kvp in _seamGroups)
                {
                    string seamName = kvp.Key;            
                    HashSet<ObjectId> ids = kvp.Value;    

                    if (ids.Count == 0) continue; 
                    List<CADObjectData> seamData = GetCadData(tr, ids.ToList(), seamName);

                    if (seamData.Count > 0)
                    {
                        string safeSeamName = string.Join("_", seamName.Split(Path.GetInvalidFileNameChars()));
                        string filePath = Path.Combine(directory, $"{baseName}_{safeSeamName}.json");

                        string json = JsonConvert.SerializeObject(seamData, jsonSettings);
                        File.WriteAllText(filePath, json);
                        filesCreated++;
                    }
                }
                tr.Commit();
            }

            MessageBox.Show($"Export Complete!\nGenerated {filesCreated} separate files in:\n{directory}");
        }

        private async void OnSendToServer_Click(object sender, EventArgs e)
        {
            if (_seamGroups.Count == 0 || _seamGroups.All(k => k.Value.Count == 0))
            {
                MessageBox.Show("Chưa có dữ liệu nào để gửi. Hãy thêm vỉa và lưu đối tượng trước.");
                return;
            }

            string ipInput = txtServerUrl.Text.Trim();
            if (string.IsNullOrEmpty(ipInput))
            {
                MessageBox.Show("Please enter a Server IP address.");
                return;
            }

            List<CADObjectData> masterUploadList = new List<CADObjectData>();
            Document doc = Application.DocumentManager.MdiActiveDocument;

            // 2. Extract Data from all Seams
            using (DocumentLock docLock = doc.LockDocument())
            using (Transaction tr = doc.Database.TransactionManager.StartTransaction())
            {
                foreach (var kvp in _seamGroups)
                {
                    string seamName = kvp.Key;            
                    List<ObjectId> ids = kvp.Value.ToList(); 

                    if (ids.Count == 0) continue;

                    List<CADObjectData> seamData = GetCadData(tr, ids, seamName);
                    masterUploadList.AddRange(seamData);
                }
                tr.Commit();
            }

            if (masterUploadList.Count > 0)
            {
                System.Windows.Forms.Button btn = sender as System.Windows.Forms.Button;
                string originalText = btn.Text;
                btn.Text = "Uploading...";
                btn.Enabled = false;

                try
                {
                    await UploadJsonDataAsync(masterUploadList, ipInput);
                }
                finally
                {
                    btn.Text = originalText;
                    btn.Enabled = true;
                }
            }
            else
            {
                MessageBox.Show("Không tìm thấy đối tượng hợp lệ nào để upload (có thể chúng đã bị xóa khỏi bản vẽ).");
            }
        }

        //Helper functions
        private void UI_Events_Load(object sender, EventArgs e)
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

        private async Task UploadJsonDataAsync(List<CADObjectData> payload, string serverUrl)
        {
            string cleanBaseUrl = serverUrl.TrimEnd('/');
            string fullUrl = $"{cleanBaseUrl}/api/cad-upload";

            try
            {
                string json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Send POST request
                HttpResponseMessage response = await _client.PostAsync(fullUrl, content);

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
                MessageBox.Show($"Upload Failed: {ex.Message}\nCheck your IP address.");
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

        private void UpdateSeamInfoLabel()
        {
            string currentSeam = ListOfSeams.SelectedItem as string;

            if (string.IsNullOrEmpty(currentSeam) || !_seamGroups.ContainsKey(currentSeam))
            {
                SelectedObjectsLabel.Text = "Số đối tượng: 0";
                return;
            }

            int count = _seamGroups[currentSeam].Count;
            SelectedObjectsLabel.Text = $"Số đối tượng: {count}";
        }
    }
}