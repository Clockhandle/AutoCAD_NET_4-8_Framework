using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyMiningPlugin.Models;
using MyMiningPlugin.Services;
using MyMiningPlugin.UI;

namespace AutoCAD_NET_4_8_Framework
{
    public partial class MiningManagerDForm : Form
    {
        // Data Container
        private MiningProject _project = new MiningProject();

        // Services
        private AutoCADSelectionService _selectionService;
        private GeometryProcessingService _geometryProcessor;
        private PersistenceService _persistenceService;
        private ExportService _exportService;

        private TextBox txtServerUrl;

        public MiningManagerDForm()
        {
            _selectionService = new AutoCADSelectionService();
            _geometryProcessor = new GeometryProcessingService(_selectionService);
            _persistenceService = new PersistenceService();
            _exportService = new ExportService(_geometryProcessor);

            InitializeComponent();
            
            this.Text = "MineTerra3D";
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Size = new System.Drawing.Size(1024, 700);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            
            // Wire up the TreeView selection event
            treeView.AfterSelect += TreeView_AfterSelect;
            treeView.LabelEdit = true;
            treeView.BeforeLabelEdit += TreeView_BeforeLabelEdit;
            treeView.AfterLabelEdit += TreeView_AfterLabelEdit;
            treeView.NodeMouseClick += TreeView_NodeMouseClick;
            
            // Try loading saved project silently (no MessageBox during construction)
            var loadedProject = _persistenceService.LoadProjectData(silent: true);
            if (loadedProject != null)
            {
                _project = loadedProject;
            }
            RebuildTreeView();

            Label lblServerUrl = new Label
            {
                Text = "Server URL:",
                Location = new Point(20, 405),
                AutoSize = true
            };
            this.Controls.Add(lblServerUrl);

            txtServerUrl = new TextBox
            {
                Location = new Point(20, 425),
                Size = new Size(390, 25),
                Text = "http://mica.edu.vn:55322/"
            };
            this.Controls.Add(txtServerUrl);
        }

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            rightPanel.Controls.Clear();
            
            string nodeTag = e.Node.Tag as string;

