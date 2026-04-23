using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ExcelDataReader;
using MyMiningPlugin.Models;
using Newtonsoft.Json;

namespace MyMiningPlugin.Services
{
    public static class BoreholeExcelParser
    {
        public static List<BoreholeData> ParseAllBoreholes(string filePath)
        {
            var boreholes = new Dictionary<string, BoreholeData>();

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Excel file not found at: {filePath}");
            }

            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    // Auto-detect format, supports .xls and .xlsx
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // Read all sheets into a single DataSet
                        var result = reader.AsDataSet();

                        // 1. Read THIET DO (Sheet 1)
                        DataTable thietDoTable = GetTableByName(result, "THIET DO") ?? result.Tables[0];
                        if (thietDoTable != null)
                        {
                            // Start at row index 1 to skip the header row
                            for (int r = 1; r < thietDoTable.Rows.Count; r++)
                            {
                                var row = thietDoTable.Rows[r];
                                
                                // Ensure row has enough columns
                                if (row.ItemArray.Length < 12) continue; // We need up to col L (index 11)

                                string lkStr = Convert.ToString(row[1])?.Trim(); // Col B is index 1

                                if (!string.IsNullOrEmpty(lkStr))
                                {
                                    if (!boreholes.TryGetValue(lkStr, out var borehole))
                                    {
                                        borehole = new BoreholeData
                                        {
                                            Name = lkStr,
                                            ExcelFilePath = filePath
                                        };
                                        // Coordinates (Swapped for AutoCAD)
                                        borehole.X = GetDoubleValue(row[3]); // Col D (index 3)
                                        borehole.Y = GetDoubleValue(row[2]); // Col C (index 2)
                                        borehole.Z = GetDoubleValue(row[4]); // Col E (index 4)
                                        boreholes[lkStr] = borehole;
                                    }

                                    double? fromVal = GetNullableDoubleValue(row[10]); // Col K (index 10)
                                    double? toVal = GetNullableDoubleValue(row[11]);   // Col L (index 11)
                                    string seamName = Convert.ToString(row[8])?.Trim(); // Col I (index 8) - THAN

                                    if (fromVal.HasValue && toVal.HasValue)
                                    {
                                        if (!string.IsNullOrEmpty(seamName) && 
                                            borehole.Intervals.Count > 0 && 
                                            borehole.Intervals.Last().SeamName == seamName)
                                        {
                                            // Extend the existing interval
                                            borehole.Intervals.Last().To = toVal.Value;
                                        }
                                        else
                                        {
                                            borehole.Intervals.Add(new DepthInterval
                                            {
                                                From = fromVal.Value,
                                                To = toVal.Value,
                                                SeamName = seamName
                                            });
                                        }
                                    }
                                }
                            }
                        }

                        // 2. Read DO CONG (Sheet 2)
                        DataTable doCongTable = GetTableByName(result, "DO CONG") ?? (result.Tables.Count > 1 ? result.Tables[1] : null);
                        if (doCongTable != null)
                        {
                            for (int r = 1; r < doCongTable.Rows.Count; r++)
                            {
                                var row = doCongTable.Rows[r];

                                // Ensure row has enough columns
                                if (row.ItemArray.Length < 5) continue; // We need up to col E (index 4)

                                string lkStr = Convert.ToString(row[0])?.Trim(); // Col A is index 0

                                if (!string.IsNullOrEmpty(lkStr) && boreholes.TryGetValue(lkStr, out var borehole))
                                {
                                    double? csoVal = GetNullableDoubleValue(row[1]); // Col B (index 1)
                                    double? doVal = GetNullableDoubleValue(row[2]);  // Col C (index 2)
                                    double? pviVal = GetNullableDoubleValue(row[4]); // Col E (index 4)

                                    if (csoVal.HasValue && doVal.HasValue && pviVal.HasValue)
                                    {
                                        borehole.Trajectory.Add(new SurveyReading
                                        {
                                            DepthRange = csoVal.Value,
                                            DO = doVal.Value,
                                            PVI = pviVal.Value
                                        });
                                    }
                                }
                            }
                        }
                    }
                }

                // Temporary JSON Dump for verification
                string tempJsonPath = Path.Combine(Path.GetDirectoryName(filePath), "Test_Boreholes_All.json");
                string jsonOutput = JsonConvert.SerializeObject(boreholes.Values.ToList(), Formatting.Indented);
                File.WriteAllText(tempJsonPath, jsonOutput);
                
                System.Windows.Forms.MessageBox.Show($"Test Data Read and saved to:\n{tempJsonPath}", "Success", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

                return boreholes.Values.ToList();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Lỗi đọc file Excel: {ex.Message}", "Lỗi", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }

        private static DataTable GetTableByName(DataSet dataSet, string name)
        {
            foreach (DataTable table in dataSet.Tables)
            {
                if (table.TableName == name)
                {
                    return table;
                }
            }
            return null;
        }

        private static double GetDoubleValue(object cellValue)
        {
            if (cellValue == null || cellValue == DBNull.Value) return 0;
            if (double.TryParse(cellValue.ToString(), out double result)) return result;
            return 0;
        }

        private static double? GetNullableDoubleValue(object cellValue)
        {
            if (cellValue == null || cellValue == DBNull.Value || string.IsNullOrWhiteSpace(cellValue.ToString())) return null;
            if (double.TryParse(cellValue.ToString(), out double result)) return result;
            return null;
        }
    }
}