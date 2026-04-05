# Debugging Guide - Export Still Showing 0 Lines

## Current Situation
You're seeing the same export bug even after the fix was applied. The diagnostic shows:
- **Vách: 44 lines** (in pre-export diagnostic)
- **Tr?: 0 lines** (in export error)
- **Total geometry: 0**

## Possible Causes

### 1. **Data Structure Corruption**
The `_project` object might not be the same instance or the data might be getting cleared between selection and export.

**Symptoms:**
- Pre-export diagnostic shows lines
- Export shows 0 lines
- Data seems to disappear

**Test:**
1. Look at the new pre-export diagnostic that shows **sample handle and DWG name**
2. Check if the handles are actually populated
3. Verify the DWG name matches your current file

### 2. **Wrong Via/Block Being Selected**
You might be selecting one Via but exporting a different one.

**Test:**
1. In the pre-export diagnostic, note which Via and Block have the 44 lines
2. Make sure you're selecting the same Via in the export dialog

### 3. **Geometry Resolution Failing Silently**
The `ResolveCurrentDrawingReferences` method might be setting all `CurrentObjectId` to null.

**Test:**
1. After running, check Visual Studio **Output Window > Debug**
2. Look for lines like: `ResolveCurrentDrawingReferences [Vách]: Success=0, Failed=44`
3. If Success=0, the handles aren't being found in the current DWG

### 4. **Different DWG File**
You selected lines in one DWG file but are now in a different DWG file.

**Test:**
1. Check the pre-export diagnostic - it now shows `DWG=filename.dwg`
2. Compare with your current open DWG in AutoCAD
3. If they don't match, you need to either:
   - Open the original DWG file
   - Or re-select the lines in the current DWG

## Enhanced Diagnostics Added

### 1. AutoCADSelectionService
Now logs detailed resolution info:
```
ResolveCurrentDrawingReferences [Vách]: Success=44, Failed=0, CurrentDwg=myfile.dwg
```
This tells you:
- How many handles were successfully resolved
- How many failed
- Which DWG file is currently open

### 2. Pre-Export Diagnostic
Now shows:
```
V?a 1:
  Blocks: 1
    Kh?i 1:
      Vách: 44 lines
        Sample: Handle=1A4F, Layer=0, DWG=original.dwg
```
This shows you:
- The actual handle value
- The DWG file where lines were originally selected

### 3. Export Error Dialog
Now shows:
```
- T?ng s? geometry ?ã ch?n: 44
- Geometry ?ã resolve: 0  ? THIS IS THE KEY!
```
If resolved=0, it means the handles aren't being found.

## Step-by-Step Debugging Procedure

### Step 1: Check Pre-Export Diagnostic
When you click "Xu?t JSON", the first dialog shows:
```
=== PRE-EXPORT DIAGNOSTIC ===
S? V?a: 1

V?a 1:
  Blocks: 1
    Kh?i 1:
      Vách: 44 lines
        Sample: Handle=1A4F, Layer=0, DWG=myfile.dwg
      Tr?: 0 lines

TOTAL: Vách=44, Tr?=0, Grand Total=44
```

**What to check:**
- Is the handle populated (not empty)?
- Does the DWG name match your current open file in AutoCAD?
- Is the layer name correct?

### Step 2: Check Visual Studio Output Window
After the export fails:
1. Go to Visual Studio
2. View > Output (or Ctrl+Alt+O)
3. Select "Debug" from the dropdown
4. Look for lines starting with `ResolveCurrentDrawingReferences`

**What you should see (GOOD):**
```
ResolveCurrentDrawingReferences [Vách]: Success=44, Failed=0, CurrentDwg=myfile.dwg
```

**What you might see (BAD):**
```
ResolveCurrentDrawingReferences [Vách]: Success=0, Failed=44, CurrentDwg=differentfile.dwg
Failed to resolve handle 1A4F: Object not found
```

### Step 3: Check Export Error Dialog
If export fails, the error dialog shows:
```
Không có d? li?u ?? export!

Debug Info:
- T?ng s? geometry ?ã ch?n: 44
- Geometry ?ã resolve: 0  ? PROBLEM IS HERE
- JSON generated: 2 chars
```

If resolved=0:
- The lines are NOT in the current DWG
- You need to open the original DWG
- Or re-select the lines

## Common Scenarios & Solutions

### Scenario A: Wrong DWG File Open
**Symptoms:**
- Pre-export shows `DWG=file1.dwg`
- Current AutoCAD has `file2.dwg` open
- Resolved=0

**Solution:**
- Open `file1.dwg` in AutoCAD
- Or delete the Via and create a new one with fresh selection

### Scenario B: Data Structure Issue
**Symptoms:**
- Pre-export shows 44 lines with valid handles
- Export shows 0 lines
- Handles are empty strings or null

**Solution:**
- This is a memory/reference issue
- Try closing and reopening the form
- Try saving and loading the project

### Scenario C: Transaction/Threading Issue
**Symptoms:**
- Debug output shows "No active document!"
- Or shows transaction errors

**Solution:**
- The form might be running on wrong thread
- Ensure you're running from AutoCAD command line: `OPENMINING`

### Scenario D: Handle Format Issue
**Symptoms:**
- Debug output shows "Failed to resolve handle: Invalid format"

**Solution:**
- The handle string might be malformed
- Check if handles are in correct hexadecimal format (e.g., "1A4F")

## Quick Test Script

Add this test button to your form to verify data integrity:

```csharp
private void TestDataIntegrity()
{
    string msg = "=== DATA INTEGRITY TEST ===\n\n";
    
    foreach (var via in _project.Vias)
    {
        msg += $"Via: {via.Name} (Ref: {via.GetHashCode()})\n";
        foreach (var block in via.Blocks)
        {
            msg += $"  Block: {block.Name}\n";
            msg += $"    Vach.SelectedGeometry: {block.Vach.SelectedGeometry.Count} items (Ref: {block.Vach.SelectedGeometry.GetHashCode()})\n";
            if (block.Vach.SelectedGeometry.Count > 0)
            {
                var first = block.Vach.SelectedGeometry[0];
                msg += $"      First: Handle={first.Handle ?? "NULL"}, DWG={first.SourceDwgName ?? "NULL"}\n";
            }
        }
    }
    
    MessageBox.Show(msg, "Data Integrity", MessageBoxButtons.OK);
}
```

## Next Steps

1. **Run the export again** and note all three pieces of information:
   - Pre-export diagnostic (note the Sample line)
   - Visual Studio Debug output
   - Export error dialog

2. **Report back with:**
   - What DWG file is open in AutoCAD?
   - What does the Sample line show in pre-export diagnostic?
   - What does Debug output show for ResolveCurrentDrawingReferences?
   - What is the "Geometry ?ã resolve" count?

3. **Most likely issue:**
   The lines were selected in a different DWG file than the one currently open.
   
   **Quick fix:** Open the correct DWG file that matches the SourceDwgName shown in diagnostic.
