using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using Newtonsoft.Json;

namespace AutoCAD_NET_4_8_Framework
{
    public class MainLoader
    {
        private static UI_Events _myForm;
        [CommandMethod("OPENMINING")]
        public void OpenMiningUI()
        {
            // Check if form is already open to avoid duplicates
            foreach (System.Windows.Forms.Form form in System.Windows.Forms.Application.OpenForms)
            {
                if (form is MyMiningPlugin.MiningManagerForm)
                {
                    form.Activate();
                    return;
                }
            }

            MyMiningPlugin.MiningManagerForm myForm = new MyMiningPlugin.MiningManagerForm();
            Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(myForm);
        }
        [CommandMethod("OpenObjectSelectionUI")]
        public void OpenObjectSelectionUI()
        {
            if (_myForm == null || _myForm.IsDisposed)
            {
                _myForm = new UI_Events();
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

