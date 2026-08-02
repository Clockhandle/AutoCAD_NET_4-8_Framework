using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using AcApp = Autodesk.AutoCAD.ApplicationServices.Application;

namespace MyMiningPlugin.Services
{
    /// <summary>
    /// Draws quality-check markers directly onto the AutoCAD drawing so operators
    /// can see and navigate to problem entities without leaving CAD.
    /// Errors ? layer DQ_ERRORS  (red,    circle radius = 2)
    /// Warnings ? layer DQ_WARNINGS (yellow, circle radius = 1.5)
    /// Notes ? layer DQ_NOTES    (cyan,   circle radius = 1)
    /// Each marker has an MText label offset above the circle.
    /// </summary>
    public class QualityMarkerService
    {
        // Layer names
        private const string LayerErrors   = "DQ_ERRORS";
        private const string LayerWarnings = "DQ_WARNINGS";
        private const string LayerNotes    = "DQ_NOTES";

        // AutoCAD color indices
        private const short ColorError   = 1;  // Red
        private const short ColorWarning = 2;  // Yellow
        private const short ColorNote    = 4;  // Cyan

        // Marker sizes (drawing units)
        private const double RadiusError   = 2.0;
        private const double RadiusWarning = 1.5;
        private const double RadiusNote    = 1.0;

        // MText label offset above circle centre
        private const double LabelOffsetY = 3.0;

        // Text height for labels
        private const double TextHeight = 1.2;

        /// <summary>
        /// Place markers for all issues in the active drawing.
        /// Returns a summary string suitable for a MessageBox.
        /// </summary>
        public string PlaceMarkers(List<QualityIssue> issues)
        {
            if (issues == null || issues.Count == 0)
                return "Khong co van de chat luong nao duoc phat hien.";

            Document doc = AcApp.DocumentManager.MdiActiveDocument;
            if (doc == null) return "Khong co ban ve dang mo.";

            // Always clear previous markers first so repeat runs don't stack
            ClearMarkers();

            Database db  = doc.Database;

            int errorCount   = 0;
            int warningCount = 0;
            int noteCount    = 0;

            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord ms = tr.GetObject(
                    bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // Ensure marker layers exist
                EnsureLayer(db, tr, LayerErrors,   ColorError);
                EnsureLayer(db, tr, LayerWarnings, ColorWarning);
                EnsureLayer(db, tr, LayerNotes,    ColorNote);

                foreach (var issue in issues)
                {
                    if (issue.XY == null || issue.XY.Length < 2) continue;

                    double cx = issue.XY[0];
                    double cy = issue.XY[1];
                    double cz = issue.Z ?? 0.0;

                    string layer;
                    double radius;
                    switch (issue.Severity)
                    {
                        case QualityCheckSeverity.Error:
                            layer  = LayerErrors;
                            radius = RadiusError;
                            errorCount++;
                            break;
                        case QualityCheckSeverity.Warning:
                            layer  = LayerWarnings;
                            radius = RadiusWarning;
                            warningCount++;
                            break;
                        default:
                            layer  = LayerNotes;
                            radius = RadiusNote;
                            noteCount++;
                            break;
                    }

                    // Circle at problem location
                    var circle = new Circle(
                        new Point3d(cx, cy, cz),
                        Vector3d.ZAxis,
                        radius);
                    circle.Layer = layer;
                    ms.AppendEntity(circle);
                    tr.AddNewlyCreatedDBObject(circle, true);

                    // MText label above the circle
                    string label = BuildLabel(issue);
                    var mtext = new MText();
                    mtext.Location    = new Point3d(cx, cy + LabelOffsetY, cz);
                    mtext.TextHeight  = TextHeight;
                    mtext.Layer       = layer;
                    mtext.Contents    = label;
                    ms.AppendEntity(mtext);
                    tr.AddNewlyCreatedDBObject(mtext, true);
                }

                tr.Commit();
            }

            return BuildSummary(errorCount, warningCount, noteCount, issues);
        }

        /// <summary>
        /// Remove all DQ_* marker layers and their entities from the drawing.
        /// </summary>
        public string ClearMarkers()
        {
            Document doc = AcApp.DocumentManager.MdiActiveDocument;
            if (doc == null) return "Khong co ban ve dang mo.";

            Database db = doc.Database;
            int removed = 0;

            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord ms = tr.GetObject(
                    bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var markerLayers = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    { LayerErrors, LayerWarnings, LayerNotes };

                // Delete all entities on marker layers
                foreach (ObjectId id in ms)
                {
                    Entity ent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                    if (ent == null) continue;
                    if (!markerLayers.Contains(ent.Layer)) continue;

                    ent.UpgradeOpen();
                    ent.Erase();
                    removed++;
                }

                // Delete the layers themselves (only if now empty)
                foreach (string layerName in markerLayers)
                {
                    if (!lt.Has(layerName)) continue;
                    LayerTableRecord ltr = tr.GetObject(lt[layerName], OpenMode.ForWrite)
                        as LayerTableRecord;
                    try { ltr.Erase(); }
                    catch { /* layer may still have refs — leave it */ }
                }

                tr.Commit();
            }

            return $"Da xoa {removed} marker chat luong khoi ban ve.";
        }

        // -------------------------------------------------------------------------
        // Helpers
        // -------------------------------------------------------------------------

        private static void EnsureLayer(Database db, Transaction tr,
            string name, short colorIndex)
        {
            LayerTable lt = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
            if (lt.Has(name)) return;

            lt.UpgradeOpen();
            var ltr = new LayerTableRecord
            {
                Name  = name,
                Color = Color.FromColorIndex(ColorMethod.ByAci, colorIndex)
            };
            lt.Add(ltr);
            tr.AddNewlyCreatedDBObject(ltr, true);
        }

        private static string BuildLabel(QualityIssue issue)
        {
            string sev = issue.Severity == QualityCheckSeverity.Error ? "[!]" :
                         issue.Severity == QualityCheckSeverity.Warning ? "[W]" : "[i]";
            string zStr = issue.Z.HasValue ? $" Z={issue.Z.Value:F2}" : "";
            return $"{sev} {issue.Check}\\P" +
                   $"{issue.Handle} | {issue.Layer}{zStr}\\P" +
                   $"{TruncateMessage(issue.Message, 60)}";
        }

        private static string TruncateMessage(string msg, int maxLen)
        {
            if (msg == null) return "";
            return msg.Length <= maxLen ? msg : msg.Substring(0, maxLen) + "...";
        }

        private static string BuildSummary(int errors, int warnings, int notes,
            List<QualityIssue> issues)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Loi: {errors}  Canh bao: {warnings}  Ghi chu: {notes}");
            if (errors > 0)
            {
                sb.AppendLine("\nCan sua (vong do):");
                foreach (var i in issues.Where(x => x.Severity == QualityCheckSeverity.Error))
                    sb.AppendLine($"  {i.Handle} [{i.Layer}] {i.Message}");
            }
            if (warnings > 0)
            {
                sb.AppendLine("\nCanh bao (vong vang):");
                foreach (var i in issues.Where(x => x.Severity == QualityCheckSeverity.Warning))
                    sb.AppendLine($"  {i.Handle} [{i.Layer}] {i.Message}");
            }
            sb.AppendLine(errors > 0
                ? "\n[!] Co loi - xem vong do tren ban ve."
                : "\n[OK] Khong co loi.");
            return sb.ToString();
        }
    }
}