            if (nodeTag == "RootVias" || nodeTag == "RootFaults" || nodeTag == "RootRocks" || nodeTag == "RootBoreholes" || nodeTag == "RootBeMats" || nodeTag == "RootMineTopologies" || nodeTag == "RootGioiHans")
            {
                // 1. Create the instance of the UserControl
                UCRootCategory rootUc = new UCRootCategory { Dock = DockStyle.Fill };

                // 2. Configure it based on the exact type of root clicked
                switch (nodeTag)
                {
                    case "RootVias":
                        rootUc.LoadData(
                            title: "Quản lý Vỉa",
                            addBtnText: "+ Thêm Vỉa Mới",
                            onAddNew: () => AddNewVia(),
                            onSendToServer: () => ShowSendMultipleDialog("Vỉa"),
                            onExportJson: () => ShowExportMultipleDialog("Vỉa"),
                            onSaveProject: () => SaveProjectData(),
                            onLoadProject: () => LoadProjectData()
                        );
                        break;

                    case "RootBeMats":
                        var beMatUC = rootUc; // capture ref for async callback
                        rootUc.LoadData(
                            title: "Quản lý Bề mặt",
                            addBtnText: "+ Thêm Bề mặt Mới",
                            onAddNew: () => AddNewBeMat(),
                            onSendToServer: () => ShowSendMultipleDialog("Bề mặt"),
                            onExportJson: () => ShowExportMultipleDialog("Bề mặt"),
                            onSaveProject: () => SaveProjectData(),
                            onLoadProject: () => LoadProjectData(),
                            onRunDQ: async () =>
                            {
                                var names = _project.BeMats.Select(b => b.Name).ToList();
                                string summary = await _exportService.RunDQChecksForBeMats(
                                    _project.BeMats, names, _project);
                                beMatUC.SetMarkersActive(true);
                                MessageBox.Show(summary, "Kết quả kiểm tra DQ",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            },
                            onClearMarkers: () =>
                            {
                                string result = _exportService.ClearQualityMarkers();
                                beMatUC.SetMarkersActive(false);
                                MessageBox.Show(result, "Xóa marker",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        );
                        break;

                    case "RootFaults":
                        rootUc.LoadData(
                            title: "Quản lý Đứt gãy",
                            addBtnText: "+ Thêm Đứt gãy Mới",
                            onAddNew: () => AddNewFault(), 
                            onSendToServer: () => ShowSendMultipleDialog("Đứt gãy"),
                            onExportJson: () => ShowExportMultipleDialog("Đứt gãy"),
                            onSaveProject: () => SaveProjectData(),
                            onLoadProject: () => LoadProjectData()
                        );
                        break;

                    case "RootBoreholes":
                        rootUc.LoadData(
                            title: "Quản lý Lỗ khoan",
                            addBtnText: "+ Thêm Lỗ khoan Mới",
                            onAddNew: () => AddNewBorehole(),
                            onSendToServer: () => ShowSendMultipleDialog("Lỗ khoan"),
                            onExportJson: () => ShowExportMultipleDialog("Lỗ khoan"),
                            onSaveProject: () => SaveProjectData(),
                            onLoadProject: () => LoadProjectData()
                        );
                        break;

                    case "RootMineTopologies":
                        rootUc.LoadData(
                            title: "Địa hình lò (Nền / Nóc / Biên)",
                            addBtnText: "+ Thêm địa hình lò",
                            onAddNew: () => AddNewMineTopology(),
                            onSendToServer: () => ShowSendMultipleDialog("Địa hình lò"),
                            onExportJson: () => ShowExportMultipleDialog("Địa hình lò"),
                            onSaveProject: () => SaveProjectData(),
                            onLoadProject: () => LoadProjectData()
                        );
                        break;

                    case "RootGioiHans":
                        rootUc.LoadData(
                            title: "Giới hạn cấp phép",
                            addBtnText: "+ Thêm Giới hạn Mới",
                            onAddNew: () => AddNewGioiHan(),
                            onSendToServer: () => ShowSendMultipleDialog("Giới hạn"),
                            onExportJson: () => ShowExportMultipleDialog("Giới hạn"),
                            onSaveProject: () => SaveProjectData(),
                            onLoadProject: () => LoadProjectData()
                        );
                        break;
                        
                    case "RootRocks":
                        rootUc.LoadData(
                            title: "Quản lý Nham thạch",
                            addBtnText: "+ Thêm Nham thạch Mới",
                            onAddNew: () => AddNewRock(),
                            onSendToServer: () => ShowSendMultipleDialog("Nham thạch"),
                            onExportJson: () => ShowExportMultipleDialog("Nham thạch"),
                            onSaveProject: () => SaveProjectData(),
                            onLoadProject: () => LoadProjectData()
                        );
                        break;
                }

                // 3. Add to panel
                rightPanel.Controls.Add(rootUc);
            }
            else if (e.Node.Tag is ViaData via)
            {
                UCVia ucVia = new UCVia { Dock = DockStyle.Fill };
                ucVia.LoadData(via,
                    onAddBlock: () => AddNewBlock(e.Node),
                    onDeleteVia: () => { e.Node.Remove(); _project.Vias.Remove(via); rightPanel.Controls.Clear(); }
                );
                rightPanel.Controls.Add(ucVia);
            }
            else if (e.Node.Tag is KhoiData khoi)
            {
                UCKhoi ucKhoi = new UCKhoi { Dock = DockStyle.Fill };
                ucKhoi.LoadData(khoi,
                    onDeleteKhoi: () =>
                    {
                        ViaData parentVia = e.Node.Parent?.Tag as ViaData;
                        parentVia?.Blocks.Remove(khoi);
                        e.Node.Remove();
                        rightPanel.Controls.Clear();
                    }
                );
                rightPanel.Controls.Add(ucKhoi);
            }
            else if (e.Node.Tag is BoreholeData borehole)
            {
                UCBoreholes ucBorehole = new UCBoreholes { Dock = DockStyle.Fill };
                ucBorehole.LoadData(borehole,
                    onSelectExcel: () => {
                        using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Excel Files|*.xls;*.xlsx;*.xlsm" })
                        {
                            if (ofd.ShowDialog() == DialogResult.OK)
                            {
                                borehole.ExcelFilePath = ofd.FileName;
                                TreeView_AfterSelect(sender, e); // Refresh UI to show the new path string
                            }
                        }
                    },
                    onParseExcel: () => {
                        if (string.IsNullOrEmpty(borehole.ExcelFilePath) || !System.IO.File.Exists(borehole.ExcelFilePath))
                        {
                            MessageBox.Show("Vui lòng chọn file Excel hợp lệ trước.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        try { MyMiningPlugin.Services.BoreholeExcelParser.ParseAllBoreholes(borehole.ExcelFilePath); }
                        catch (Exception ex) { MessageBox.Show($"Lỗi đọc file Excel:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    },
                    onDelete: () => { e.Node.Remove(); _project.Boreholes.Remove(borehole); rightPanel.Controls.Clear(); }
                );
                rightPanel.Controls.Add(ucBorehole);
            }
            else if (e.Node.Tag is BeMatData bemat)
            {
                UCSurface ucSurface = new UCSurface { Dock = DockStyle.Fill };
                RenderSurfaceLogic(ucSurface, bemat.Surface, e.Node, isDeletable: true, onDelete: () => { e.Node.Remove(); _project.BeMats.Remove(bemat); rightPanel.Controls.Clear(); });
                rightPanel.Controls.Add(ucSurface);
            }
            else if (e.Node.Tag is FaultData fault)
            {
                UCSurface ucSurface = new UCSurface { Dock = DockStyle.Fill };
                RenderSurfaceLogic(ucSurface, fault.Surface, e.Node, isDeletable: true, onDelete: () => { e.Node.Remove(); _project.Faults.Remove(fault); });
                rightPanel.Controls.Add(ucSurface);
            }
            else if (e.Node.Tag is RockData rock)
            {
                UCSurface ucSurface = new UCSurface { Dock = DockStyle.Fill };
                RenderSurfaceLogic(ucSurface, rock.Surface, e.Node, isDeletable: true, onDelete: () => { e.Node.Remove(); _project.Rocks.Remove(rock); });
                rightPanel.Controls.Add(ucSurface);
            }
            else if (e.Node.Tag is SurfaceData surface)
            {
                UCSurface ucSurface = new UCSurface { Dock = DockStyle.Fill };
                RenderSurfaceLogic(ucSurface, surface, e.Node, isDeletable: false, onDelete: null);
                if (surface.Type == "Đứt gãy")
                    ucSurface.HideExtraSections();
                rightPanel.Controls.Add(ucSurface);
            }
            else if (e.Node.Tag is MineTopologyData topo)
            {
                _selectionService.ResolveCurrentDrawingReferences(topo);
                UCMineTopology ucTopo = new UCMineTopology { Dock = DockStyle.Fill };
                var topoNode = e.Node;
                ucTopo.LoadData(topo,
                    onSelectNen: () => {
                        this.Hide();
                        _selectionService.SelectLinesToList(topo.Nen, "Nền");
                        this.Show();
                        TreeView_AfterSelect(sender, new TreeViewEventArgs(topoNode));
                    },
                    onClearNen: () => { topo.Nen.Clear(); TreeView_AfterSelect(sender, new TreeViewEventArgs(topoNode)); },
                    onSelectNoc: () => {
                        this.Hide();
                        _selectionService.SelectLinesToList(topo.Noc, "Nóc");
                        this.Show();
                        TreeView_AfterSelect(sender, new TreeViewEventArgs(topoNode));
                    },
                    onClearNoc: () => { topo.Noc.Clear(); TreeView_AfterSelect(sender, new TreeViewEventArgs(topoNode)); },
                    onSelectBien: () => {
                        this.Hide();
                        _selectionService.SelectLinesToList(topo.Bien, "Biên");
                        this.Show();
                        TreeView_AfterSelect(sender, new TreeViewEventArgs(topoNode));
                    },
                    onClearBien: () => { topo.Bien.Clear(); TreeView_AfterSelect(sender, new TreeViewEventArgs(topoNode)); }
                );
                rightPanel.Controls.Add(ucTopo);
            }
            else if (e.Node.Tag is GioiHanData gioiHan)
            {
                UCGioiHan ucGioiHan = new UCGioiHan { Dock = DockStyle.Fill };
                ucGioiHan.LoadData(gioiHan,
                    onAddBlock: () => AddNewGioiHanBlock(e.Node),
                    onDeleteGioiHan: () => { e.Node.Remove(); _project.GioiHans.Remove(gioiHan); rightPanel.Controls.Clear(); }
                );
                rightPanel.Controls.Add(ucGioiHan);
            }
            else if (e.Node.Tag is GioiHanKhoiData gioiHanKhoi)
            {
                UCGioiHanKhoi ucGioiHanKhoi = new UCGioiHanKhoi { Dock = DockStyle.Fill };
                ucGioiHanKhoi.LoadData(gioiHanKhoi,
                    onDeleteKhoi: () =>
                    {
                        GioiHanData parentGioiHan = e.Node.Parent?.Tag as GioiHanData;
                        parentGioiHan?.Blocks.Remove(gioiHanKhoi);
                        e.Node.Remove();
                        rightPanel.Controls.Clear();
                    }
                );
                rightPanel.Controls.Add(ucGioiHanKhoi);
            }
        }

        private void RenderSurfaceLogic(UCSurface ucSurface, SurfaceData surface, TreeNode node, bool isDeletable, Action onDelete)
        {
            _selectionService.ResolveCurrentDrawingReferences(surface);

            bool isPointMode = surface.Type == "Bề mặt";

            ucSurface.LoadData(surface, isDeletable,
                onSelectLines: () => {
                    this.Hide();
                    if (isPointMode)
                        _selectionService.SelectPointsFromAutoCAD(surface);
                    else
                        _selectionService.SelectLinesFromAutoCAD(surface);
                    this.Show();
                    TreeView_AfterSelect(null, new TreeViewEventArgs(node));
                },
                onClearSurfaceLines: () => {
                    surface.SelectedGeometry.Clear();
                    TreeView_AfterSelect(null, new TreeViewEventArgs(node));
                },
                onClearBoundaryLines: () => {
                    if (surface.BoundaryGeometry != null) surface.BoundaryGeometry.Clear();
                    TreeView_AfterSelect(null, new TreeViewEventArgs(node));
                },
                onSelectBoundaryLines: () => {
                    this.Hide();
                    _selectionService.SelectBorderlinesFromAutoCAD(surface);
                    this.Show();
                    TreeView_AfterSelect(null, new TreeViewEventArgs(node));
                },
                onSelectHoles: () => {
                    this.Hide();
                    _selectionService.SelectHolesFromAutoCAD(surface);
                    this.Show();
                    TreeView_AfterSelect(null, new TreeViewEventArgs(node));
                },
                onClearHoleLines: () => {
                    if (surface.HoleGeometry != null) surface.HoleGeometry.Clear();
                    TreeView_AfterSelect(null, new TreeViewEventArgs(node));
                },
                onSelectBreakLines: () => {
                    this.Hide();
                    _selectionService.SelectBreakLinesFromAutoCAD(surface);
                    this.Show();
                    TreeView_AfterSelect(null, new TreeViewEventArgs(node));
                },
                onClearBreakLines: () => {
                    if (surface.BreaklineGeometry != null) surface.BreaklineGeometry.Clear();
                    TreeView_AfterSelect(null, new TreeViewEventArgs(node));
                }
            );
        }

        // --- INJECTED LOGIC METHODS ---

        private void AddNewVia(string name = null)
        {
            string viaName = name ?? $"Vỉa {_project.Vias.Count + 1}";
            ViaData newVia = new ViaData { Name = viaName };
            _project.Vias.Add(newVia);

            TreeNode node = new TreeNode(viaName);
            node.Tag = newVia;

            // Notice we assume your treeview in Designer is named 'treeView'
            if (treeView != null)
            {
                foreach (TreeNode n in treeView.Nodes)
                {
                    if (n.Tag as string == "RootVias")
                    {
                        n.Nodes.Add(node);
                        n.Expand();
                        break;
                    }
                }
            }
            
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
                Tru = new SurfaceData { Type = "Trụ", ParentName = $"{via.Name} - {blockName}" },
                DutGay = new SurfaceData { Type = "Đứt gãy", ParentName = $"{via.Name} - {blockName}" }
            };
            via.Blocks.Add(newBlock);

            TreeNode blockNode = new TreeNode(blockName);
            blockNode.Tag = newBlock;

            TreeNode vachNode = new TreeNode("Vách");
            vachNode.Tag = newBlock.Vach;

            TreeNode truNode = new TreeNode("Trụ");
            truNode.Tag = newBlock.Tru;

            TreeNode dutGayNode = new TreeNode("Đứt gãy");
            dutGayNode.Tag = newBlock.DutGay;

            blockNode.Nodes.Add(vachNode);
            blockNode.Nodes.Add(truNode);
            blockNode.Nodes.Add(dutGayNode);
            viaNode.Nodes.Add(blockNode);
            viaNode.Expand();
        }

        private void AddNewBeMat()
        {
            string bematName = $"Bề mặt {_project.BeMats.Count + 1}";
            BeMatData newBeMat = new BeMatData
            {
                Name = bematName,
                Surface = new SurfaceData { Type = "Bề mặt", ParentName = bematName }
            };
            _project.BeMats.Add(newBeMat);

            TreeNode node = new TreeNode(bematName);
            node.Tag = newBeMat;

            if (treeView != null)
            {
                foreach (TreeNode n in treeView.Nodes)
                {
                    if (n.Tag as string == "RootBeMats")
                    {
                        n.Nodes.Add(node);
                        n.Expand();
                        break;
                    }
                }
            }
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

            if (treeView != null)
            {
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

            if (treeView != null)
            {
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

            if (treeView != null)
            {
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
        }

        private void AddNewMineTopology()
        {
            string topoName = $"Địa hình lò {_project.MineTopologies.Count + 1}";
            MineTopologyData newTopo = new MineTopologyData { Name = topoName };
            _project.MineTopologies.Add(newTopo);

            TreeNode node = new TreeNode(topoName);
            node.Tag = newTopo;

            if (treeView != null)
            {
                foreach (TreeNode n in treeView.Nodes)
                {
                    if (n.Tag as string == "RootMineTopologies")
                    {
                        n.Nodes.Add(node);
                        n.Expand();
                        break;
                    }
                }
            }
        }

        private void AddNewGioiHan()
        {
            string ghName = $"Giới hạn {_project.GioiHans.Count + 1}";
            GioiHanData newGioiHan = new GioiHanData { Name = ghName };
            _project.GioiHans.Add(newGioiHan);

            TreeNode node = new TreeNode(ghName);
            node.Tag = newGioiHan;

            if (treeView != null)
            {
                foreach (TreeNode n in treeView.Nodes)
                {
                    if (n.Tag as string == "RootGioiHans")
                    {
                        n.Nodes.Add(node);
                        n.Expand();
                        break;
                    }
                }
            }

            AddNewGioiHanBlock(node);
        }

        private void AddNewGioiHanBlock(TreeNode ghNode)
        {
            GioiHanData gioiHan = ghNode.Tag as GioiHanData;
            if (gioiHan == null) return;

            string blockName = $"Vùng {gioiHan.Blocks.Count + 1}";
            GioiHanKhoiData newKhoi = new GioiHanKhoiData
            {
                Name = blockName,
                Vach = new SurfaceData { Type = "Vách", ParentName = $"{gioiHan.Name} - {blockName}" },
                Tru  = new SurfaceData { Type = "Trụ",  ParentName = $"{gioiHan.Name} - {blockName}" }
            };
            gioiHan.Blocks.Add(newKhoi);

            TreeNode blockNode = new TreeNode(blockName);
            blockNode.Tag = newKhoi;

            TreeNode vachNode = new TreeNode("Vách");
            vachNode.Tag = newKhoi.Vach;

            TreeNode truNode = new TreeNode("Trụ");
            truNode.Tag = newKhoi.Tru;

            blockNode.Nodes.Add(vachNode);
            blockNode.Nodes.Add(truNode);
            ghNode.Nodes.Add(blockNode);
            ghNode.Expand();
        }

        private void SaveProjectData()
        {
            _persistenceService.SaveProjectData(_project);
        }

        private void LoadProjectData()
        {
            var loadedProject = _persistenceService.LoadProjectData(silent: false);
            if (loadedProject != null)
            {
                _project = loadedProject;
                RebuildTreeView();
            }
        }

        private void TreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            // Only show context menu for renameable/deletable nodes (not root/category string tags)
            if (e.Node.Tag is string)
                return;

            treeView.SelectedNode = e.Node;

            ContextMenuStrip menu = new ContextMenuStrip();

            ToolStripMenuItem renameItem = new ToolStripMenuItem("Đổi tên");
            renameItem.Click += (s, args) => e.Node.BeginEdit();
            menu.Items.Add(renameItem);

            // Only offer delete for leaf data nodes that have a clear parent-list owner
            bool isDeletable = e.Node.Tag is ViaData
                            || e.Node.Tag is FaultData
                            || e.Node.Tag is RockData
                            || e.Node.Tag is BeMatData
                            || e.Node.Tag is BoreholeData
                            || e.Node.Tag is MineTopologyData
                            || e.Node.Tag is GioiHanData;

            if (isDeletable)
            {
                menu.Items.Add(new ToolStripSeparator());
                ToolStripMenuItem deleteItem = new ToolStripMenuItem("Xóa");
                deleteItem.ForeColor = System.Drawing.Color.Red;
                deleteItem.Click += (s, args) =>
                {
                    string itemName = e.Node.Text;
                    var confirm = MessageBox.Show(
                        $"Xóa \"{itemName}\"?", "Xác nhận xóa",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (confirm != DialogResult.Yes) return;

                    if (e.Node.Tag is ViaData via)
                        _project.Vias.Remove(via);
                    else if (e.Node.Tag is FaultData fault)
                        _project.Faults.Remove(fault);
                    else if (e.Node.Tag is RockData rock)
                        _project.Rocks.Remove(rock);
                    else if (e.Node.Tag is BeMatData bemat)
                        _project.BeMats.Remove(bemat);
                    else if (e.Node.Tag is BoreholeData borehole)
                        _project.Boreholes.Remove(borehole);
                    else if (e.Node.Tag is MineTopologyData topo)
                        _project.MineTopologies.Remove(topo);
                    else if (e.Node.Tag is GioiHanData gioiHan)
                        _project.GioiHans.Remove(gioiHan);

                    e.Node.Remove();
                    rightPanel.Controls.Clear();
                };
                menu.Items.Add(deleteItem);
            }

            menu.Show(treeView, e.Location);
        }

        private void TreeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            // Block editing on root/category nodes
            if (e.Node.Tag is string)
                e.CancelEdit = true;
        }

        private void TreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.CancelEdit || string.IsNullOrWhiteSpace(e.Label))
            {
                e.CancelEdit = true;
                return;
            }

            string newName = e.Label.Trim();
            var node = e.Node;

            if (node.Tag is ViaData via)
                via.Name = newName;
            else if (node.Tag is FaultData fault)
            {
                fault.Name = newName;
                fault.Surface.ParentName = newName;
            }
            else if (node.Tag is RockData rock)
            {
                rock.Name = newName;
                rock.Surface.ParentName = newName;
            }
            else if (node.Tag is BeMatData bemat)
            {
                bemat.Name = newName;
                bemat.Surface.ParentName = newName;
            }
            else if (node.Tag is BoreholeData borehole)
                borehole.Name = newName;
            else if (node.Tag is MineTopologyData topo)
                topo.Name = newName;
            else if (node.Tag is GioiHanData gioiHan)
                gioiHan.Name = newName;
            else if (node.Tag is GioiHanKhoiData gioiHanKhoi)
                gioiHanKhoi.Name = newName;
            else if (node.Tag is KhoiData khoi)
                khoi.Name = newName;
            else
            {
                e.CancelEdit = true;
                return;
            }

            // Allow the node label to update visually
            node.EndEdit(false);
            e.CancelEdit = true; // we set the label via node.Text ourselves below
            node.Text = newName;
        }

        private void ShowSendMultipleDialog(string category)
        {
            List<string> items = GetItemsByCategory(category);
            if (items.Count == 0)
            {
                MessageBox.Show($"Chưa có {category} nào!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var dialog = new SendMultipleDataDialog($"Chọn {category} để gửi", items, category, GetCurrentDrawingName());
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SendMultipleToServer(category, dialog.SelectedItems, dialog.MapName, dialog.ServerUrl, dialog.SelectedDate);
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

            var dialog = new ExportMultipleDataDialog($"Chọn {category} để export", items, category, GetCurrentDrawingName());
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ExportMultipleToFile(category, dialog.SelectedItems, dialog.MapName);
            }
        }

        private string GetCurrentDrawingName()
        {
            try
            {
                var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                if (doc != null && !string.IsNullOrEmpty(doc.Database.Filename))
                    return System.IO.Path.GetFileNameWithoutExtension(doc.Database.Filename);
            }
            catch { }
            return string.Empty;
        }

        private List<string> GetItemsByCategory(string category)
        {
            switch (category)
            {
                case "Vỉa": return _project.Vias.Select(v => v.Name).ToList();
                case "Bề mặt": return _project.BeMats.Select(b => b.Name).ToList();
                case "Đứt gãy": return _project.Faults.Select(f => f.Name).ToList();
                case "Nham thạch": return _project.Rocks.Select(r => r.Name).ToList();
                case "Lỗ khoan": return _project.Boreholes.Select(b => b.Name).ToList();
                case "Địa hình lò": return _project.MineTopologies.Select(t => t.Name).ToList();
                case "Giới hạn": return _project.GioiHans.Select(g => g.Name).ToList();
                default: return new List<string>();
            }
        }

        private async void SendMultipleToServer(string category, List<string> selectedNames, string mapName, string serverUrl, DateTime? date = null)
        {
            try
            {
                await _exportService.SendToServer(category, selectedNames, mapName, serverUrl, _project, this, date);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi gửi dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ExportMultipleToFile(string category, List<string> selectedNames, string mapName)
        {
            try
            {
                await _exportService.ExportToFile(category, selectedNames, mapName, _project);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi export: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RebuildTreeView()
        {
            if (treeView == null) return;
            treeView.Nodes.Clear();

            TreeNode rootVias = new TreeNode("Danh sách Vỉa");
            rootVias.Tag = "RootVias";
            treeView.Nodes.Add(rootVias);

            TreeNode rootBeMats = new TreeNode("Danh sách Bề mặt");
            rootBeMats.Tag = "RootBeMats";
            treeView.Nodes.Add(rootBeMats);

            TreeNode rootFaults = new TreeNode("Danh sách Đứt gãy");
            rootFaults.Tag = "RootFaults";
            treeView.Nodes.Add(rootFaults);

            TreeNode rootRocks = new TreeNode("Danh sách Nham thạch");
            rootRocks.Tag = "RootRocks";
            treeView.Nodes.Add(rootRocks);

            TreeNode rootBoreholes = new TreeNode("Danh sách Lỗ khoan");
            rootBoreholes.Tag = "RootBoreholes";
            treeView.Nodes.Add(rootBoreholes);

            TreeNode rootMineTopologies = new TreeNode("Địa hình lò");
            rootMineTopologies.Tag = "RootMineTopologies";
            treeView.Nodes.Add(rootMineTopologies);

            TreeNode rootGioiHans = new TreeNode("Giới hạn cấp phép");
            rootGioiHans.Tag = "RootGioiHans";
            treeView.Nodes.Add(rootGioiHans);

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

                    TreeNode dutGayNode = new TreeNode("Đứt gãy");
                    dutGayNode.Tag = khoi.DutGay ?? new SurfaceData { Type = "Đứt gãy", ParentName = khoi.Name };

                    blockNode.Nodes.Add(vachNode);
                    blockNode.Nodes.Add(truNode);
                    blockNode.Nodes.Add(dutGayNode);
                    viaNode.Nodes.Add(blockNode);
                }
            }

            foreach (var bemat in _project.BeMats)
            {
                TreeNode node = new TreeNode(bemat.Name);
                node.Tag = bemat;
                rootBeMats.Nodes.Add(node);
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

            foreach (var topo in _project.MineTopologies)
            {
                TreeNode node = new TreeNode(topo.Name);
                node.Tag = topo;
                rootMineTopologies.Nodes.Add(node);
            }

            foreach (var gioiHan in _project.GioiHans)
            {
                TreeNode ghNode = new TreeNode(gioiHan.Name);
                ghNode.Tag = gioiHan;
                foreach (var khoi in gioiHan.Blocks)
                {
                    TreeNode blockNode = new TreeNode(khoi.Name);
                    blockNode.Tag = khoi;
                    TreeNode vachNode = new TreeNode("Vách");
                    vachNode.Tag = khoi.Vach;
                    TreeNode truNode = new TreeNode("Trụ");
                    truNode.Tag = khoi.Tru;
                    blockNode.Nodes.Add(vachNode);
                    blockNode.Nodes.Add(truNode);
                    ghNode.Nodes.Add(blockNode);
                }
                rootGioiHans.Nodes.Add(ghNode);
            }

            treeView.ExpandAll();
        }
    }
}
