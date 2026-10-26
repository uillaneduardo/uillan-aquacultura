#!/usr/bin/env python3
"""Activate tank components only after compiling the matching game assembly."""
import argparse
from pathlib import Path
from xml.etree import ElementTree as ET
ROOT = Path(__file__).resolve().parents[1]
CAPACITIES = {"AQ_TankSmall": 8, "AQ_TankMedium": 24, "AQ_TankLarge": 60}
def activate(version):
    if version not in ("1.5", "1.6", "1.7"):
        raise ValueError("Unsupported game version")
    folder = ROOT / "Aquacultura" / version
    dll = folder / "Assemblies" / "Aquacultura.RimWorld.dll"
    if not dll.is_file() or dll.stat().st_size == 0:
        raise FileNotFoundError("Compile RimWorld DLL for " + version + " before activating")
    target = folder / "Patches" / "TankComponents.xml"
    root = ET.Element("Patch")
    for name, capacity in CAPACITIES.items():
        op = ET.SubElement(root, "Operation", {"Class":"PatchOperationAdd"})
        ET.SubElement(op, "xpath").text = "/Defs/ThingDef[defName='" + name + "']"
        value = ET.SubElement(op, "value")
        comps = ET.SubElement(value, "comps")
        li = ET.SubElement(comps, "li", {"Class": "Uillan.Aquacultura.CompProperties_Tank"})
        ET.SubElement(li, "capacity").text = str(capacity)
    target.parent.mkdir(parents=True, exist_ok=True)
    ET.indent(root, space="  ")
    ET.ElementTree(root).write(target, encoding="utf-8", xml_declaration=True)
    print("Activated for", version, ":", target)
if __name__ == "__main__":
    parser=argparse.ArgumentParser()
    parser.add_argument("--version", choices=("1.5","1.6","1.7"), required=True)
    args=parser.parse_args()
    activate(args.version)
