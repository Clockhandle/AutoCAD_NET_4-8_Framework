# Export Debugging Guide

## Issues Found

### Issue 1: Vietnamese Text in Dialog (Minor)
The Vietnamese text in the dialog buttons shows correctly in source code but appears corrupted in UI.

**Solution**: This is because AutoCAD hasn't reloaded the new DLL.

**Fix Steps**:
1. Close AutoCAD completely
2. Reopen AutoCAD
3. Type `NETLOAD` and load the DLL again
4. Open the Mining Manager form
5. Vietnamese text should display correctly now

### Issue 2: Export Shows "No Data" Even With 44 Lines Selected (MAJOR)

From your screenshots I can see:
- ? You have 44 lines selected (with ? marks)
- ? Lines are in current DWG
- ? Export says "Không có d? li?u" (No data)

## What I've Changed

### 1. Improved Error Messages
The export will now show:
```
Không có d? li?u ?? export!

Debug Info:
- T?ng s? geometry ?ã ch?n: 44
- JSON generated: 2 chars

Vui lòng ki?m tra:
- ???ng ?ã ch?n có trong file DWG hi?n t?i không?
- Th? ?óng và m? l?i form, sau ?ó export l?i
```

This will help identify WHERE the problem is.

### 2. Fixed ProcessVias Logic
**Before**:
```csharp
bool hasData = via.Blocks.Any(b => b.Vach.SelectedGeometry.Count > 0 || b.Tru.SelectedGeometry.Count > 0);
if (!hasData) continue;

foreach (var khoi in via.Blocks)
{
    // Process even empty blocks
    var vachGeometryList = await _geometryProcessor.ProcessGeometryWithSmartZ(khoi.Vach);
    var truGeometryList = await _geometryProcessor.ProcessGeometryWithSmartZ(khoi.Tru);
}
```

**After**:
```csharp
foreach (var khoi in via.Blocks)
{
    // Only process if has geometry
    if (khoi.Vach.SelectedGeometry.Count > 0)
    {
        var vachGeometryList = await _geometryProcessor.ProcessGeometryWithSmartZ(khoi.Vach);
        // ...
    }
    
    if (khoi.Tru.SelectedGeometry.Count > 0)
    {
        var truGeometryList = await _geometryProcessor.ProcessGeometryWithSmartZ(khoi.Tru);
        // ...
    }
}
```

### 3. Added Debug Output in GeometryProcessingService
The service now tracks:
- How many geometries were processed
- How many were skipped
- Outputs to Debug console

## Testing Steps

### Test 1: Check If Lines Are Really Selected
1. Open your form
2. Click on "V?a 1" ? "Kh?i 1" ? "Vách"
3. Check the summary line: "T?ng: 44 lines | Trong DWG này: 44 lines"
4. If the second number is 0, the lines are NOT in the current DWG

### Test 2: Try Export Again
1. Make sure AutoCAD DLL is reloaded (close/reopen AutoCAD + NETLOAD)
2. Select "V?a 1" for export
3. Look at the error message - it will now show debug info
4. Take a screenshot of the debug message

### Test 3: Check AutoCAD Debug Output
1. Open Visual Studio
2. Go to Debug ? Windows ? Output
3. Run the export
4. Look for lines like: `ProcessGeometryWithSmartZ: Processed 0, Skipped 44 out of 44 total`

## Possible Root Causes

### Cause A: ObjectId Not Being Resolved
**Symptom**: UI shows ? marks but export fails
**Reason**: The `CurrentObjectId` is set when you view the UI, but when export runs, it's in a different AutoCAD document context

**Solution**: Force resolve before export
```csharp
// Before processing, explicitly resolve
_selectionService.ResolveCurrentDrawingReferences(surface);
```

### Cause B: Transaction/Database Context Issue
**Symptom**: ObjectId is valid but `tr.GetObject()` fails
**Reason**: The transaction might be in the wrong database context

**Test**: Add try-catch in GeometryProcessingService and log the exception

### Cause C: Lines Selected in Different Drawing
**Symptom**: Lines show ? instead of ?
**Reason**: You selected lines in one DWG but now have a different DWG open

**Solution**: Re-select the lines in the current DWG

## Next Steps for You

1. **Reload the DLL**:
   ```
   - Close AutoCAD
   - Reopen
   - NETLOAD ? Select new DLL
   - Open Mining Manager
   ```

2. **Try Export Again** and note:
   - What does the debug message say?
   - How many geometries were selected?
   - How many chars in JSON?

3. **Check Debug Output**:
   - Open Visual Studio Output window
   - Look for "ProcessGeometryWithSmartZ:" messages
   - Send me screenshot

4. **If Still Fails**, try this workaround:
   - Close the Mining Manager form
   - Reopen it
   - Click on Vách node to refresh
   - Try export again

## Quick Fix to Try Now

Add this code to force re-resolution before export. In `ExportService.cs`, modify `ProcessVias`:

```csharp
foreach (var khoi in via.Blocks)
{
    // FORCE RE-RESOLVE before processing
    _geometryProcessor._selectionService.ResolveCurrentDrawingReferences(khoi.Vach);
    _geometryProcessor._selectionService.ResolveCurrentDrawingReferences(khoi.Tru);
    
    if (khoi.Vach.SelectedGeometry.Count > 0)
    {
        var vachGeometryList = await _geometryProcessor.ProcessGeometryWithSmartZ(khoi.Vach);
        // ...
    }
}
```

But wait - `_selectionService` is private! Let me fix that...
