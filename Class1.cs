using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using Newtonsoft.Json;

namespace AutoCAD_NET_4_8_Framework
{
    public class Class1
    {
        private static Form1 _myForm;

        [CommandMethod("OpenObjectSelectionUI")]
        public void OpenObjectSelectionUI()
        {
            if (_myForm == null || _myForm.IsDisposed)
            {
                _myForm = new Form1();
            }

            // Passing 'null' as the owner creates a floating window
            Application.ShowModelessDialog(null, _myForm, false);
        }

        [CommandMethod("HelloAutoCAD")]
        public void HelloAutoCAD()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            ed.WriteMessage("\nHello from AutoCAD .NET API!");
        }

        [CommandMethod("TestJson")]
        public void TestJsonCAD()
        {
            try
            {
                var jsonObj = new
                {
                    Name = "AutoCAD Json Object",
                    Success = true,
                    Timestamp = DateTime.Now
                };

                Document doc = Application.DocumentManager.MdiActiveDocument;
                Editor ed = doc.Editor;
                ed.WriteMessage(JsonConvert.SerializeObject(jsonObj));
            }
            catch (System.Exception ex)
            {
                Document doc = Application.DocumentManager.MdiActiveDocument;
                Editor ed = doc.Editor;
                ed.WriteMessage($"\nError: {ex.Message}");
            }
        }

        [CommandMethod("DrawLine")]
        public void DrawLine()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            using (Transaction tm = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tm.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord btr = tm.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Line line = new Line(new Point3d(0, 0, 0), new Point3d(100, 100, 0));

                btr.AppendEntity(line);

                tm.AddNewlyCreatedDBObject(line, true);

                tm.Commit();
            }

            ed.WriteMessage("\nLine drawn from (0,0,0) to (100,100,0).");
        }
    }
}

