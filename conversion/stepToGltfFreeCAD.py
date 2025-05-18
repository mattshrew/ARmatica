import sys

sys.path.append("/usr/lib/freecad/lib")
import FreeCAD
import ImportGui
import FreeCADGui

doc = FreeCAD.newDocument()
ImportGui.insert("./breadboard.step", doc.Name)
doc.removeObject(doc.getObjectsByLabel("breadboard_PCB")[0].Name)
# FreeCADGui.Selection.addSelection("Unnamed", "breadboard_1")
# FreeCADGui.runCommand("Std_DlgMacroExecute", 0)
__objs__ = [doc.getObject("breadboard_1")]
path = "model.gltf"
if hasattr(ImportGui, "exportOptions"):
    options = ImportGui.exportOptions(path)
    ImportGui.export(__objs__, path, options)
else:
    ImportGui.export(__objs__, path)

del __objs__
FreeCADGui.getMainWindow().deleteLater()
