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
    /// One named project save file, as listed in the "Quản lý dữ liệu" (save manager) node —
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
        // Save slots — every "Lưu dự án mới" creates one named *.t3d file here
        // instead of the single always-overwritten file this used to be. LastSave.txt
        // remembers which slot to silently reopen the next time the plugin starts.
        // Content is still plain JSON — .t3d is just the file's public extension so it
        // reads as a project save rather than a generic data file.
        private const string SaveExtension = ".t3d";

        private string GetLegacyProjectDataPath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string projectFolder = Path.Combine(appData, "MyMiningPlugin");

            if (!Directory.Exists(projectFolder))
                Directory.CreateDirectory(projectFolder);

            return Path.Combine(projectFolder, "MiningProject.json");
        }

        private string GetSavesFolderPointerPath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string pluginFolder = Path.Combine(appData, "MyMiningPlugin");

            if (!Directory.Exists(pluginFolder))
                Directory.CreateDirectory(pluginFolder);

            return Path.Combine(pluginFolder, "SavesFolderPath.txt");
        }

        /// <summary>
        /// Where "+ Lưu dự án mới" writes new save slots — the user-chosen folder from
        /// "Đổi thư mục lưu..." if one was set (see SetSavesFolder), otherwise the default
        /// AppData\MyMiningPlugin\Saves. Created if it doesn't exist yet.
        /// </summary>
        public string GetSavesFolder()
        {
            string folder = null;
            try
            {
                string pointerPath = GetSavesFolderPointerPath();
                if (File.Exists(pointerPath))
                {
                    string chosen = File.ReadAllText(pointerPath).Trim();
                    if (!string.IsNullOrEmpty(chosen)) folder = chosen;
                }
            }
            catch { /* fall through to the default folder below */ }

            if (string.IsNullOrEmpty(folder))
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                folder = Path.Combine(appData, "MyMiningPlugin", "Saves");
            }

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            return folder;
        }

        /// <summary>
        /// Points future saves at <paramref name="folder"/> instead of the default Saves
        /// folder. Existing save files are left exactly where they are — the caller
        /// (MiningManagerDForm.ChangeSavesFolder) is responsible for offering to copy them
        /// over first, the same "copy, never move" safety MigrateLegacyProjectIfNeeded uses.
        /// </summary>
        public void SetSavesFolder(string folder)
        {
            if (string.IsNullOrEmpty(folder)) return;

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            File.WriteAllText(GetSavesFolderPointerPath(), folder);
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

        // Entity-name diffing against a save slot — lets the mass-send dialog tell
        // which of the CURRENT project's entities aren't in a given save file yet,
        // so it can auto-check only those instead of everything. Read straight off
        // the raw JSON (just names, no geometry) rather than through ParseProjectJson,
        // since this doesn't need — and shouldn't require — an AutoCAD context.
        private static readonly string[] CategoriesInSaveFile =
        {
            "Vỉa", "Đứt gãy", "Nham thạch", "Lỗ khoan", "Bề mặt",
            "Địa hình lò", "Địa hình lò Loại 2", "Giới hạn"
        };

        /// <summary>
        /// Names present in <paramref name="filePath"/>'s save, grouped by the same
        /// category strings used elsewhere (GetItemsByCategory / ExportService). Missing
        /// or unreadable file → every category comes back empty (nothing saved yet, so
        /// everything currently in the project counts as new).
        /// </summary>
        public Dictionary<string, HashSet<string>> GetSavedEntityNames(string filePath)
        {
            var result = new Dictionary<string, HashSet<string>>();
            foreach (var category in CategoriesInSaveFile)
                result[category] = new HashSet<string>();

            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return result;

                dynamic projectData = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(filePath));

                CollectSavedNames(projectData?.Vias, result["Vỉa"]);
                CollectSavedNames(projectData?.Faults, result["Đứt gãy"]);
                CollectSavedNames(projectData?.Rocks, result["Nham thạch"]);
                CollectSavedNames(projectData?.Boreholes, result["Lỗ khoan"]);
                CollectSavedNames(projectData?.BeMats, result["Bề mặt"]);
                CollectSavedNames(projectData?.MineTopologies, result["Địa hình lò"]);
                CollectSavedNames(projectData?.MineTopologies2, result["Địa hình lò Loại 2"]);
                CollectSavedNames(projectData?.GioiHans, result["Giới hạn"]);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetSavedEntityNames error: {ex.Message}");
            }

            return result;
        }

        private static void CollectSavedNames(dynamic list, HashSet<string> into)
        {
            if (list == null) return;
            foreach (var item in list)
                if (item.Name != null) into.Add(item.Name.ToString());
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
                            Tru = SerializeSurfaceData(b.Tru)
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
                }).ToList(),
                MineTopologies = project.MineTopologies.Select(t => new
                {
                    t.Name,
                    Nen = SerializeGeoRefList(t.Nen),
                    Noc = SerializeGeoRefList(t.Noc),
                    Bien = SerializeGeoRefList(t.Bien)
                }).ToList(),
                GioiHans = project.GioiHans.Select(gh => new
                {
                    gh.Name,
                    Blocks = gh.Blocks.Select(b => new
                    {
                        b.Name,
                        Vach = SerializeSurfaceData(b.Vach),
                        Tru = SerializeSurfaceData(b.Tru)
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
                        Tru = DeserializeSurfaceData(blockData.Tru)
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

            // Reconstruct MineTopologies (Địa hình lò — Loại 1)
            if (projectData.MineTopologies != null)
            {
                foreach (var topoData in projectData.MineTopologies)
                {
                    project.MineTopologies.Add(new MineTopologyData
                    {
                        Name = topoData.Name.ToString(),
                        Nen  = topoData.Nen  != null ? DeserializeGeometryReferences(topoData.Nen)  : new List<GeometryReference>(),
                        Noc  = topoData.Noc  != null ? DeserializeGeometryReferences(topoData.Noc)  : new List<GeometryReference>(),
                        Bien = topoData.Bien != null ? DeserializeGeometryReferences(topoData.Bien) : new List<GeometryReference>()
                    });
                }
            }

            // Reconstruct GioiHans
            if (projectData.GioiHans != null)
            {
                foreach (var ghData in projectData.GioiHans)
                {
                    var gioiHan = new GioiHanData { Name = ghData.Name.ToString() };

                    foreach (var blockData in ghData.Blocks)
                    {
                        gioiHan.Blocks.Add(new GioiHanKhoiData
                        {
                            Name = blockData.Name.ToString(),
                            Vach = DeserializeSurfaceData(blockData.Vach),
                            Tru  = DeserializeSurfaceData(blockData.Tru)
                        });
                    }
                    project.GioiHans.Add(gioiHan);
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

        // A plain List<GeometryReference> (Nen/Noc/Bien on MineTopologyData) serialized
        // the same shape DeserializeGeometryReferences expects — the SurfaceData lists
        // above build this same anonymous shape inline instead, since each needs its own
        // property (SelectedGeometry vs BoundaryGeometry etc); this is the one used where
        // the list itself is the whole field.
        private object SerializeGeoRefList(List<GeometryReference> list)
        {
            return list.Select(g => new
            {
                g.Handle,
                g.SourceDwgPath,
                g.SourceDwgName,
                g.Layer,
                g.EntityType,
                g.VertexCount
            }).ToList();
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

        // Single GeometryReference helpers (for TietDien / DoanDuongLo)
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
                g.IsClosed,
                // Without this, a reloaded reference has no live ObjectId (CurrentObjectId
                // is [JsonIgnore]) and no vertices either — it can only export again if the
                // exact same drawing happens to be open with a matching Handle. Tiết diện in
                // particular is meant to be reusable across projects/drawings, so it needs
                // its own coordinates saved here, not just a pointer back to a CAD entity.
                CachedVertices = g.CachedVertices?.Select(v => new[] { v[0], v[1], v[2] }).ToList(),
                TietDienName = tietDienName   // only populated for TietDien entries
            };
        }

        private GeometryReference DeserializeGeoRef(dynamic d)
        {
            if (d == null) return null;
            var geoRef = new GeometryReference
            {
                Handle        = d.Handle        != null ? d.Handle.ToString()        : "",
                SourceDwgPath = d.SourceDwgPath != null ? d.SourceDwgPath.ToString() : "",
                SourceDwgName = d.SourceDwgName != null ? d.SourceDwgName.ToString() : "",
                Layer         = d.Layer         != null ? d.Layer.ToString()         : "",
                EntityType    = d.EntityType    != null ? d.EntityType.ToString()    : "",
                VertexCount   = d.VertexCount   != null ? (int)d.VertexCount         : 0,
                IsClosed      = d.IsClosed      != null && (bool)d.IsClosed,
                CurrentObjectId = null
            };

            if (d.CachedVertices != null)
            {
                var verts = new List<double[]>();
                foreach (var v in d.CachedVertices)
                    verts.Add(new double[] { (double)v[0], (double)v[1], (double)v[2] });
                geoRef.CachedVertices = verts;
            }

            return geoRef;
        }

        // Tiết diện Library (global, cross-project). Same .t3d treatment as the
        // project save slots above — the roaming library file is still plain JSON
        // underneath, just named so it reads as a save rather than a generic data
        // file. TietDienLibrary.json is scanned as a fallback so anyone upgrading
        // doesn't silently lose their existing library.
        private string GetTietDienLibraryPath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, "MyMiningPlugin", "TietDienLibrary" + SaveExtension);
        }

        private string GetLegacyTietDienLibraryPath()
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
                if (!File.Exists(path))
                {
                    // Fall back to a library saved before the switch to .t3d.
                    string legacyPath = GetLegacyTietDienLibraryPath();
                    if (!File.Exists(legacyPath)) return result;
                    path = legacyPath;
                }

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
