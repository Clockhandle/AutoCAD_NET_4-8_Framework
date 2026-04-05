using Autodesk.AutoCAD.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MyMiningPlugin.Models;
using MyMiningPlugin.Services;
using MyMiningPlugin.UI;

namespace MyMiningPlugin
{
    // --- MAIN FORM (REFACTORED) ---
    public class MiningManagerForm : Form
    {
        // UI Components
        private SplitContainer splitContainer;
        private TreeView treeView;
        private Panel rightPanel;

        // Data Container
        private MiningProject _project = new MiningProject();

        // Services
        private readonly AutoCADSelectionService _selectionService;
        private readonly GeometryProcessingService _geometryProcessor;
        private readonly PersistenceService _persistenceService;
        private readonly ExportService _exportService;

        public MiningManagerForm()
        {
            // Initialize services
            _selectionService = new AutoCADSelectionService();
            _geometryProcessor = new GeometryProcessingService(_selectionService);
            _persistenceService = new PersistenceService();
            _exportService = new ExportService(_geometryProcessor);

            InitializeComponent();
            InitializeCustomUI();
            InitializeSampleData();
        }

        private void InitializeComponent()
        {
            this.Text = "Mining Data Manager";
            this.Size = new Size(900, 600);
        }

        private void InitializeCustomUI()
        {
            // 1. Setup SplitContainer
            splitContainer = new SplitContainer();
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.SplitterDistance = 250;
            this.Controls.Add(splitContainer);

            // 2. Setup TreeView (Left Panel)
            treeView = new TreeView();
            treeView.Dock = DockStyle.Fill;
            treeView.HideSelection = false;
            treeView.AfterSelect += TreeView_AfterSelect;
            splitContainer.Panel1.Controls.Add(treeView);

            // 3. Setup Right Panel
            rightPanel = new Panel();
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.Padding = new Padding(20);
            rightPanel.AutoScroll = true;
            splitContainer.Panel2.Controls.Add(rightPanel);
        }

        private void InitializeSampleData()
        {
            treeView.Nodes.Clear();

            // Root Node 1: Vỉa
            TreeNode rootVias = new TreeNode("Danh sách Vỉa");
            rootVias.Tag = "RootVias";
            treeView.Nodes.Add(rootVias);

            // Root Node 2: Đứt gãy
            TreeNode rootFaults = new TreeNode("Danh sách Đứt gãy");
            rootFaults.Tag = "RootFaults";
            treeView.Nodes.Add(rootFaults);

            // Root Node 3: Nham thạch
            TreeNode rootRocks = new TreeNode("Danh sách Nham thạch");
            rootRocks.Tag = "RootRocks";
            treeView.Nodes.Add(rootRocks);

            // Root Node 4: Lỗ khoan
            TreeNode rootBoreholes = new TreeNode("Danh sách Lỗ khoan");
            rootBoreholes.Tag = "RootBoreholes";
            treeView.Nodes.Add(rootBoreholes);

            // Add sample data
            AddNewVia("Vỉa 1");

            treeView.ExpandAll();
        }

        // --- LOGIC: ADDING DATA ---

        private void AddNewVia(string name = null)
        {
            string viaName = name ?? $"Vỉa {_project.Vias.Count + 1}";
            ViaData newVia = new ViaData { Name = viaName };
            _project.Vias.Add(newVia);

            TreeNode node = new TreeNode(viaName);
            node.Tag = newVia;

            foreach (TreeNode n in treeView.Nodes)
            {
                if (n.Tag as string == "RootVias")
                {
                    n.Nodes.Add(node);
                    n.Expand();
                    break;
                }
            }
            
            // Automatically add first block to new Via
            AddNewBlock(node);
        }

        private void AddNewBlock(TreeNode viaNode)
        {
            ViaData via = viaNode.Tag as ViaData;
            if (via == null) return;

            string blockName = $"Khối {via.Blocks.Count + 1}";
            KhoiData newBlock = new KhoiData
            {
                Name = blockName,
                Vach = new SurfaceData { Type = "Vách", ParentName = $"{via.Name} - {blockName}" },
                Tru = new SurfaceData { Type = "Trụ", ParentName = $"{via.Name} - {blockName}" }
            };
            via.Blocks.Add(newBlock);

            TreeNode blockNode = new TreeNode(blockName);
            blockNode.Tag = newBlock;

            TreeNode vachNode = new TreeNode("Vách");
            vachNode.Tag = newBlock.Vach;

            TreeNode truNode = new TreeNode("Trụ");
            truNode.Tag = newBlock.Tru;

            blockNode.Nodes.Add(vachNode);
            blockNode.Nodes.Add(truNode);
            viaNode.Nodes.Add(blockNode);
            viaNode.Expand();
        }

