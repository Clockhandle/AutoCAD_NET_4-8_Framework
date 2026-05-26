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
                if (form is MiningManagerDForm)
                {
                    form.Activate();
                    return;
                }
            }

            MiningManagerDForm myForm = new MiningManagerDForm();
            Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(myForm);
        }
    }
}

