using AutoCAD_NET_4_8_Framework;
using MyMiningPlugin.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyMiningPlugin.Services
{
    /// <summary>
    /// Handles JSON generation and export/upload functionality
    /// </summary>
    public class ExportService
    {
        private readonly GeometryProcessingService _geometryProcessor;
        private readonly QualityMarkerService _markerService = new QualityMarkerService();
        private static readonly HttpClient client = new HttpClient();

        public ExportService(GeometryProcessingService geometryProcessor)
        {
            _geometryProcessor = geometryProcessor;
        }

        /// <summary>
        /// Generate combined JSON payload for multiple items
        /// </summary>
        public async Task<string> GenerateCombinedJsonPayload(
            string category, 
            List<string> selectedNames, 
            string mapName,
            MiningProject project,
            DateTime? date = null)
        {
            var flattenedItems = new List<object>();

            switch (category)
            {
                case "Vỉa":
                    await ProcessVias(project.Vias, selectedNames, mapName, flattenedItems);
                    break;

                case "Đứt gãy":
                    await ProcessFaults(project.Faults, selectedNames, mapName, flattenedItems);
                    break;

                case "Nham thạch":
                    await ProcessRocks(project.Rocks, selectedNames, mapName, flattenedItems);
                    break;

                case "Lỗ khoan":
                    await ProcessBoreholes(project.Boreholes, selectedNames, mapName, flattenedItems);
                    break;

                case "Bề mặt":
                    bool proceed = await ProcessBeMats(project.BeMats, selectedNames, mapName, flattenedItems);
                    if (!proceed) return null; // quality errors — caller must abort
                    break;

                case "Địa hình lò":
                    await ProcessMineTopologies(project.MineTopologies, selectedNames, mapName, flattenedItems, date);
                    break;

                case "Địa hình lò Loại 2":
                    await ProcessMineTopologies2(project.MineTopologies2, selectedNames, mapName, flattenedItems);
                    break;

                case "Giới hạn":
                    await ProcessGioiHans(project.GioiHans, selectedNames, mapName, flattenedItems);
                    break;
            }

            return JsonConvert.SerializeObject(flattenedItems, Formatting.Indented);
        }

        /// <summary>
        /// Export JSON to file
        /// </summary>
        public async Task ExportToFile(string category, List<string> selectedNames, string mapName, MiningProject project)
        {
            try
            {
                // CRITICAL: Resolve all geometry references BEFORE async processing
                ResolveAllGeometryReferences(category, selectedNames, project);
                
                // Add diagnostic info before generating JSON
                int totalGeometryCount = 0;
                int resolvedGeometryCount = 0;
                string debugDetails = "";
                
                // Diagnostic for ALL categories
                switch (category)
                {
                    case "Vỉa":
                        debugDetails += $"Số Vỉa trong project: {project.Vias.Count}\n";
                        foreach (var name in selectedNames)
                        {
                            var via = project.Vias.FirstOrDefault(v => v.Name == name);
                            if (via != null)
                            {
                                debugDetails += $"- Vỉa '{via.Name}': {via.Blocks.Count} blocks\n";
                                foreach (var khoi in via.Blocks)
                                {
                                    int vachCount = khoi.Vach.SelectedGeometry.Count;
                                    int vachResolvedCount = khoi.Vach.SelectedGeometry.Count(g => g.CurrentObjectId.HasValue && !g.CurrentObjectId.Value.IsNull);
                                    int truCount = khoi.Tru.SelectedGeometry.Count;
                                    int truResolvedCount = khoi.Tru.SelectedGeometry.Count(g => g.CurrentObjectId.HasValue && !g.CurrentObjectId.Value.IsNull);
                                    debugDetails += $"  - {khoi.Name}: Vách={vachCount} (resolved={vachResolvedCount}), Trụ={truCount} (resolved={truResolvedCount})\n";
                                    totalGeometryCount += vachCount + truCount;
                                    resolvedGeometryCount += vachResolvedCount + truResolvedCount;
                                }
                            }
                            else
                            {
                                debugDetails += $"- Vỉa '{name}': KHÔNG TÌM THẤY!\n";
                            }
                        }
                        break;

                    case "Đứt gãy":
                        debugDetails += $"Số Đứt gãy trong project: {project.Faults.Count}\n";
                        foreach (var name in selectedNames)
                        {
                            var fault = project.Faults.FirstOrDefault(f => f.Name == name);
                            if (fault != null)
                            {
                                int count = fault.Surface.SelectedGeometry.Count;
                                int resolvedCount = fault.Surface.SelectedGeometry.Count(g => g.CurrentObjectId.HasValue && !g.CurrentObjectId.Value.IsNull);
                                debugDetails += $"- Đứt gãy '{fault.Name}': {count} lines (resolved={resolvedCount})\n";
                                totalGeometryCount += count;
                                resolvedGeometryCount += resolvedCount;
                            }
                        }
                        break;

                    case "Nham thạch":
                        debugDetails += $"Số Nham thạch trong project: {project.Rocks.Count}\n";
                        foreach (var name in selectedNames)
                        {
                            var rock = project.Rocks.FirstOrDefault(r => r.Name == name);
                            if (rock != null)
                            {
                                int count = rock.Surface.SelectedGeometry.Count;
                                int resolvedCount = rock.Surface.SelectedGeometry.Count(g => g.CurrentObjectId.HasValue && !g.CurrentObjectId.Value.IsNull);
                                debugDetails += $"- Nham thạch '{rock.Name}': {count} lines (resolved={resolvedCount})\n";
                                totalGeometryCount += count;
                                resolvedGeometryCount += resolvedCount;
                            }
                        }
                        break;

                    case "Lỗ khoan":
                        debugDetails += $"Số Lỗ khoan trong project: {project.Boreholes.Count}\n";
                        foreach (var name in selectedNames)
                        {
                            var borehole = project.Boreholes.FirstOrDefault(b => b.Name == name);
                            if (borehole != null)
                            {
                                int count = borehole.Trajectory.Count; // Count survey points
                                int resolvedCount = count; // Always resolved for Excel data
                                debugDetails += $"- Lỗ khoan '{borehole.Name}': {count} survey points\n";
                                totalGeometryCount += count;
                                resolvedGeometryCount += resolvedCount;
                            }
                        }
                        break;

                    case "Bề mặt":
                        debugDetails += $"Số Bề mặt trong project: {project.BeMats.Count}\n";
                        foreach (var name in selectedNames)
                        {
                            var bemat = project.BeMats.FirstOrDefault(b => b.Name == name);
                            if (bemat != null)
                            {
                                int count = bemat.Surface.SelectedGeometry.Count;
                                int resolvedCount = bemat.Surface.SelectedGeometry.Count(g => g.CurrentObjectId.HasValue && !g.CurrentObjectId.Value.IsNull);
                                debugDetails += $"- Bề mặt '{bemat.Name}': {count} lines (resolved={resolvedCount})\n";
                                totalGeometryCount += count;
                                resolvedGeometryCount += resolvedCount;
                            }
                        }
                        break;

                    case "Địa hình lò":
                        debugDetails += $"Số Địa hình lò trong project: {project.MineTopologies.Count}\n";
                        foreach (var name in selectedNames)
                        {
                            var topo = project.MineTopologies.FirstOrDefault(t => t.Name == name);
                            if (topo != null)
                            {
                                int nenCount  = topo.Nen.Count;
                                int nocCount  = topo.Noc.Count;
                                int bienCount = topo.Bien.Count;
                                int nenResolved  = topo.Nen.Count(g  => g.CurrentObjectId.HasValue && !g.CurrentObjectId.Value.IsNull);
                                int nocResolved  = topo.Noc.Count(g  => g.CurrentObjectId.HasValue && !g.CurrentObjectId.Value.IsNull);
                                int bienResolved = topo.Bien.Count(g => g.CurrentObjectId.HasValue && !g.CurrentObjectId.Value.IsNull);
                                debugDetails += $"- '{topo.Name}': Nền={nenCount}(r={nenResolved}), Nóc={nocCount}(r={nocResolved}), Biên={bienCount}(r={bienResolved})\n";
                                totalGeometryCount   += nenCount + nocCount + bienCount;
                                resolvedGeometryCount += nenResolved + nocResolved + bienResolved;
                            }
                        }
                        break;

                    case "Giới hạn":
                        debugDetails += $"Số Giới hạn trong project: {project.GioiHans.Count}\n";
                        foreach (var name in selectedNames)
                        {
                            var gh = project.GioiHans.FirstOrDefault(g => g.Name == name);
                            if (gh != null)
                            {
                                debugDetails += $"- Giới hạn '{gh.Name}': {gh.Blocks.Count} vùng\n";
                                foreach (var khoi in gh.Blocks)
                                {
                                    int vachCount = khoi.Vach.SelectedGeometry.Count;
                                    int vachResolved = khoi.Vach.SelectedGeometry.Count(g => g.CurrentObjectId.HasValue && !g.CurrentObjectId.Value.IsNull);
                                    int truCount = khoi.Tru.SelectedGeometry.Count;
                                    int truResolved = khoi.Tru.SelectedGeometry.Count(g => g.CurrentObjectId.HasValue && !g.CurrentObjectId.Value.IsNull);
                                    debugDetails += $"  - {khoi.Name}: Vách={vachCount}(r={vachResolved}), Trụ={truCount}(r={truResolved})\n";
                                    totalGeometryCount   += vachCount + truCount;
                                    resolvedGeometryCount += vachResolved + truResolved;
                                }
                            }
                        }
                        break;
                }

                string json = await GenerateCombinedJsonPayload(category, selectedNames, mapName, project);

                // null means quality checks blocked the export (errors found, markers placed)
                if (json == null) return;

                // Check if any data was actually generated
                if (json == "[]" || json == "[\r\n]" || json == "[\n]")
                {
                    string debugMsg = $"Không có dữ liệu để export!\n\n";
                    debugMsg += $"Debug Info:\n";
                    debugMsg += $"- Tổng số geometry đã chọn: {totalGeometryCount}\n";
                    debugMsg += $"- Geometry đã resolve: {resolvedGeometryCount}\n";
                    debugMsg += $"- JSON generated: {json.Length} chars\n\n";
                    debugMsg += $"Chi tiết:\n{debugDetails}\n";
                    debugMsg += $"Vui lòng kiểm tra:\n";
                    if (resolvedGeometryCount == 0 && totalGeometryCount > 0)
                    {
                        debugMsg += $"- ⚠ Các đường đã chọn KHÔNG CÓ trong file DWG hiện tại!\n";
                        debugMsg += $"- Hãy mở lại file DWG gốc hoặc chọn lại các đường\n";
                    }
                    else
                    {
                        debugMsg += $"- Đường đã chọn có trong file DWG hiện tại không?\n";
                        debugMsg += $"- Thử đóng và mở lại form, sau đó export lại\n";
                    }
                    
                    MessageBox.Show(debugMsg, "Không có dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                    DefaultExt = "json",
                    FileName = $"{mapName.Replace(" ", "_")}.json",
                    Title = "Export JSON File"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllText(saveDialog.FileName, json);
                    
                    int lineCount = CountTotalLines(category, selectedNames, project);

                    MessageBox.Show($"Export thành công!\n\nFile: {saveDialog.FileName}\n{category}: {selectedNames.Count}\nTổng số lines: {lineCount}", 
                        "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi Export: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Run data-quality checks for all selected Bề mặt items and place
        /// markers on the drawing. Returns the summary string.
        /// </summary>
        public async Task<string> RunDQChecksForBeMats(
            List<BeMatData> bemats, List<string> selectedNames, MiningProject project)
        {
            ResolveAllGeometryReferences("Bề mặt", selectedNames, project);

            var allIssues = new List<QualityIssue>();
            foreach (var name in selectedNames)
            {
                var bemat = bemats.FirstOrDefault(b => b.Name == name);
                if (bemat == null || bemat.Surface.SelectedGeometry.Count == 0) continue;

                var (_, issues) = await _geometryProcessor
                    .ProcessGeometryWithQualityChecks(bemat.Surface);
                allIssues.AddRange(issues);
            }

            if (allIssues.Count == 0)
                return "Kiểm tra xong — không có vấn đề nào.";

            return _markerService.PlaceMarkers(allIssues);
        }

        /// <summary>
        /// Remove all DQ_* quality marker layers from the active drawing.
        /// </summary>
        public string ClearQualityMarkers() => _markerService.ClearMarkers();

        /// <summary>
        /// Send JSON to server
        /// </summary>
        public async Task SendToServer(
            string category,
            List<string> selectedNames,
            string mapName,
            string serverUrl,
            MiningProject project,
            Form parentForm,
            DateTime? date = null)
        {
            string baseUrl = serverUrl.TrimEnd('/');
            string url = $"{baseUrl}/api/cad-upload";

            try
            {
                // CRITICAL: Resolve all geometry references BEFORE async processing
                ResolveAllGeometryReferences(category, selectedNames, project);

                string json = await GenerateCombinedJsonPayload(category, selectedNames, mapName, project, date);

                // null means quality checks blocked the export (errors found, markers placed)
                if (json == null) return;
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                await Task.Run(async () =>
                {
                    try
                    {
                        HttpResponseMessage response = await client.PostAsync(url, content);
                        string responseBody = await response.Content.ReadAsStringAsync();

                        if (parentForm.IsDisposed) return;
                        parentForm.Invoke((MethodInvoker)delegate
                        {
                            if (response.IsSuccessStatusCode)
                                MessageBox.Show($"Gửi {selectedNames.Count} {category} thành công!\n{responseBody}", 
                                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            else
                                MessageBox.Show($"Lỗi Server {response.StatusCode}:\n{responseBody}", 
                                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        });
                    }
                    catch (Exception ex)
                    {
                        if (parentForm.IsDisposed) return;
                        string innerMsg = ex.InnerException != null ? $"\nChi tiết: {ex.InnerException.Message}" : "";
                        parentForm.Invoke((MethodInvoker)delegate
                        {
                            MessageBox.Show($"Lỗi kết nối:\nURL: {url}\n{ex.Message}{innerMsg}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Private helper methods
        private async Task ProcessVias(List<ViaData> vias, List<string> selectedNames, string mapName, List<object> flattenedItems)
        {
            foreach (var name in selectedNames)
            {
                var via = vias.FirstOrDefault(v => v.Name == name);
                if (via != null)
                {
                    foreach (var khoi in via.Blocks)
                    {
                        // Process Vach (Roof) - only if it has geometry
                        if (khoi.Vach.SelectedGeometry.Count > 0)
                        {
                            // Geometry already resolved in ResolveAllGeometryReferences
                            var vachGeometryList = await _geometryProcessor.ProcessGeometryWithSmartZ(khoi.Vach);
                            foreach (var geo in vachGeometryList)
                            {
                                flattenedItems.Add(CreateGeometryItem(geo, mapName, via.Name, khoi.Name, "Vách"));
                            }
                        }

                        // Process Tru (Floor) - only if it has geometry
                        if (khoi.Tru.SelectedGeometry.Count > 0)
                        {
                            // Geometry already resolved in ResolveAllGeometryReferences
                            var truGeometryList = await _geometryProcessor.ProcessGeometryWithSmartZ(khoi.Tru);
                            foreach (var geo in truGeometryList)
                            {
                                flattenedItems.Add(CreateGeometryItem(geo, mapName, via.Name, khoi.Name, "Trụ"));
                            }
                        }

                        // Process DutGay (Fault within block) - only if it has geometry
                        if (khoi.DutGay != null && khoi.DutGay.SelectedGeometry.Count > 0)
                        {
                            var dutGayGeometryList = await _geometryProcessor.ProcessGeometryWithSmartZ(khoi.DutGay);
                            foreach (var geo in dutGayGeometryList)
                            {
                                // Đứt gãy IS the breakline for a Via block — it's the line that
                                // splits the block/volume into two downstream in the render app.
                                geo.IsBreakline = true;
                                flattenedItems.Add(CreateGeometryItem(geo, mapName, via.Name, khoi.Name, "Đứt gãy"));
                            }
                        }
                    }
                }
            }
        }

        private async Task ProcessFaults(List<FaultData> faults, List<string> selectedNames, string mapName, List<object> flattenedItems)
        {
            foreach (var name in selectedNames)
            {
                var fault = faults.FirstOrDefault(f => f.Name == name);
                if (fault != null && fault.Surface.SelectedGeometry.Count > 0)
                {
                    // Geometry already resolved in ResolveAllGeometryReferences
                    var geometryList = await _geometryProcessor.ProcessGeometryWithSmartZ(fault.Surface);
                    foreach (var geo in geometryList)
                    {
                        flattenedItems.Add(CreateGeometryItem(geo, mapName, fault.Name, null, "Đứt gãy"));
                    }
                }
            }
        }

        private async Task ProcessRocks(List<RockData> rocks, List<string> selectedNames, string mapName, List<object> flattenedItems)
        {
            foreach (var name in selectedNames)
            {
                var rock = rocks.FirstOrDefault(r => r.Name == name);
                if (rock != null && rock.Surface.SelectedGeometry.Count > 0)
                {
                    // Geometry already resolved in ResolveAllGeometryReferences
                    var geometryList = await _geometryProcessor.ProcessGeometryWithSmartZ(rock.Surface);
                    foreach (var geo in geometryList)
                    {
                        flattenedItems.Add(CreateGeometryItem(geo, mapName, rock.Name, null, "Nham thạch"));
                    }
                }
            }
        }

        private async Task ProcessBoreholes(List<BoreholeData> boreholes, List<string> selectedNames, string mapName, List<object> flattenedItems)
        {
            foreach (var name in selectedNames)
            {
                var borehole = boreholes.FirstOrDefault(b => b.Name == name);
                if (borehole != null)
                {
                    // Adding simple payload for Borehole
                    flattenedItems.Add(new
                    {
                        MapName = mapName,
                        Name = borehole.Name,
                        Type = "Lỗ khoan",
                        ExcelFilePath = borehole.ExcelFilePath,
                        X = borehole.X,
                        Y = borehole.Y,
                        Z = borehole.Z,
                        Intervals = borehole.Intervals,
                        Trajectory = borehole.Trajectory
                    });
                }
            }
        }

        private async Task<bool> ProcessBeMats(List<BeMatData> bemats, List<string> selectedNames, string mapName, List<object> flattenedItems)
        {
            foreach (var name in selectedNames)
            {
                var bemat = bemats.FirstOrDefault(b => b.Name == name);
                if (bemat == null || (bemat.Surface.SelectedGeometry.Count == 0 && bemat.Surface.BoundaryGeometry.Count == 0)) continue;

                var geometryList = await _geometryProcessor.ProcessGeometryWithSmartZ(bemat.Surface);
                foreach (var geo in geometryList)
                    flattenedItems.Add(new
                    {
                        MapName = mapName,
                        Handle = geo.Handle,
                        Layer = geo.Layer,
                        ColorIndex = geo.ColorIndex,
                        ColorName = geo.ColorName,
                        TrueColor = geo.TrueColor,
                        ViaName = bemat.Name,
                        Type = "Bề mặt",
                        IsClosed = geo.IsClosed,
                        IsBoundary = geo.IsBoundary,
                        IsHole = geo.IsHole,
                        IsBreakline = geo.IsBreakline,
                        VertexCount = geo.FlattenedVertices.Count,
                        FlattenedVertices = geo.FlattenedVertices.Select(pt => new double[] { pt[0], pt[1], pt[2] }).ToList()
                    });
            }
            return true;
        }

        private object CreateGeometryItem(CADObjectData geo, string mapName, string name, string blockName, string type)
        {
            var item = new
            {
                MapName = mapName,
                Handle = geo.Handle,
                Layer = geo.Layer,
                ColorIndex = geo.ColorIndex,
                ColorName = geo.ColorName,
                TrueColor = geo.TrueColor,
                Name = name,
                Type = type,
                IsClosed = geo.IsClosed,
                IsBoundary = geo.IsBoundary,
                IsHole = geo.IsHole,
                IsBreakline = geo.IsBreakline,
                VertexCount = geo.FlattenedVertices.Count,
                FlattenedVertices = geo.FlattenedVertices.Select(pt => new double[] { pt[0], pt[1], pt[2] }).ToList()
            };

            // Add BlockName if it's a Via
            if (blockName != null)
            {
                return new
                {
                    item.MapName,
                    item.Handle,
                    item.Layer,
                    item.ColorIndex,
                    item.ColorName,
                    item.TrueColor,
                    ViaName = name,
                    BlockName = blockName,
                    item.Type,
                    item.IsClosed,
                    item.IsBoundary,
                    item.IsHole,
                    item.IsBreakline,
                    item.VertexCount,
                    item.FlattenedVertices
                };
            }

            return item;
        }

        private int CountTotalLines(string category, List<string> selectedNames, MiningProject project)
        {
            int lineCount = 0;

            switch (category)
            {
                case "Vỉa":
                    foreach (var name in selectedNames)
                    {
                        var via = project.Vias.FirstOrDefault(v => v.Name == name);
                        if (via != null)
                        {
                            lineCount += via.Blocks.Sum(b =>
                                b.Vach.SelectedGeometry.Count + b.Vach.BoundaryGeometry.Count +
                                b.Tru.SelectedGeometry.Count + b.Tru.BoundaryGeometry.Count +
                                b.DutGay.SelectedGeometry.Count);
                        }
                    }
                    break;
                case "Đứt gãy":
                    foreach (var name in selectedNames)
                    {
                        var fault = project.Faults.FirstOrDefault(f => f.Name == name);
                        if (fault != null) lineCount += fault.Surface.SelectedGeometry.Count + fault.Surface.BoundaryGeometry.Count;
                    }
                    break;
                case "Nham thạch":
                    foreach (var name in selectedNames)
                    {
                        var rock = project.Rocks.FirstOrDefault(r => r.Name == name);
                        if (rock != null) lineCount += rock.Surface.SelectedGeometry.Count + rock.Surface.BoundaryGeometry.Count;
                    }
                    break;
                case "Lỗ khoan":
                    foreach (var name in selectedNames)
                    {
                        var borehole = project.Boreholes.FirstOrDefault(b => b.Name == name);
                        if (borehole != null) lineCount += borehole.Trajectory.Count; // sum the trajectory points
                    }
                    break;
                case "Bề mặt":
                    foreach (var name in selectedNames)
                    {
                        var bemat = project.BeMats.FirstOrDefault(b => b.Name == name);
                        if (bemat != null) lineCount += bemat.Surface.SelectedGeometry.Count + bemat.Surface.BoundaryGeometry.Count;
                    }
                    break;
                case "Địa hình lò":
                    foreach (var name in selectedNames)
                    {
                        var topo = project.MineTopologies.FirstOrDefault(t => t.Name == name);
                        if (topo != null) lineCount += topo.Nen.Count + topo.Noc.Count + topo.Bien.Count;
                    }
                    break;
                case "Giới hạn":
                    foreach (var name in selectedNames)
                    {
                        var gh = project.GioiHans.FirstOrDefault(g => g.Name == name);
                        if (gh != null)
                            lineCount += gh.Blocks.Sum(b => b.Vach.SelectedGeometry.Count + b.Tru.SelectedGeometry.Count);
                    }
                    break;
            }

            return lineCount;
        }

        /// <summary>
        /// Resolve all geometry references for selected items BEFORE async processing
        /// This ensures ObjectIds are resolved in the correct AutoCAD context
        /// </summary>
        private void ResolveAllGeometryReferences(string category, List<string> selectedNames, MiningProject project)
        {
            switch (category)
            {
                case "Vỉa":
                    foreach (var name in selectedNames)
                    {
                        var via = project.Vias.FirstOrDefault(v => v.Name == name);
                        if (via != null)
                        {
                            foreach (var khoi in via.Blocks)
                            {
                                _geometryProcessor._selectionService.ResolveCurrentDrawingReferences(khoi.Vach);
                                _geometryProcessor._selectionService.ResolveCurrentDrawingReferences(khoi.Tru);
                                if (khoi.DutGay != null)
                                    _geometryProcessor._selectionService.ResolveCurrentDrawingReferences(khoi.DutGay);
                            }
                        }
                    }
                    break;
                case "Đứt gãy":
                    foreach (var name in selectedNames)
                    {
                        var fault = project.Faults.FirstOrDefault(f => f.Name == name);
                        if (fault != null)
                        {
                            _geometryProcessor._selectionService.ResolveCurrentDrawingReferences(fault.Surface);
                        }
                    }
                    break;
                case "Nham thạch":
                    foreach (var name in selectedNames)
                    {
                        var rock = project.Rocks.FirstOrDefault(r => r.Name == name);
                        if (rock != null)
                        {
                            _geometryProcessor._selectionService.ResolveCurrentDrawingReferences(rock.Surface);
                        }
                    }
                    break;
                case "Lỗ khoan":
                    foreach (var name in selectedNames)
                    {
                        var borehole = project.Boreholes.FirstOrDefault(b => b.Name == name);
                        if (borehole != null)
                        {
                            // Excel-based boreholes do not need to resolve AutoCAD geometry
                        }
                    }
                    break;
                case "Bề mặt":
                    foreach (var name in selectedNames)
                    {
                        var bemat = project.BeMats.FirstOrDefault(b => b.Name == name);
                        if (bemat != null)
                        {
                            _geometryProcessor._selectionService.ResolveCurrentDrawingReferences(bemat.Surface);
                        }
                    }
                    break;
                case "Địa hình lò":
                    foreach (var name in selectedNames)
                    {
                        var topo = project.MineTopologies.FirstOrDefault(t => t.Name == name);
                        if (topo != null)
                        {
                            _geometryProcessor._selectionService.ResolveCurrentDrawingReferences(topo);
                        }
                    }
                    break;
                case "Địa hình lò Loại 2":
                    foreach (var name in selectedNames)
                    {
                        var t2 = project.MineTopologies2.FirstOrDefault(t => t.Name == name);
                        if (t2 != null)
                        {
                            foreach (var td in t2.TietDiens)
                                _geometryProcessor._selectionService.ResolveReference(td.Polyline);
                            foreach (var d in t2.DoanDuongLos)
                                _geometryProcessor._selectionService.ResolveReference(d.Polyline);
                        }
                    }
                    break;
                case "Giới hạn":
                    foreach (var name in selectedNames)
                    {
                        var gh = project.GioiHans.FirstOrDefault(g => g.Name == name);
                        if (gh != null)
                        {
                            foreach (var khoi in gh.Blocks)
                            {
                                _geometryProcessor._selectionService.ResolveCurrentDrawingReferences(khoi.Vach);
                                _geometryProcessor._selectionService.ResolveCurrentDrawingReferences(khoi.Tru);
                            }
                        }
                    }
                    break;
            }
        }

        private async Task ProcessGioiHans(
            List<GioiHanData> gioiHans, List<string> selectedNames,
            string mapName, List<object> flattenedItems)
        {
            foreach (var name in selectedNames)
            {
                var gh = gioiHans.FirstOrDefault(g => g.Name == name);
                if (gh == null) continue;

                foreach (var khoi in gh.Blocks)
                {
                    if (khoi.Vach.SelectedGeometry.Count > 0)
                    {
                        var geoList = await _geometryProcessor.ProcessGeometryWithSmartZ(khoi.Vach);
                        foreach (var geo in geoList)
                            flattenedItems.Add(CreateGioiHanItem(geo, mapName, gh.Name, khoi.Name, "Vách"));
                    }
                    if (khoi.Tru.SelectedGeometry.Count > 0)
                    {
                        var geoList = await _geometryProcessor.ProcessGeometryWithSmartZ(khoi.Tru);
                        foreach (var geo in geoList)
                            flattenedItems.Add(CreateGioiHanItem(geo, mapName, gh.Name, khoi.Name, "Trụ"));
                    }
                }
            }
        }

        private object CreateGioiHanItem(
            CADObjectData geo, string mapName,
            string gioiHanName, string vungName, string surfaceType)
        {
            return new
            {
                MapName          = mapName,
                Handle           = geo.Handle,
                Layer            = geo.Layer,
                ColorIndex       = geo.ColorIndex,
                ColorName        = geo.ColorName,
                TrueColor        = geo.TrueColor,
                Name             = gioiHanName,
                VungName         = vungName,
                Type             = surfaceType,
                IsVungGioiHan    = true,
                IsClosed         = geo.IsClosed,
                IsBoundary       = geo.IsBoundary,
                IsBreakline      = geo.IsBreakline,
                VertexCount      = geo.FlattenedVertices.Count,
                FlattenedVertices = geo.FlattenedVertices.Select(pt => new double[] { pt[0], pt[1], pt[2] }).ToList()
            };
        }

        private async Task ProcessMineTopologies(
            List<MineTopologyData> topologies, List<string> selectedNames,
            string mapName, List<object> flattenedItems, DateTime? date = null)
        {
            foreach (var name in selectedNames)
            {
                var topo = topologies.FirstOrDefault(t => t.Name == name);
                if (topo == null) continue;

                List<CADObjectData> nenGeoList = new List<CADObjectData>();

                // Nền
                if (topo.Nen.Count > 0)
                {
                    nenGeoList = await _geometryProcessor.ProcessGeometryList(topo.Nen, topo.Name, "Nền");
                    foreach (var geo in nenGeoList)
                        flattenedItems.Add(CreateMineTopologyItem(geo, mapName, topo.Name, "Nền", date));
                }
                // Nóc
                if (topo.Noc.Count > 0)
                {
                    var geoList = await _geometryProcessor.ProcessGeometryList(topo.Noc, topo.Name, "Nóc");
                    foreach (var geo in geoList)
                        flattenedItems.Add(CreateMineTopologyItem(geo, mapName, topo.Name, "Nóc", date));
                }
                // Biên — Z values are assigned from the nearest Nền vertex in XY space
                if (topo.Bien.Count > 0)
                {
                    var bienGeoList = await _geometryProcessor.ProcessGeometryList(topo.Bien, topo.Name, "Biên");
                    _geometryProcessor.AssignBienZFromNen(bienGeoList, nenGeoList);
                    foreach (var geo in bienGeoList)
                        flattenedItems.Add(CreateMineTopologyItem(geo, mapName, topo.Name, "Biên", date));
                }
            }
        }

        private object CreateMineTopologyItem(CADObjectData geo, string mapName, string topoName, string layerType, DateTime? date = null)
        {
            return new
            {
                MapName      = mapName,
                Handle       = geo.Handle,
                Layer        = geo.Layer,
                ColorIndex   = geo.ColorIndex,
                ColorName    = geo.ColorName,
                TrueColor    = geo.TrueColor,
                Name         = topoName,
                Type         = "Địa hình lò",
                LayerType    = layerType,
                Date         = date.HasValue ? date.Value.ToString("yyyy-MM-dd") : null,
                IsClosed     = geo.IsClosed,
                VertexCount  = geo.FlattenedVertices.Count,
                FlattenedVertices = geo.FlattenedVertices.Select(pt => new double[] { pt[0], pt[1], pt[2] }).ToList()
            };
        }

        private async Task ProcessMineTopologies2(
            List<MineTopologyLoai2Data> topologies2, List<string> selectedNames,
            string mapName, List<object> flattenedItems)
        {
            foreach (var name in selectedNames)
            {
                var t2 = topologies2.FirstOrDefault(t => t.Name == name);
                if (t2 == null) continue;

                // Export each TietDien polyline
                foreach (var td in t2.TietDiens)
                {
                    if (td.Polyline == null) continue;
                    var surface = new MyMiningPlugin.Models.SurfaceData
                    {
                        Type       = "Tiết diện",
                        ParentName = t2.Name
                    };
                    surface.SelectedGeometry.Add(td.Polyline);
                    var geoList = await _geometryProcessor.ProcessGeometryWithSmartZ(surface);
                    foreach (var geo in geoList)
                        flattenedItems.Add(new
                        {
                            MapName      = mapName,
                            Handle       = geo.Handle,
                            Layer        = geo.Layer,
                            ColorIndex   = geo.ColorIndex,
                            ColorName    = geo.ColorName,
                            TrueColor    = geo.TrueColor,
                            Name         = t2.Name,
                            Type         = "Địa hình lò Loại 2",
                            LayerType    = "TietDien",
                            Date         = (string)null,
                            IsClosed     = geo.IsClosed,
                            VertexCount  = geo.FlattenedVertices.Count,
                            FlattenedVertices = geo.FlattenedVertices.Select(pt => new double[] { pt[0], pt[1], pt[2] }).ToList(),
                            // Type 2 additions
                            DuongLoName  = t2.Name,
                            TietDienName = td.Name
                        });
                }

                // Export each Đoạn đường lò polyline
                foreach (var doan in t2.DoanDuongLos)
                {
                    if (doan.Polyline == null) continue;
                    var surface = new MyMiningPlugin.Models.SurfaceData
                    {
                        Type       = "Đoạn đường lò",
                        ParentName = t2.Name
                    };
                    surface.SelectedGeometry.Add(doan.Polyline);
                    var geoList = await _geometryProcessor.ProcessGeometryWithSmartZ(surface);
                    foreach (var geo in geoList)
                        flattenedItems.Add(new
                        {
                            MapName      = mapName,
                            Handle       = geo.Handle,
                            Layer        = geo.Layer,
                            ColorIndex   = geo.ColorIndex,
                            ColorName    = geo.ColorName,
                            TrueColor    = geo.TrueColor,
                            Name         = t2.Name,
                            Type         = "Địa hình lò Loại 2",
                            LayerType    = "DoanDuongLo",
                            Date         = (string)null,
                            IsClosed     = geo.IsClosed,
                            VertexCount  = geo.FlattenedVertices.Count,
                            FlattenedVertices = geo.FlattenedVertices.Select(pt => new double[] { pt[0], pt[1], pt[2] }).ToList(),
                            // Type 2 additions
                            DuongLoName  = t2.Name,
                            TietDienName = doan.TietDienName,
                            DoanName     = doan.Name
                        });
                }
            }
        }
    }
}
