# Export Bug Fix - Geometry Resolution Issue

## Problem Summary
The export functionality was showing that lines were selected (44 lines) but the export was producing empty JSON (0 lines exported). The debug dialog showed:
- Total geometry selected: 44 lines
- JSON generated: 2 chars (empty array "[]")

## Root Cause
The issue was related to **timing of geometry resolution**. The `ResolveCurrentDrawingReferences` method was being called inside the async processing methods (`ProcessVias`, `ProcessFaults`, etc.), which could potentially execute in a different thread context where AutoCAD's document might not be accessible.

### Specific Issues:
1. **Late Resolution**: Geometry handles were being resolved AFTER the async operation started
2. **Thread Context**: Async operations might execute on different threads where AutoCAD's database isn't accessible
3. **Multiple Resolution Calls**: Each geometry type was calling resolve independently during processing

## Solution
The fix ensures all geometry references are resolved **upfront** in the calling thread (where AutoCAD context is guaranteed) BEFORE any async processing begins.

### Changes Made:

#### 1. Added `ResolveAllGeometryReferences` Method
```csharp
private void ResolveAllGeometryReferences(string category, List<string> selectedNames, MiningProject project)
```
This method:
- Runs synchronously in the main thread
- Resolves ALL geometry references for selected items BEFORE async processing
- Handles all categories (V?a, ??t gãy, Nham th?ch, L? khoan)

#### 2. Updated `ExportToFile` Method
- Calls `ResolveAllGeometryReferences` at the START of the method
- Added tracking for both total and resolved geometry counts
- Improved error messages to show if geometry wasn't resolved

#### 3. Updated `SendToServer` Method
- Calls `ResolveAllGeometryReferences` before processing
- Ensures server upload also has proper geometry resolution

#### 4. Cleaned Up Process Methods
- Removed redundant `ResolveCurrentDrawingReferences` calls from:
  - `ProcessVias`
  - `ProcessFaults`
  - `ProcessRocks`
  - `ProcessBoreholes`

#### 5. Updated `GeometryProcessingService`
- Removed the internal call to `ResolveCurrentDrawingReferences`
- Added comment that references should be resolved before calling

### Improved Diagnostics
The error dialog now shows:
- **Total geometry selected**: How many lines are in the project
- **Geometry resolved**: How many lines were successfully found in current DWG
- **Specific guidance**: Different messages based on whether resolution succeeded or failed

If `resolvedGeometryCount == 0` but `totalGeometryCount > 0`, the message now clearly states:
> "? Các ???ng ?ã ch?n KHÔNG CÓ trong file DWG hi?n t?i!"
> "Hãy m? l?i file DWG g?c ho?c ch?n l?i các ???ng"

## Why This Fix Works

### Thread Safety
- AutoCAD's API requires operations to be performed on the main thread
- By resolving geometry upfront, we ensure it happens in the correct thread context
- The resolved `CurrentObjectId` values are then safely passed to async operations

### Guarantee of Context
- When `ResolveAllGeometryReferences` is called from `ExportToFile` or `SendToServer`, it's guaranteed to be in the UI thread where AutoCAD's document is accessible
- The resolution happens synchronously before any `await` keywords are encountered

### Single Point of Resolution
- All geometry is resolved in one place at the right time
- Eliminates potential race conditions or multiple resolution attempts
- Makes the code flow more predictable and debuggable

## Testing Recommendations

1. **Test with same DWG**: Select lines and export - should work perfectly
2. **Test with different DWG**: 
   - Save project
   - Open a different DWG file
   - Load project
   - Try to export - should show clear error about lines not being in current DWG
3. **Test after reopening**: Close and reopen the form, then export
4. **Test multiple categories**: Try exporting V?a, ??t gãy, etc.

## Expected Behavior After Fix

### Successful Export:
- Geometry is resolved successfully
- JSON contains all selected lines
- Export dialog shows success message

### Failed Export (wrong DWG):
- Error dialog shows:
  - Total geometry: 44 lines
  - Resolved geometry: 0 lines
  - Clear message: "? Lines are NOT in current DWG!"
  - Guidance to open correct DWG or reselect lines

## Related Files Modified
- `Services/ExportService.cs` - Main fix implementation
- `Services/GeometryProcessingService.cs` - Removed redundant resolution call
