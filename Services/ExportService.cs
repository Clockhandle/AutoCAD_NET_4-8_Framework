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
            MiningProject project)
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
                }

                string json = await GenerateCombinedJsonPayload(category, selectedNames, mapName, project);

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
        /// Send JSON to server
        /// </summary>
        public async Task SendToServer(
            string category, 
            List<string> selectedNames, 
            string mapName, 
            string ip, 
            string port,
            MiningProject project,
            Form parentForm)
        {
            string url = $"http://{ip}:{port}/api/cad-data";

            try
            {
                // CRITICAL: Resolve all geometry references BEFORE async processing
                ResolveAllGeometryReferences(category, selectedNames, project);
                
                string json = await GenerateCombinedJsonPayload(category, selectedNames, mapName, project);
                
                // Check if any data was actually generated
                if (json == "[]")
                {
                    MessageBox.Show($"Không có dữ liệu để gửi!\n\nVui lòng kiểm tra:\n- {category} đã có Khối (Block) chưa?\n- Các Khối đã chọn đường (lines) chưa?\n- Đường đã chọn có trong file DWG hiện tại không?", 
                        "Không có dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                await Task.Run(async () =>
                {
                    try
                    {
                        HttpResponseMessage response = await client.PostAsync(url, content);
                        string responseBody = await response.Content.ReadAsStringAsync();

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
                        parentForm.Invoke((MethodInvoker)delegate
                        {
                            MessageBox.Show($"Lỗi kết nối: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                            lineCount += via.Blocks.Sum(b => b.Vach.SelectedGeometry.Count + b.Tru.SelectedGeometry.Count);
                        }
                    }
                    break;
                case "Đứt gãy":
                    foreach (var name in selectedNames)
                    {
                        var fault = project.Faults.FirstOrDefault(f => f.Name == name);
                        if (fault != null) lineCount += fault.Surface.SelectedGeometry.Count;
                    }
                    break;
                case "Nham thạch":
                    foreach (var name in selectedNames)
                    {
                        var rock = project.Rocks.FirstOrDefault(r => r.Name == name);
                        if (rock != null) lineCount += rock.Surface.SelectedGeometry.Count;
                    }
                    break;
                case "Lỗ khoan":
                    foreach (var name in selectedNames)
                    {
                        var borehole = project.Boreholes.FirstOrDefault(b => b.Name == name);
                        if (borehole != null) lineCount += borehole.Trajectory.Count; // sum the trajectory points
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
            }
        }
    }
}
