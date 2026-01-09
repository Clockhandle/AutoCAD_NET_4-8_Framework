using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;

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

        [CommandMethod("DrawCircle")]
        public void DrawCircle()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            PromptPointResult ppr = ed.GetPoint("\nSpecify center point: ");
            if (ppr.Status != PromptStatus.OK) return;

            PromptDoubleResult pdr = ed.GetDouble("\nSpecify radius: ");
            if (pdr.Status != PromptStatus.OK) return;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Circle circle = new Circle(ppr.Value, Vector3d.ZAxis, pdr.Value);

                btr.AppendEntity(circle);
                tr.AddNewlyCreatedDBObject(circle, true);

                tr.Commit();
            }

            ed.WriteMessage($"\nCircle created with radius {pdr.Value}");
        }
    }
}

