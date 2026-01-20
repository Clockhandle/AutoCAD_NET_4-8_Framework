using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO; 
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

using WinCombo = System.Windows.Forms.ComboBox;

namespace AutoCAD_NET_4_8_Framework
{
    public partial class UI_Events : Form
    {
        private Dictionary<string, HashSet<ObjectId>> _seamGroups = new Dictionary<string, HashSet<ObjectId>>(); // Vỉa
        private Dictionary<string, HashSet<ObjectId>> _roofGroups = new Dictionary<string, HashSet<ObjectId>>(); // Vách
        private Dictionary<string, HashSet<ObjectId>> _floorGroups = new Dictionary<string, HashSet<ObjectId>>(); // Trụ
        private Dictionary<string, HashSet<ObjectId>> _faultGroups = new Dictionary<string, HashSet<ObjectId>>(); // Đứt gãy
        private static readonly HttpClient _client = new HttpClient();
        public UI_Events()
        {
            InitializeComponent();
        }

        //------ Adding Events ------
        private void OnAddSeams_Click(object sender, EventArgs e)
        {
            ChooseEntities(ListOfSeams, _seamGroups);

        }
        private void OnAddRoofs_Click(object sender, EventArgs e)
        {
            ChooseEntities(ListOfRoofs, _roofGroups);
        }


        private void OnAddFloors_Click(object sender, EventArgs e)
        {
            ChooseEntities(ListOfFloors, _floorGroups);
        }

        private void OnAddFaults_Click(object sender, EventArgs e)
        {
            ChooseEntities(ListOfFaults, _faultGroups);
        }
        //---------------------------

        //------ Storing Events ------
        private void OnStoreSeams_Click(object sender, EventArgs e)
        {
            StoreEntities(_seamGroups, ListOfSeams, lblSeamCount);
        }
        private void OnStoreRoofs_Click(object sender, EventArgs e)
        {
            StoreEntities(_roofGroups, ListOfRoofs, lblRoofCount);
        }

        private void OnStoreFloors_Click(object sender, EventArgs e)
        {
            StoreEntities(_floorGroups, ListOfFloors, lblFloorCount);
        }

        private void OnStoreFaults_Click(object sender, EventArgs e)
        {
            StoreEntities(_faultGroups, ListOfFaults, lblFaultCount);
        }
        //----------------------------
        private void OnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OnExportToJSON_Click(object sender, EventArgs e)
        {
            if (_seamGroups.Count == 0 && _roofGroups.Count == 0 && _floorGroups.Count == 0)
            {
                MessageBox.Show("Chưa có dữ liệu nào (Vỉa, Vách, Trụ) được lưu.");
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
                ProcessDictionaryForExport(tr, _seamGroups, "Via", directory, baseName, jsonSettings, ref filesCreated);

                ProcessDictionaryForExport(tr, _roofGroups, "Vach", directory, baseName, jsonSettings, ref filesCreated);

                ProcessDictionaryForExport(tr, _floorGroups, "Tru", directory, baseName, jsonSettings, ref filesCreated);

                ProcessDictionaryForExport(tr, _faultGroups, "dg", directory, baseName, jsonSettings, ref filesCreated);

                tr.Commit();
            }

            MessageBox.Show($"Export Complete!\nGenerated {filesCreated} separate files in:\n{directory}");
        }

