#!/bin/bash
if [ -z "$1" ]; then
    echo "put a file in dummy"
else
    if [[ "$OSTYPE" == "darwin"* ]]; then
        # Mac OSX
        prev=$PWD
        cd "${0%/*}"
        # Replace these with the paths to the binaries
        /Applications/KiCad/KiCad.app/Contents/MacOS/kicad-cli pcb export step "$1"
        /Applications/FreeCAD.app/Contents/Resources/bin/freecad ./stepToGltfFreeCAD.py
        /Applications/Blender.app/Contents/MacOS/Blender --background --python ./gltfToFbxBlender.py
        if [ -n "$2" ]; then
            timestamp=$(date +"%Y%m%d%H%M")
            mv model.fbx "$prev/$2/model_$timestamp.fbx"
        fi
        # elif [[ "$OSTYPE" == "msys" ]]; then
        # TODO later
        # /Applications/KiCad/KiCad.app/Contents/MacOS/kicad-cli pcb export step $1
        # /Applications/FreeCAD.app/Contents/Resources/bin/freecad ./stepToGltfFreeCAD.py
        # /Applications/Blender.app/Contents/MacOS/Blender --background --python ./gltfToFbxBlender.py
    fi
fi
