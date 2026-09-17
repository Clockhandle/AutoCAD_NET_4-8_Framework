using MyMiningPlugin.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MyMiningPlugin.Services
{
    /// <summary>
    /// One named project save file, as listed in the "Lưu trữ dữ liệu" (save manager) node —
    /// the RPG-style save-slot list.
    /// </summary>
    public class ProjectSaveSlot
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public DateTime SavedAt { get; set; }
    }

    /// <summary>
    /// Handles project persistence (Save/Load to JSON)
    /// </summary>
    public class PersistenceService
    {
        // ---------------------------------------------------------------
        // Save slots — every "Lưu dự án mới" creates one named *.t3d file here
        // instead of the single always-overwritten file this used to be. LastSave.txt
        // remembers which slot to silently reopen the next time the plugin starts.
        // Content is still plain JSON — .t3d is just the file's public extension so it
        // reads as a project save rather than a generic data file.
        // ---------------------------------------------------------------

        private const string SaveExtension = ".t3d";

        private string GetLegacyProjectDataPath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string projectFolder = Path.Combine(appData, "MyMiningPlugin");

            if (!Directory.Exists(projectFolder))
                Directory.CreateDirectory(projectFolder);

            return Path.Combine(projectFolder, "MiningProject.json");
        }

        public string GetSavesFolder()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appData, "MyMiningPlugin", "Saves");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            return folder;
        }

        private string GetLastSavePointerPath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, "MyMiningPlugin", "LastSave.txt");
        }

        /// <summary>Path of the save slot last written or loaded, or null if there isn't one yet.</summary>
        public string GetLastSavePath()
        {
            try
            {
                string pointerPath = GetLastSavePointerPath();
                if (!File.Exists(pointerPath)) return null;
                string path = File.ReadAllText(pointerPath).Trim();
                return string.IsNullOrEmpty(path) ? null : path;
            }
            catch
            {
                return null;
            }
        }

        private void SetLastSavePath(string path)
        {
            try { File.WriteAllText(GetLastSavePointerPath(), path); }
            catch { /* best effort — worst case, next launch just won't auto-reopen it */ }
        }

        /// <summary>
        /// One-time migration for anyone upgrading from the old single always-overwritten
        /// save file: if that legacy file exists and no save slot has been created yet,
        /// copy (never move — never destroy the original) it into the Saves folder as the
        /// first slot, so existing work shows up in the new save list instead of vanishing.
        /// </summary>
        public void MigrateLegacyProjectIfNeeded()
        {
            try
            {
                if (GetLastSavePath() != null) return; // already on the new system

                string legacyPath = GetLegacyProjectDataPath();
                if (!File.Exists(legacyPath)) return;

                string savesFolder = GetSavesFolder();
                if (GetSaveFiles(savesFolder).Any()) return;

                string destPath = Path.Combine(savesFolder, "Dữ liệu trước đó" + SaveExtension);
                if (!File.Exists(destPath))
                    File.Copy(legacyPath, destPath, overwrite: false);
                SetLastSavePath(destPath);
            }
            catch { /* best effort */ }
        }

        public static string SanitizeSaveName(string name)
        {
            var invalid = Path.GetInvalidFileNameChars();
            string cleaned = new string((name ?? "").Where(c => !invalid.Contains(c)).ToArray()).Trim();
            return string.IsNullOrEmpty(cleaned) ? "Dự án" : cleaned;
        }

        // *.json is still scanned so save slots created before the switch to .t3d
        // keep showing up in the list instead of silently vanishing.
        private static IEnumerable<string> GetSaveFiles(string savesFolder)
        {
            return Directory.GetFiles(savesFolder, "*" + SaveExtension)
                .Concat(Directory.GetFiles(savesFolder, "*.json"));
        }

        /// <summary>Lists every save slot in the Saves folder, most recently saved first.</summary>
        public List<ProjectSaveSlot> ListSaves()
        {
            var result = new List<ProjectSaveSlot>();
            foreach (var file in GetSaveFiles(GetSavesFolder()))
            {
                result.Add(new ProjectSaveSlot
                {
                    Name = Path.GetFileNameWithoutExtension(file),
                    FilePath = file,
                    SavedAt = File.GetLastWriteTime(file)
                });
            }
            return result.OrderByDescending(s => s.SavedAt).ToList();
        }

        /// <summary>Writes a brand-new save slot named <paramref name="saveName"/> and returns its path.</summary>
        public string SaveProjectAs(MiningProject project, string saveName)
        {
            string path = Path.Combine(GetSavesFolder(), SanitizeSaveName(saveName) + SaveExtension);
            File.WriteAllText(path, BuildProjectJson(project));
            SetLastSavePath(path);
            return path;
        }

        /// <summary>Re-writes an existing save slot in place — an RPG-style "overwrite save".</summary>
        public void OverwriteSave(MiningProject project, string filePath)
        {
            File.WriteAllText(filePath, BuildProjectJson(project));
            SetLastSavePath(filePath);
        }

        public void DeleteSave(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        /// <summary>Loads a project from an explicit save-slot path (or any project JSON file the user browsed to).</summary>
        public MiningProject LoadProjectFromFile(string filePath, bool silent = false)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    if (!silent)
                        MessageBox.Show("Không tìm thấy file lưu này.", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return null;
                }

                var project = ParseProjectJson(File.ReadAllText(filePath));
                SetLastSavePath(filePath);

                if (!silent)
                    MessageBox.Show($"Đã tải dự án từ:\n{filePath}", "Tải thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                return project;
            }
            catch (Exception ex)
            {
                if (!silent)
                    MessageBox.Show($"Lỗi khi tải: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private string BuildProjectJson(MiningProject project)
        {
            var projectData = new
            {
                SavedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Vias = project.Vias.Select(v => new
                {
                    v.Name,
                    v.IsDutGay,
                    Blocks = v.Blocks.Select(b => new
                        {
                            b.Name,
                            Vach = SerializeSurfaceData(b.Vach),
                            Tru = SerializeSurfaceData(b.Tru),
                            DutGay = b.DutGay != null ? SerializeSurfaceData(b.DutGay) : null
                        }).ToList()
                }).ToList(),
                Faults = project.Faults.Select(f => new
                {
                    f.Name,
                    Surface = SerializeSurfaceData(f.Surface)
                }).ToList(),
                Rocks = project.Rocks.Select(r => new
                {
                    r.Name,
                    Surface = SerializeSurfaceData(r.Surface)
                }).ToList(),
                Boreholes = project.Boreholes.Select(b => new
                {
                    b.Name,
                    b.ExcelFilePath,
                    b.X,
                    b.Y,
                    b.Z,
                    b.Intervals,
                    b.Trajectory,
                    ImportedBoreholes = b.ImportedBoreholes.Select(ib => new
                    {
                        ib.Name,
                        ib.ExcelFilePath,
                        ib.X,
                        ib.Y,
                        ib.Z,
                        ib.Intervals,
                        ib.Trajectory
                    }).ToList()
                }).ToList(),
                BeMats = project.BeMats.Select(b => new
                {
                    b.Name,
                    Surface = SerializeSurfaceData(b.Surface)
                }).ToList(),
                MineTopologies2 = project.MineTopologies2.Select(t2 => new
                {
                    t2.Name,
                    TietDiens = t2.TietDiens.Select(td => SerializeGeoRef(td.Polyline, td.Name)).ToList(),
                    DoanDuongLos = t2.DoanDuongLos.Select(d => new
                    {
                        d.Name,
                        Polyline = d.Polyline != null ? SerializeGeoRef(d.Polyline) : null,
                        d.TietDienName,
                        MinedDate = d.MinedDate.HasValue ? d.MinedDate.Value.ToString("yyyy-MM-dd") : null
                    }).ToList()
                }).ToList()
            };

            return JsonConvert.SerializeObject(projectData, Formatting.Indented);
        }

        private MiningProject ParseProjectJson(string json)
        {
            dynamic projectData = JsonConvert.DeserializeObject<dynamic>(json);

            MiningProject project = new MiningProject();

            // Reconstruct Vias
            foreach (var viaData in projectData.Vias)
            {
                ViaData via = new ViaData
                {
                    Name = viaData.Name.ToString(),
                    IsDutGay = viaData.IsDutGay != null && (bool)viaData.IsDutGay
                };

                foreach (var blockData in viaData.Blocks)
                {
                    KhoiData khoi = new KhoiData
                    {
                        Name = blockData.Name.ToString(),
                        Vach = DeserializeSurfaceData(blockData.Vach),
                        Tru = DeserializeSurfaceData(blockData.Tru),
                        DutGay = blockData.DutGay != null
                            ? DeserializeSurfaceData(blockData.DutGay)
                            : new SurfaceData { Type = "Đứt gãy", ParentName = blockData.Name.ToString() }
                    };
                    via.Blocks.Add(khoi);
                }
                project.Vias.Add(via);
            }

            // Reconstruct Faults
            foreach (var faultData in projectData.Faults)
            {
                FaultData fault = new FaultData
                {
                    Name = faultData.Name.ToString(),
                    Surface = DeserializeSurfaceData(faultData.Surface)
                };
                project.Faults.Add(fault);
            }

            // Reconstruct Rocks
            foreach (var rockData in projectData.Rocks)
            {
                RockData rock = new RockData
                {
                    Name = rockData.Name.ToString(),
                    Surface = DeserializeSurfaceData(rockData.Surface)
                };
                project.Rocks.Add(rock);
            }

            // Reconstruct Boreholes
            if (projectData.Boreholes != null)
            {
                foreach (var boreholeData in projectData.Boreholes)
                {
                    BoreholeData borehole = DeserializeBoreholeLeaf(boreholeData);

                    if (boreholeData.ImportedBoreholes != null)
                    {
                        foreach (var importedData in boreholeData.ImportedBoreholes)
                            borehole.ImportedBoreholes.Add(DeserializeBoreholeLeaf(importedData));
                    }

                    project.Boreholes.Add(borehole);
                }
            }

            // Reconstruct BeMats
            if (projectData.BeMats != null)
            {
                foreach (var beMatData in projectData.BeMats)
                {
                    BeMatData beMat = new BeMatData
                    {
                        Name = beMatData.Name.ToString(),
                        Surface = DeserializeSurfaceData(beMatData.Surface)
                    };
                    project.BeMats.Add(beMat);
                }
            }

            // Reconstruct MineTopologies2
            if (projectData.MineTopologies2 != null)
            {
                foreach (var t2Data in projectData.MineTopologies2)
                {
                    var t2 = new MineTopologyLoai2Data { Name = t2Data.Name.ToString() };

                    if (t2Data.TietDiens != null)
                    {
                        foreach (var tdItem in t2Data.TietDiens)
                        {
                            t2.TietDiens.Add(new TietDienData
                            {
                                Name     = tdItem.TietDienName != null ? tdItem.TietDienName.ToString() : "",
                                Polyline = DeserializeGeoRef(tdItem)
                            });
                        }
                    }

                    if (t2Data.DoanDuongLos != null)
                    {
                        foreach (var dData in t2Data.DoanDuongLos)
                        {
                            DateTime? minedDate = null;
                            string minedDateStr = dData.MinedDate != null ? dData.MinedDate.ToString() : null;
                            if (!string.IsNullOrEmpty(minedDateStr) && DateTime.TryParse(minedDateStr, out DateTime parsedDate))
                                minedDate = parsedDate;

                            t2.DoanDuongLos.Add(new DoanDuongLoData
                            {
                                Name         = dData.Name.ToString(),
                                Polyline     = dData.Polyline != null ? DeserializeGeoRef(dData.Polyline) : null,
                                TietDienName = dData.TietDienName != null ? dData.TietDienName.ToString() : null,
                                MinedDate    = minedDate
                            });
                        }
                    }

                    project.MineTopologies2.Add(t2);
                }
            }

            return project;
        }

        /// <summary>
        /// Reconstructs one BoreholeData "leaf" (Name/ExcelFilePath/X/Y/Z/Intervals/
        /// Trajectory only — no ImportedBoreholes) from saved JSON. Shared by the
        /// top-level Boreholes loop and each entry of a container's ImportedBoreholes,
        /// which are saved in the same shape.
        /// </summary>
        private BoreholeData DeserializeBoreholeLeaf(dynamic boreholeData)
        {
            BoreholeData borehole = new BoreholeData
            {
                Name = boreholeData.Name != null ? boreholeData.Name.ToString() : "",
                ExcelFilePath = boreholeData.ExcelFilePath != null ? boreholeData.ExcelFilePath.ToString() : "",
                X = boreholeData.X != null ? (double)boreholeData.X : 0,
                Y = boreholeData.Y != null ? (double)boreholeData.Y : 0,
                Z = boreholeData.Z != null ? (double)boreholeData.Z : 0,
                Intervals = new List<DepthInterval>(),
                Trajectory = new List<SurveyReading>()
            };

            if (boreholeData.Intervals != null)
            {
                foreach (var interval in boreholeData.Intervals)
                {
                    borehole.Intervals.Add(new DepthInterval
                    {
                        From = (double)interval.From,
                        To = (double)interval.To,
                        SeamName = interval.SeamName != null ? interval.SeamName.ToString() : null
                    });
                }
            }

            if (boreholeData.Trajectory != null)
            {
                foreach (var traj in boreholeData.Trajectory)
                {
                    borehole.Trajectory.Add(new SurveyReading
                    {
                        DepthRange = (double)traj.DepthRange,
                        DO = (double)traj.DO,
                        PVI = (double)traj.PVI
                    });
                }
            }

            return borehole;
        }

        private object SerializeSurfaceData(SurfaceData surface)
        {
            return new
            {
                surface.Type,
                surface.ParentName,
                SelectedGeometry = surface.SelectedGeometry.Select(g => new
                {
                    g.Handle,
                    g.SourceDwgPath,
                    g.SourceDwgName,
                    g.Layer,
                    g.EntityType,
                    g.VertexCount
                }).ToList(),
                BoundaryGeometry = surface.BoundaryGeometry.Select(g => new
                {
                    g.Handle,
                    g.SourceDwgPath,
                    g.SourceDwgName,
                    g.Layer,
                    g.EntityType,
                    g.VertexCount
                }).ToList(),
                HoleGeometry = surface.HoleGeometry.Select(g => new
                {
                    g.Handle,
                    g.SourceDwgPath,
                    g.SourceDwgName,
                    g.Layer,
                    g.EntityType,
                    g.VertexCount
                }).ToList(),
                BreaklineGeometry = surface.BreaklineGeometry.Select(g => new
                {
                    g.Handle,
                    g.SourceDwgPath,
                    g.SourceDwgName,
                    g.Layer,
                    g.EntityType,
                    g.VertexCount
                }).ToList()
            };
        }

        private SurfaceData DeserializeSurfaceData(dynamic surfaceData)
        {
            var surface = new SurfaceData
            {
                Type = surfaceData.Type.ToString(),
                ParentName = surfaceData.ParentName.ToString(),
                SelectedGeometry = DeserializeGeometryReferences(surfaceData.SelectedGeometry)
            };

            if (surfaceData.BoundaryGeometry != null)
            {
                surface.BoundaryGeometry = DeserializeGeometryReferences(surfaceData.BoundaryGeometry);
            }

            if (surfaceData.HoleGeometry != null)
            {
                surface.HoleGeometry = DeserializeGeometryReferences(surfaceData.HoleGeometry);
            }

            if (surfaceData.BreaklineGeometry != null)
            {
                surface.BreaklineGeometry = DeserializeGeometryReferences(surfaceData.BreaklineGeometry);
            }

            return surface;
        }

        private List<GeometryReference> DeserializeGeometryReferences(dynamic geoList)
        {
            List<GeometryReference> result = new List<GeometryReference>();

            foreach (var geoData in geoList)
            {
                result.Add(new GeometryReference
                {
                    Handle = geoData.Handle.ToString(),
                    SourceDwgPath = geoData.SourceDwgPath.ToString(),
                    SourceDwgName = geoData.SourceDwgName.ToString(),
                    Layer = geoData.Layer.ToString(),
                    EntityType = geoData.EntityType.ToString(),
                    VertexCount = (int)geoData.VertexCount,
                    CurrentObjectId = null // Will be resolved when needed
                });
            }

            return result;
        }

        // ---------------------------------------------------------------
        // Single GeometryReference helpers (for TietDien / DoanDuongLo)
        // ---------------------------------------------------------------

        private object SerializeGeoRef(GeometryReference g, string tietDienName = null)
        {
            if (g == null) return null;
            return new
            {
                g.Handle,
                g.SourceDwgPath,
                g.SourceDwgName,
                g.Layer,
                g.EntityType,
                g.VertexCount,
                TietDienName = tietDienName   // only populated for TietDien entries
            };
        }

        private GeometryReference DeserializeGeoRef(dynamic d)
        {
            if (d == null) return null;
            return new GeometryReference
            {
                Handle        = d.Handle        != null ? d.Handle.ToString()        : "",
                SourceDwgPath = d.SourceDwgPath != null ? d.SourceDwgPath.ToString() : "",
                SourceDwgName = d.SourceDwgName != null ? d.SourceDwgName.ToString() : "",
                Layer         = d.Layer         != null ? d.Layer.ToString()         : "",
                EntityType    = d.EntityType    != null ? d.EntityType.ToString()    : "",
                VertexCount   = d.VertexCount   != null ? (int)d.VertexCount         : 0,
                CurrentObjectId = null
            };
        }

        // ---------------------------------------------------------------
        // Tiết diện Library  (global, cross-project)
        // ---------------------------------------------------------------

        private string GetTietDienLibraryPath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, "MyMiningPlugin", "TietDienLibrary.json");
        }

        public void SaveTietDienLibrary(List<TietDienData> library)
        {
            try
            {
                var data = library.Select(td => SerializeGeoRef(td.Polyline, td.Name)).ToList();
                File.WriteAllText(GetTietDienLibraryPath(),
                    JsonConvert.SerializeObject(data, Formatting.Indented));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lưu thư viện Tiết diện: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public List<TietDienData> LoadTietDienLibrary()
        {
            var result = new List<TietDienData>();
            try
            {
                string path = GetTietDienLibraryPath();
                if (!File.Exists(path)) return result;

                dynamic list = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(path));
                foreach (var item in list)
                {
                    result.Add(new TietDienData
                    {
                        Name     = item.TietDienName != null ? item.TietDienName.ToString() : "",
                        Polyline = DeserializeGeoRef(item)
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadTietDienLibrary error: {ex.Message}");
            }
            return result;
        }

        /// <summary>
        /// Reads a Tiết diện JSON previously written by the library's "Xuất JSON" button
        /// (a flattened-geometry export, not the roaming TietDienLibrary.json format) and
        /// reconstructs it as reusable TietDienData entries. Each entry's shape is stored as
        /// CachedVertices with no live drawing reference, so Đoạn đường lò can assign it even
        /// in a project where the original polyline was never selected — see the
        /// CachedVertices fallback in GeometryProcessingService.ProcessGeometryWithSmartZ.
        /// </summary>
        public List<TietDienData> LoadTietDienLibraryFromExportedJson(string filePath)
        {
            var result = new List<TietDienData>();
            dynamic items = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(filePath));

            foreach (var item in items)
            {
                string name = item.TietDienName != null ? item.TietDienName.ToString() : null;
                if (string.IsNullOrEmpty(name)) continue;

                var cachedVertices = new List<double[]>();
                if (item.FlattenedVertices != null)
                {
                    foreach (var v in item.FlattenedVertices)
                        cachedVertices.Add(new double[] { (double)v[0], (double)v[1], (double)v[2] });
                }

                result.Add(new TietDienData
                {
                    Name = name,
                    Polyline = new GeometryReference
                    {
                        Handle        = item.Handle != null ? item.Handle.ToString() : "",
                        Layer         = item.Layer != null ? item.Layer.ToString() : "",
                        SourceDwgPath = "",
                        SourceDwgName = "(từ JSON)",
                        EntityType    = "LWPOLYLINE",
                        VertexCount   = item.VertexCount != null ? (int)item.VertexCount : cachedVertices.Count,
                        IsClosed      = item.IsClosed != null && (bool)item.IsClosed,
                        CachedVertices = cachedVertices,
                        CurrentObjectId = null
                    }
                });
            }

            return result;
        }
    }
}
