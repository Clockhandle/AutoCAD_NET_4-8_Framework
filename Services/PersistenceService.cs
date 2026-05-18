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
    /// Handles project persistence (Save/Load to JSON)
    /// </summary>
    public class PersistenceService
    {
        private string GetProjectDataPath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string projectFolder = Path.Combine(appData, "MyMiningPlugin");
            
            if (!Directory.Exists(projectFolder))
                Directory.CreateDirectory(projectFolder);
            
            return Path.Combine(projectFolder, "MiningProject.json");
        }

        public void SaveProjectData(MiningProject project)
        {
            try
            {
                var projectData = new
                {
                    SavedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Vias = project.Vias.Select(v => new
                    {
                        v.Name,
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
                        b.Trajectory
                    }).ToList()
                };

                string json = JsonConvert.SerializeObject(projectData, Formatting.Indented);
                File.WriteAllText(GetProjectDataPath(), json);
                
                MessageBox.Show($"Đã lưu dự án tại:\n{GetProjectDataPath()}", "Lưu thành công", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    public MiningProject LoadProjectData(bool silent = false)
    {
        try
        {
            string path = GetProjectDataPath();
            if (!File.Exists(path))
            {
                if (!silent)
                    MessageBox.Show("Chưa có dữ liệu dự án đã lưu.", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

                string json = File.ReadAllText(path);
                dynamic projectData = JsonConvert.DeserializeObject<dynamic>(json);

                MiningProject project = new MiningProject();

                // Reconstruct Vias
                foreach (var viaData in projectData.Vias)
                {
                    ViaData via = new ViaData { Name = viaData.Name.ToString() };
                    
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
                                    To = (double)interval.To
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

                        project.Boreholes.Add(borehole);
                    }
                }

                if (!silent)
                    MessageBox.Show($"Đã tải dự án từ:\n{path}", "Tải thành công", 
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
    }
}
