import bpy
import os

bpy.ops.import_scene.gltf(filepath="model.gltf")
bpy.ops.object.origin_set(type="ORIGIN_GEOMETRY", center="BOUNDS")
bpy.ops.export_scene.fbx(filepath="model.fbx", path_mode="COPY", embed_textures=True)

os.remove("model.gltf")
