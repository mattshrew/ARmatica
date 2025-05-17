import sys

sys.path.append("/usr/lib/freecad/lib")
import FreeCAD
import ImportGui

doc = FreeCAD.newDocument()
ImportGui.insert("../kicad/breadboard.step", doc.Name)
doc.removeObject("Part__Feature004")
__objs__ = [doc.getObject("breadboard_1")]
path = "model.gltf"
if hasattr(ImportGui, "exportOptions"):
    options = ImportGui.exportOptions(path)
    ImportGui.export(__objs__, path, options)
else:
    ImportGui.export(__objs__, path)

del __objs__
exit()