        private void AddNewFault()
        {
            string faultName = $"Đứt gãy {_project.Faults.Count + 1}";
            FaultData newFault = new FaultData
            {
                Name = faultName,
                Surface = new SurfaceData { Type = "Đứt gãy", ParentName = faultName }
            };
            _project.Faults.Add(newFault);

            TreeNode node = new TreeNode(faultName);
            node.Tag = newFault;

            foreach (TreeNode n in treeView.Nodes)
            {
                if (n.Tag as string == "RootFaults")
                {
                    n.Nodes.Add(node);
                    n.Expand();
                    break;
                }
            }
        }

        private void AddNewRock()
        {
            string rockName = $"Nham thạch {_project.Rocks.Count + 1}";
            RockData newRock = new RockData
            {
                Name = rockName,
                Surface = new SurfaceData { Type = "Nham thạch", ParentName = rockName }
            };
            _project.Rocks.Add(newRock);

            TreeNode node = new TreeNode(rockName);
            node.Tag = newRock;

            foreach (TreeNode n in treeView.Nodes)
            {
                if (n.Tag as string == "RootRocks")
                {
                    n.Nodes.Add(node);
                    n.Expand();
                    break;
                }
            }
        }

        private void AddNewBorehole()
        {
            string boreholeName = $"Lỗ khoan {_project.Boreholes.Count + 1}";
            BoreholeData newBorehole = new BoreholeData
            {
                Name = boreholeName
            };
            _project.Boreholes.Add(newBorehole);

            TreeNode node = new TreeNode(boreholeName);
            node.Tag = newBorehole;

            foreach (TreeNode n in treeView.Nodes)
            {
                if (n.Tag as string == "RootBoreholes")
                {
                    n.Nodes.Add(node);
                    n.Expand();
                    break;
                }
            }
        }

        // --- CORE LOGIC: SWITCHING UI ---

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            rightPanel.Controls.Clear();

