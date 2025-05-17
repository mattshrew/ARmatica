#!/bin/bash
if [ -z "$1" ]; then
    echo "put a file in dummy"
else
    # Replace these with the paths to the binaries
    /Applications/KiCad/KiCad.app/Contents/MacOS/kicad-cli pcb export step $1
    /Applications/FreeCAD.app/Contents/Resources/bin/freecad ./stepToGltfFreeCAD.py
    /Applications/Blender.app/Contents/MacOS/Blender --background --python ./gltfToFbxBlender.py
fi
