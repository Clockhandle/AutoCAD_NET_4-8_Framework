using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AutoCAD_NET_4_8_Framework
{
    public class AutoCADObjectData
    {
        public Dictionary<string, List<string>> Seams { get; set; } = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> Roofs { get; set; } = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> Floors { get; set; } = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> Faults { get; set; } = new Dictionary<string, List<string>>();
    }

    // --- Manager Class for Saving/Loading State ---
    public static class AutoCADObjectStateManager
    {
        private const string DICT_NAME = "AutoCADObject_Data";
        private const string KEY_NAME = "ProjectJSON";
        public static void SaveState(
            Document doc,
            Dictionary<string, HashSet<ObjectId>> seams,
            Dictionary<string, HashSet<ObjectId>> roofs,
            Dictionary<string, HashSet<ObjectId>> floors,
            Dictionary<string, HashSet<ObjectId>> faults
            )
        {
            var state = new AutoCADObjectData();
            state.Seams = ConvertToHandles(seams);
            state.Roofs = ConvertToHandles(roofs);
            state.Floors = ConvertToHandles(floors);
            state.Faults = ConvertToHandles(faults);

            string json = JsonConvert.SerializeObject(state);

            using (DocumentLock docLock = doc.LockDocument())
            using (Transaction tr = doc.Database.TransactionManager.StartTransaction())
            {
                DBDictionary nod = (DBDictionary)tr.GetObject(doc.Database.NamedObjectsDictionaryId, OpenMode.ForWrite);

                DBDictionary myDict;
                if (nod.Contains(DICT_NAME))
                {
                    myDict = (DBDictionary)tr.GetObject(nod.GetAt(DICT_NAME), OpenMode.ForWrite);
                }
                else
                {
                    myDict = new DBDictionary();
                    nod.SetAt(DICT_NAME, myDict);
                    tr.AddNewlyCreatedDBObject(myDict, true);
                }

                Xrecord myXRecord = new Xrecord();
                myXRecord.Data = new ResultBuffer(new TypedValue((int)DxfCode.Text, json));

                // Save or overwrite the Xrecord in the dictionary
                if (myDict.Contains(KEY_NAME))
                {
                    myDict.Remove(KEY_NAME);
                }
                myDict.SetAt(KEY_NAME, myXRecord);
                tr.AddNewlyCreatedDBObject(myXRecord, true);
                tr.Commit();
            }
        }
        // -----------------------------------------

        public static AutoCADObjectData LoadState(Document doc)
        {
            using (Transaction tr = doc.TransactionManager.StartTransaction())
            {
                DBDictionary nod = (DBDictionary)tr.GetObject(doc.Database.NamedObjectsDictionaryId, OpenMode.ForRead);

                if (!nod.Contains(DICT_NAME)) return null;

                DBDictionary myDict = (DBDictionary)tr.GetObject(nod.GetAt(DICT_NAME), OpenMode.ForRead);

                if (!myDict.Contains(KEY_NAME)) return null;

                Xrecord myXRecord = (Xrecord)tr.GetObject(myDict.GetAt(KEY_NAME), OpenMode.ForRead);

                // Read the JSON string
                TypedValue[] data = myXRecord.Data.AsArray();
                if (data.Length > 0 && data[0].TypeCode == (int)DxfCode.Text)
                {
                    string json = (string)data[0].Value;
                    return JsonConvert.DeserializeObject<AutoCADObjectData>(json);
                }
            }
            return null;
        }

        // --- Helper methods ---
        private static Dictionary<string, List<string>> ConvertToHandles(Dictionary<string, HashSet<ObjectId>> groups)
        {
            var result = new Dictionary<string, List<string>>();
            foreach (var kvp in groups)
            {
                var handles = kvp.Value.
                    Where(id => id.IsValid && !id.IsErased).
                    Select(id => id.Handle.ToString()).
                    ToList();

                if(handles.Count > 0) result.Add(kvp.Key, handles);
            }
            return result;
        }

        // Convert ProjectState back to Live ObjectIds
        public static void ReconstructDictionaries(
            Database db,
            AutoCADObjectData state,
            out Dictionary<string, HashSet<ObjectId>> seams,
            out Dictionary<string, HashSet<ObjectId>> roofs,
            out Dictionary<string, HashSet<ObjectId>> floors,
            out Dictionary<string, HashSet<ObjectId>> faults)
        {
            seams = ConvertToIds(db, state.Seams);
            roofs = ConvertToIds(db, state.Roofs);
            floors = ConvertToIds(db, state.Floors);
            faults = ConvertToIds(db, state.Faults);
        }

        private static Dictionary<string, HashSet<ObjectId>> ConvertToIds(Database db, Dictionary<string, List<string>> source)
        {
            var result = new Dictionary<string, HashSet<ObjectId>>();
            if (source == null) return result;

            foreach (var kvp in source)
            {
                var idSet = new HashSet<ObjectId>();
                foreach (string handleHex in kvp.Value)
                {
                    try
                    {
                        long ln = System.Convert.ToInt64(handleHex, 16);
                        Handle h = new Handle(ln);
                        ObjectId id = db.GetObjectId(false, h, 0);
                        if (!id.IsNull) idSet.Add(id);
                    }
                    catch { /* Handle might be invalid or deleted, ignore */ }
                }
                if (idSet.Count > 0) result.Add(kvp.Key, idSet);
            }
            return result;
        }
    }
}