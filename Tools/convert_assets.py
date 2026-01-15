#!/usr/bin/env python3

import os
import json
import subprocess
import sys
from pathlib import Path

class AssetConverter:
    def __init__(self, pko_viewer_path, blender_path):
        self.pko_viewer = pko_viewer_path
        self.blender = blender_path
        self.converted_assets = []
    
    def convert_lmo_to_obj(self, lmo_file):
        print(f"Convertendo {lmo_file} para OBJ...")
        obj_file = lmo_file.replace('.lmo', '.obj')
        
        try:
            subprocess.run([self.pko_viewer, lmo_file, obj_file], check=True)
            print(f"✓ Convertido para {obj_file}")
            return obj_file
        except subprocess.CalledProcessError as e:
            print(f"✗ Erro ao converter: {e}")
            return None
    
    def convert_obj_to_fbx(self, obj_file):
        print(f"Convertendo {obj_file} para FBX...")
        fbx_file = obj_file.replace('.obj', '.fbx')
        
        blender_script = f"""
import bpy
bpy.ops.import_scene.obj(filepath='{obj_file}')
bpy.ops.export_scene.fbx(filepath='{fbx_file}')
"""
        
        try:
            subprocess.run([self.blender, '--background', '--python-expr', blender_script], check=True)
            print(f"✓ Convertido para {fbx_file}")
            return fbx_file
        except subprocess.CalledProcessError as e:
            print(f"✗ Erro ao converter: {e}")
            return None
    
    def convert_all_assets(self, input_dir, output_dir):
        os.makedirs(output_dir, exist_ok=True)
        
        for root, dirs, files in os.walk(input_dir):
            for file in files:
                if file.endswith('.lmo'):
                    lmo_path = os.path.join(root, file)
                    obj_path = self.convert_lmo_to_obj(lmo_path)
                    
                    if obj_path:
                        fbx_path = self.convert_obj_to_fbx(obj_path)
                        if fbx_path:
                            output_path = os.path.join(output_dir, os.path.basename(fbx_path))
                            os.rename(fbx_path, output_path)
                            self.converted_assets.append(output_path)
    
    def generate_manifest(self, output_dir):
        manifest = {
            "version": "1.0.0",
            "assets": self.converted_assets,
            "total": len(self.converted_assets)
        }
        
        manifest_path = os.path.join(output_dir, "manifest.json")
        with open(manifest_path, 'w') as f:
            json.dump(manifest, f, indent=2)
        
        print(f"\n✓ Manifesto gerado: {manifest_path}")
        return manifest_path

def main():
    if len(sys.argv) < 3:
        print("Uso: python3 convert_assets.py <input_dir> <output_dir> [pko_viewer_path] [blender_path]")
        sys.exit(1)
    
    input_dir = sys.argv[1]
    output_dir = sys.argv[2]
    pko_viewer = sys.argv[3] if len(sys.argv) > 3 else "PKOModelViewer.exe"
    blender = sys.argv[4] if len(sys.argv) > 4 else "blender"
    
    converter = AssetConverter(pko_viewer, blender)
    converter.convert_all_assets(input_dir, output_dir)
    converter.generate_manifest(output_dir)

if __name__ == "__main__":
    main()