            // Handle Roots
            if (e.Node.Tag as string == "RootVias")
            {
                AddHeader("Quản lý Vỉa");
                AddButton("+ Thêm Vỉa Mới", (s, ev) => AddNewVia(), Color.LightBlue);
                AddButton("Gửi lên Server", (s, ev) => ShowSendMultipleDialog("Vỉa"), Color.LightGreen, 90);
                AddButton("Xuất JSON", (s, ev) => ShowExportMultipleDialog("Vỉa"), Color.LightYellow, 130);
                AddButton("💾 Lưu dự án", (s, ev) => SaveProjectData(), Color.LightCoral, 170);
                AddButton("📁 Tải dự án", (s, ev) => LoadProjectData(), Color.LightSkyBlue, 210);
            }
            else if (e.Node.Tag as string == "RootFaults")
            {
                AddHeader("Quản lý Đứt gãy");
                AddButton("+ Thêm Đứt gãy Mới", (s, ev) => AddNewFault(), Color.LightBlue);
                AddButton("Gửi lên Server", (s, ev) => ShowSendMultipleDialog("Đứt gãy"), Color.LightGreen, 90);
                AddButton("Xuất JSON", (s, ev) => ShowExportMultipleDialog("Đứt gãy"), Color.LightYellow, 130);
                AddButton("💾 Lưu dự án", (s, ev) => SaveProjectData(), Color.LightCoral, 170);
                AddButton("📁 Tải dự án", (s, ev) => LoadProjectData(), Color.LightSkyBlue, 210);
            }
            else if (e.Node.Tag as string == "RootRocks")
            {
                AddHeader("Quản lý Nham thạch");
                AddButton("+ Thêm Nham thạch Mới", (s, ev) => AddNewRock(), Color.LightBlue);
                AddButton("Gửi lên Server", (s, ev) => ShowSendMultipleDialog("Nham thạch"), Color.LightGreen, 90);
                AddButton("Xuất JSON", (s, ev) => ShowExportMultipleDialog("Nham thạch"), Color.LightYellow, 130);
                AddButton("💾 Lưu dự án", (s, ev) => SaveProjectData(), Color.LightCoral, 170);
                AddButton("📁 Tải dự án", (s, ev) => LoadProjectData(), Color.LightSkyBlue, 210);
            }
            else if (e.Node.Tag as string == "RootBoreholes")
            {
                AddHeader("Quản lý Lỗ khoan");
                AddButton("+ Thêm Lỗ khoan Mới", (s, ev) => AddNewBorehole(), Color.LightBlue);
                AddButton("Gửi lên Server", (s, ev) => ShowSendMultipleDialog("Lỗ khoan"), Color.LightGreen, 90);
                AddButton("Xuất JSON", (s, ev) => ShowExportMultipleDialog("Lỗ khoan"), Color.LightYellow, 130);
                AddButton("💾 Lưu dự án", (s, ev) => SaveProjectData(), Color.LightCoral, 170);
                AddButton("📁 Tải dự án", (s, ev) => LoadProjectData(), Color.LightSkyBlue, 210);
            }
            else if (e.Node.Tag is ViaData via)
            {
                RenderViaUI(via, e.Node);
            }
            else if (e.Node.Tag is KhoiData khoi)
            {
                AddHeader(khoi.Name);
                Label info = new Label { Text = $"Vách: {khoi.Vach.SelectedGeometry.Count} lines\nTrụ: {khoi.Tru.SelectedGeometry.Count} lines", AutoSize = true, Location = new Point(0, 50) };
                rightPanel.Controls.Add(info);
                AddButton("Xóa Khối", (s, ev) => { e.Node.Remove(); rightPanel.Controls.Clear(); }, Color.IndianRed, 100);
            }
            else if (e.Node.Tag is FaultData fault)
            {
                RenderSurfaceUI(fault.Surface, e.Node, isFault: true);
            }
            else if (e.Node.Tag is RockData rock)
            {
                RenderSurfaceUI(rock.Surface, e.Node, isRock: true);
            }
            else if (e.Node.Tag is BoreholeData borehole)
            {
                RenderBoreholeUI(borehole, e.Node);
            }
            else if (e.Node.Tag is SurfaceData surface)
            {
                RenderSurfaceUI(surface, e.Node);
            }
        }

        // --- RENDERERS ---

        private void RenderSurfaceUI(SurfaceData surface, TreeNode node, bool isFault = false, bool isRock = false)
        {
            string title = (isFault || isRock) ? surface.ParentName : $"{surface.ParentName} - {surface.Type}";
            AddHeader(title);

            _selectionService.ResolveCurrentDrawingReferences(surface);

            ListBox lbIds = new ListBox();
            lbIds.Location = new Point(0, 60);
            lbIds.Size = new Size(450, 150);
            
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            string currentDwg = string.IsNullOrEmpty(doc.Database.Filename) ? "Unsaved" : System.IO.Path.GetFileName(doc.Database.Filename);

            foreach (var geoRef in surface.SelectedGeometry)
            {
                bool inCurrentDwg = geoRef.CurrentObjectId.HasValue && !geoRef.CurrentObjectId.Value.IsNull;
                string status = inCurrentDwg ? "✓" : "⚠";
                string display = $"{status} [{geoRef.Handle}] {geoRef.Layer} ({geoRef.SourceDwgName})";
                lbIds.Items.Add(display);
            }
            rightPanel.Controls.Add(lbIds);

            int totalCount = surface.SelectedGeometry.Count;
            int currentCount = surface.SelectedGeometry.Count(g => g.CurrentObjectId.HasValue && !g.CurrentObjectId.Value.IsNull);
            
            Label lblSummary = new Label();
            lblSummary.Text = $"Tổng: {totalCount} lines | Trong DWG này: {currentCount} lines";
            lblSummary.Location = new Point(0, 220);
            lblSummary.AutoSize = true;
            lblSummary.Font = new System.Drawing.Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold);
            lblSummary.ForeColor = currentCount < totalCount ? Color.DarkOrange : Color.DarkGreen;
            rightPanel.Controls.Add(lblSummary);

            int currentY = 250;
            
            Button btnSelect = new Button();
            btnSelect.Text = "Chọn thêm đường";
            btnSelect.AutoSize = true;
            btnSelect.Padding = new Padding(5);
            btnSelect.Location = new Point(0, currentY);
            btnSelect.BackColor = Color.LightCyan;
            btnSelect.Click += (s, e) => {
                this.Hide();
                _selectionService.SelectLinesFromAutoCAD(surface);
                this.Show();
                TreeView_AfterSelect(null, new TreeViewEventArgs(node));
            };
            rightPanel.Controls.Add(btnSelect);

