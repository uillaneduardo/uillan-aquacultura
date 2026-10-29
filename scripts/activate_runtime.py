#!/usr/bin/env python3
"""Activate tank components only after compiling the matching game assembly.

Keep patches compatible with ThingDefs that already define a comps node.
"""
import argparse
from pathlib import Path
from xml.etree import ElementTree as ET

ROOT = Path(__file__).resolve().parents[1]
CAPACITIES = {"AQ_TankSmall": 8, "AQ_TankMedium": 24, "AQ_TankLarge": 60}
COMP_CLASS = "Uillan.Aquacultura.CompProperties_Tank"


def _component(parent, capacity):
    li = ET.SubElement(parent, "li", {"Class": COMP_CLASS})
    ET.SubElement(li, "capacity").text = str(capacity)


def make_patch():
    """Generate a conditional patch so a pre-existing comps list is extended, not duplicated."""
    root = ET.Element("Patch")
    for name, capacity in CAPACITIES.items():
        tank_xpath = "/Defs/ThingDef[defName='" + name + "']"
        comps_xpath = tank_xpath + "/comps"
        conditional = ET.SubElement(root, "Operation", {"Class": "PatchOperationConditional"})
        ET.SubElement(conditional, "xpath").text = comps_xpath

        existing = ET.SubElement(conditional, "match", {"Class": "PatchOperationAdd"})
        ET.SubElement(existing, "xpath").text = comps_xpath
        _component(ET.SubElement(existing, "value"), capacity)

        missing = ET.SubElement(conditional, "nomatch", {"Class": "PatchOperationAdd"})
        ET.SubElement(missing, "xpath").text = tank_xpath
        _component(ET.SubElement(ET.SubElement(missing, "value"), "comps"), capacity)
    return root


def activate(version):
    if version not in ("1.5", "1.6", "1.7"):
        raise ValueError("Unsupported game version")
    folder = ROOT / "Aquacultura" / version
    dll = folder / "Assemblies" / "Aquacultura.RimWorld.dll"
    if not dll.is_file() or dll.stat().st_size == 0:
        raise FileNotFoundError("Compile RimWorld DLL for " + version + " before activating")
    target = folder / "Patches" / "TankComponents.xml"
    root = make_patch()
    target.parent.mkdir(parents=True, exist_ok=True)
    ET.indent(root, space="  ")
    ET.ElementTree(root).write(target, encoding="utf-8", xml_declaration=True)
    print("Activated for", version, ":", target)


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--version", choices=("1.5", "1.6", "1.7"), required=True)
    args = parser.parse_args()
    activate(args.version)