        private async void OnSendToServer_Click(object sender, EventArgs e)
        {
            bool hasSeams = _seamGroups.Any(k => k.Value.Count > 0);
            bool hasRoofs = _roofGroups.Any(k => k.Value.Count > 0);
            bool hasFloors = _floorGroups.Any(k => k.Value.Count > 0);
            bool hasFaults = _faultGroups.Any(k => k.Value.Count > 0);

            if (!hasSeams && !hasRoofs && !hasFloors && !hasFaults)
            {
                MessageBox.Show("Chưa có dữ liệu nào (Vỉa, Vách, Trụ, Đứt gãy) để gửi.");
                return;
            }

            string ipInput = txtServerUrl.Text.Trim();
            if (string.IsNullOrEmpty(ipInput))
            {
                MessageBox.Show("Hãy điền đúng địa chỉ IP.");
                return;
            }

            List<CADObjectData> masterUploadList = new List<CADObjectData>();
            Document doc = Application.DocumentManager.MdiActiveDocument;

            // 2. Extract Data from all Seams
            using (DocumentLock docLock = doc.LockDocument())
            using (Transaction tr = doc.Database.TransactionManager.StartTransaction())
            {
                void CollectData(Dictionary<string, HashSet<ObjectId>> groups, string prefix)
                {
                    foreach (var kvp in groups)
                    {
                        if (kvp.Value.Count == 0) continue;

                        string fullGroupName = $"{prefix}_{kvp.Key}";

                        List<CADObjectData> data = GetCadData(tr, kvp.Value.ToList(), fullGroupName);
                        masterUploadList.AddRange(data);
                    }
                }

                CollectData(_seamGroups, "via");
                CollectData(_roofGroups, "vach");
                CollectData(_floorGroups, "tru");
                CollectData(_faultGroups, "dg");
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

        private void UpdateLabel(WinCombo comboBox, Dictionary<string, HashSet<ObjectId>> groups, Label groupSelectedName)
        {
            string currentBox = comboBox.SelectedItem as string;

            if (string.IsNullOrEmpty(currentBox) || !groups.ContainsKey(currentBox))
            {
                groupSelectedName.Text = "Số đối tượng: 0";
                return;
            }

            int count = groups[currentBox].Count;
            groupSelectedName.Text = $"Số đối tượng: {count}";
        }

        private void StoreEntities(Dictionary<string, HashSet<ObjectId>> groups, WinCombo comboBox, Label groupSelectedName)
        {
            string type = comboBox.SelectedItem as string;

            if (string.IsNullOrEmpty(type))
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
                opt.MessageForAdding = $"\nChọn đối tượng cho vỉa '{type}': ";
                res = ed.GetSelection(opt);

                this.Show();
            }

            if (res.Status == PromptStatus.OK)
            {
                ObjectId[] ids = res.Value.GetObjectIds();

                if (!groups.ContainsKey(type))
                {
                    groups.Add(type, new HashSet<ObjectId>());
                }

                HashSet<ObjectId> bucket = groups[type];

                int before = bucket.Count;
                foreach (ObjectId id in ids)
                {
                    bucket.Add(id);
                }
                int added = bucket.Count - before;

                if (added > 0)
                {
                    MessageBox.Show($"Đã lưu {added} đối tượng mới vào '{type}'.\nTổng cộng: {bucket.Count}");
                    UpdateLabel(comboBox, groups, groupSelectedName);
                }
                else
                {
                    MessageBox.Show($"Các đối tượng này đã tồn tại trong '{type}'. Không có gì mới được thêm.");
                }
            }
        }

        private void ChooseEntities(WinCombo comboBox, Dictionary<string, HashSet<ObjectId>> groups)
        {
            string name = comboBox.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Hãy điền tên vỉa để thêm vào danh sách.");
                return;
            }
            if (groups.ContainsKey(name))
            {
                MessageBox.Show("Vỉa cùng tên đã tồn tại trong danh sách. Hãy chọn tên khác.");
                return;
            }

            groups.Add(name, new HashSet<ObjectId>());

            comboBox.Items.Add(name);
            comboBox.SelectedItem = name;

            MessageBox.Show($"Đã thêm vỉa '{name}' vào danh sách.");
        }

        private void ProcessDictionaryForExport(Transaction tr, Dictionary<string, HashSet<ObjectId>> groups, string prefix, string dir, string baseName, JsonSerializerSettings settings, ref int files)
        {
            foreach (var kvp in groups)
            {
                if (kvp.Value.Count == 0) continue;

                // Note: GroupName sent to JSON is "Via_Via1" or "Vach_Vach1"
                string fullGroupName = $"{prefix}_{kvp.Key}";
                List<CADObjectData> data = GetCadData(tr, kvp.Value.ToList(), fullGroupName);

                if (data.Count > 0)
                {
                    string safeName = string.Join("_", kvp.Key.Split(Path.GetInvalidFileNameChars()));
                    string filePath = Path.Combine(dir, $"{baseName}_{prefix}_{safeName}.json");

                    File.WriteAllText(filePath, JsonConvert.SerializeObject(data, settings));
                    files++;
                }
            }
        }


    }
}