            Button btnClear = new Button();
            btnClear.Text = "Xóa tất cả";
            btnClear.AutoSize = true;
            btnClear.Padding = new Padding(5);
            btnClear.Location = new Point(btnSelect.Right + 10, currentY);
            btnClear.BackColor = Color.White;
            btnClear.Click += (s, e) => {
                surface.SelectedGeometry.Clear();
                TreeView_AfterSelect(null, new TreeViewEventArgs(node));
            };
            rightPanel.Controls.Add(btnClear);

            if (isFault)
            {
                Button btnDelFault = new Button();
                btnDelFault.Text = "Xóa Đứt gãy này";
                btnDelFault.AutoSize = true;
                btnDelFault.Padding = new Padding(5);
                btnDelFault.Location = new Point(0, currentY + 40);
                btnDelFault.BackColor = Color.IndianRed;
                btnDelFault.Click += (s, e) => {
                    node.Remove();
                    rightPanel.Controls.Clear();
                };
                rightPanel.Controls.Add(btnDelFault);
            }
            else if (isRock)
            {
                Button btnDelRock = new Button();
                btnDelRock.Text = "Xóa Nham thạch này";
                btnDelRock.AutoSize = true;
                btnDelRock.Padding = new Padding(5);
                btnDelRock.Location = new Point(0, currentY + 40);
                btnDelRock.BackColor = Color.IndianRed;
                btnDelRock.Click += (s, e) => {
                    node.Remove();
                    rightPanel.Controls.Clear();
                };
                rightPanel.Controls.Add(btnDelRock);
            }
        }

        private void RenderBoreholeUI(BoreholeData borehole, TreeNode node)
        {
            AddHeader(borehole.Name);

            Label lblInfo = new Label();
            lblInfo.Text = "Dữ liệu nguồn (File Excel):";
            lblInfo.Location = new Point(0, 60);
            lblInfo.AutoSize = true;
            rightPanel.Controls.Add(lblInfo);

            TextBox txtExcelPath = new TextBox();
            txtExcelPath.Location = new Point(0, 85);
            txtExcelPath.Size = new Size(350, 20);
            txtExcelPath.ReadOnly = true;
            // TODO: Bind to actual model property when data structure is updated
            txtExcelPath.Text = ""; 
            rightPanel.Controls.Add(txtExcelPath);

            Button btnSelectExcel = new Button();
            btnSelectExcel.Text = "Chọn file Excel";
            btnSelectExcel.AutoSize = true;
            btnSelectExcel.Padding = new Padding(5);
            btnSelectExcel.Location = new Point(360, 80);
            btnSelectExcel.BackColor = Color.LightCyan;
            btnSelectExcel.Click += (s, e) => {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
                    ofd.Title = "Chọn file dữ liệu Lỗ khoan";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        txtExcelPath.Text = ofd.FileName;
                        borehole.ExcelFilePath = ofd.FileName;
                    }
                }
            };
            rightPanel.Controls.Add(btnSelectExcel);

            Button btnParseExcel = new Button();
            btnParseExcel.Text = "Đọc dữ liệu Excel";
            btnParseExcel.AutoSize = true;
            btnParseExcel.Padding = new Padding(5);
            btnParseExcel.Location = new Point(0, 130);
            btnParseExcel.BackColor = Color.LightGreen;
            btnParseExcel.Click += (s, e) => {
                if (string.IsNullOrEmpty(borehole.ExcelFilePath) || !System.IO.File.Exists(borehole.ExcelFilePath))
                {
                    MessageBox.Show("Vui lòng chọn file Excel hợp lệ trước.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                try 
                {
                    // Parse all boreholes and dump to JSON
                    var dummyTest = MyMiningPlugin.Services.BoreholeExcelParser.ParseAllBoreholes(borehole.ExcelFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi đọc file Excel:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            rightPanel.Controls.Add(btnParseExcel);

            Button btnDelBorehole = new Button();
            btnDelBorehole.Text = "Xóa Lỗ khoan này";
            btnDelBorehole.AutoSize = true;
            btnDelBorehole.Padding = new Padding(5);
            btnDelBorehole.Location = new Point(0, 180);
            btnDelBorehole.BackColor = Color.IndianRed;
            btnDelBorehole.Click += (s, e) => {
                node.Remove();
                _project.Boreholes.Remove(borehole);
                rightPanel.Controls.Clear();
            };
            rightPanel.Controls.Add(btnDelBorehole);
        }

        private void RenderViaUI(ViaData via, TreeNode node)
        {
            AddHeader($"Toàn bộ {via.Name}");

            Label info = new Label 
            { 
                Text = $"Số Khối: {via.Blocks.Count}\nTổng Vách: {via.Blocks.Sum(b => b.Vach.SelectedGeometry.Count)} lines\nTổng Trụ: {via.Blocks.Sum(b => b.Tru.SelectedGeometry.Count)} lines", 
                AutoSize = true, 
                Location = new Point(0, 50) 
            };
            rightPanel.Controls.Add(info);

            Button btnAdd = new Button();
            btnAdd.Text = "+ Thêm Khối vào Vỉa này";
            btnAdd.AutoSize = true;
            btnAdd.Padding = new Padding(5);
            btnAdd.Location = new Point(0, 100);
            btnAdd.BackColor = Color.LightGreen;
            btnAdd.Click += (s, ev) => AddNewBlock(node);
            rightPanel.Controls.Add(btnAdd);

            Button btnDelete = new Button();
            btnDelete.Text = "Xóa Vỉa";
            btnDelete.AutoSize = true;
            btnDelete.Padding = new Padding(5);
            btnDelete.Location = new Point(btnAdd.Right + 10, 100);
            btnDelete.BackColor = Color.IndianRed;
            btnDelete.Click += (s, ev) => { node.Remove(); _project.Vias.Remove(via); rightPanel.Controls.Clear(); };
            rightPanel.Controls.Add(btnDelete);
        }

        // --- HELPER UI METHODS ---

        private void AddHeader(string text)
        {
            Label title = new Label();
            title.Text = text;
            title.Font = new System.Drawing.Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold);
            title.AutoSize = true;
            title.Location = new Point(0, 10);
            rightPanel.Controls.Add(title);
        }

        private Button AddButton(string text, EventHandler onClick, Color? bg = null, int top = -1)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.AutoSize = true;
            btn.Padding = new Padding(5);
            if (top == -1)
                top = rightPanel.Controls.Count > 0 ? rightPanel.Controls[rightPanel.Controls.Count - 1].Bottom + 10 : 50;

            btn.Location = new Point(0, top);
            btn.BackColor = bg ?? Control.DefaultBackColor;
            btn.Click += onClick;
            rightPanel.Controls.Add(btn);
            return btn;
        }

        // --- SERVICE DELEGATION METHODS ---

        private void SaveProjectData()
        {
            _persistenceService.SaveProjectData(_project);
        }

        private void LoadProjectData()
        {
            var loadedProject = _persistenceService.LoadProjectData();
            if (loadedProject != null)
            {
                _project = loadedProject;
                RebuildTreeView();
            }
        }

        private void ShowSendMultipleDialog(string category)
        {
            List<string> items = GetItemsByCategory(category);
            if (items.Count == 0)
            {
                MessageBox.Show($"Chưa có {category} nào!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var dialog = new SendMultipleDataDialog($"Chọn {category} để gửi", items, category);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SendMultipleToServer(category, dialog.SelectedItems, dialog.MapName, dialog.ServerIP, dialog.ServerPort);
            }
        }

        private void ShowExportMultipleDialog(string category)
        {
            List<string> items = GetItemsByCategory(category);
            if (items.Count == 0)
            {
                MessageBox.Show($"Chưa có {category} nào!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // DIAGNOSTIC: Check project state before opening dialog
            if (category == "Vỉa")
            {
                string diagMsg = "=== PRE-EXPORT DIAGNOSTIC ===\n";
                diagMsg += $"Số Vỉa: {_project.Vias.Count}\n";
                int totalVach = 0;
                int totalTru = 0;
                
                foreach (var via in _project.Vias)
                {
                    diagMsg += $"\n{via.Name}:\n";
                    diagMsg += $"  Blocks: {via.Blocks.Count}\n";
                    foreach (var block in via.Blocks)
                    {
                        int vachCount = block.Vach.SelectedGeometry.Count;
                        int truCount = block.Tru.SelectedGeometry.Count;
                        diagMsg += $"    {block.Name}:\n";
                        diagMsg += $"      Vách: {vachCount} lines\n";
                        
                        // Check if geometry references are populated
                        if (vachCount > 0)
                        {
                            var firstVach = block.Vach.SelectedGeometry.First();
                            diagMsg += $"        Sample: Handle={firstVach.Handle}, Layer={firstVach.Layer}, DWG={firstVach.SourceDwgName}\n";
                        }
                        
                        diagMsg += $"      Trụ: {truCount} lines\n";
                        
                        if (truCount > 0)
                        {
                            var firstTru = block.Tru.SelectedGeometry.First();
                            diagMsg += $"        Sample: Handle={firstTru.Handle}, Layer={firstTru.Layer}, DWG={firstTru.SourceDwgName}\n";
                        }
                        
                        totalVach += vachCount;
                        totalTru += truCount;
                    }
                }
                
                diagMsg += $"\n\nTOTAL: Vách={totalVach}, Trụ={totalTru}, Grand Total={totalVach + totalTru}";
                
                System.Diagnostics.Debug.WriteLine(diagMsg);
                MessageBox.Show(diagMsg, "Debug - Project State", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            var dialog = new ExportMultipleDataDialog($"Chọn {category} để export", items, category);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ExportMultipleToFile(category, dialog.SelectedItems, dialog.MapName);
            }
        }

        private List<string> GetItemsByCategory(string category)
        {
            switch (category)
            {
                case "Vỉa": return _project.Vias.Select(v => v.Name).ToList();
                case "Đứt gãy": return _project.Faults.Select(f => f.Name).ToList();
                case "Nham thạch": return _project.Rocks.Select(r => r.Name).ToList();
                case "Lỗ khoan": return _project.Boreholes.Select(b => b.Name).ToList();
                default: return new List<string>();
            }
        }

        private async void SendMultipleToServer(string category, List<string> selectedNames, string mapName, string ip, string port)
        {
            await _exportService.SendToServer(category, selectedNames, mapName, ip, port, _project, this);
        }

        private async void ExportMultipleToFile(string category, List<string> selectedNames, string mapName)
        {
            await _exportService.ExportToFile(category, selectedNames, mapName, _project);
        }

        private void RebuildTreeView()
        {
            treeView.Nodes.Clear();

            TreeNode rootVias = new TreeNode("Danh sách Vỉa");
            rootVias.Tag = "RootVias";
            treeView.Nodes.Add(rootVias);

            TreeNode rootFaults = new TreeNode("Danh sách Đứt gãy");
            rootFaults.Tag = "RootFaults";
            treeView.Nodes.Add(rootFaults);

            TreeNode rootRocks = new TreeNode("Danh sách Nham thạch");
            rootRocks.Tag = "RootRocks";
            treeView.Nodes.Add(rootRocks);

            TreeNode rootBoreholes = new TreeNode("Danh sách Lỗ khoan");
            rootBoreholes.Tag = "RootBoreholes";
            treeView.Nodes.Add(rootBoreholes);

            foreach (var via in _project.Vias)
            {
                TreeNode viaNode = new TreeNode(via.Name);
                viaNode.Tag = via;
                rootVias.Nodes.Add(viaNode);

                foreach (var khoi in via.Blocks)
                {
                    TreeNode blockNode = new TreeNode(khoi.Name);
                    blockNode.Tag = khoi;

                    TreeNode vachNode = new TreeNode("Vách");
                    vachNode.Tag = khoi.Vach;

                    TreeNode truNode = new TreeNode("Trụ");
                    truNode.Tag = khoi.Tru;

                    blockNode.Nodes.Add(vachNode);
                    blockNode.Nodes.Add(truNode);
                    viaNode.Nodes.Add(blockNode);
                }
            }

            foreach (var fault in _project.Faults)
            {
                TreeNode node = new TreeNode(fault.Name);
                node.Tag = fault;
                rootFaults.Nodes.Add(node);
            }

            foreach (var rock in _project.Rocks)
            {
                TreeNode node = new TreeNode(rock.Name);
                node.Tag = rock;
                rootRocks.Nodes.Add(node);
            }

            foreach (var borehole in _project.Boreholes)
            {
                TreeNode node = new TreeNode(borehole.Name);
                node.Tag = borehole;
                rootBoreholes.Nodes.Add(node);
            }

            treeView.ExpandAll();
        }
    }
}
