"""Structural tests for versioned RimWorld patch generation (no game DLL required)."""
import tempfile
import unittest
from pathlib import Path
from unittest import mock
from xml.etree import ElementTree as ET

from scripts import activate_runtime


class RuntimeActivationTests(unittest.TestCase):
    def test_generated_patch_handles_existing_and_missing_comps(self):
        root = activate_runtime.make_patch()
        ops = root.findall("Operation")
        self.assertEqual(len(ops), 3)
        for op, (name, capacity) in zip(ops, activate_runtime.CAPACITIES.items()):
            with self.subTest(name=name):
                tank_xpath = "/Defs/ThingDef[defName='" + name + "']"
                self.assertEqual(op.get("Class"), "PatchOperationConditional")
                self.assertEqual(op.findtext("xpath"), tank_xpath + "/comps")
                for branch, expected_xpath, expected_path in (
                    ("match", tank_xpath + "/comps", "value/li"),
                    ("nomatch", tank_xpath, "value/comps/li"),
                ):
                    node = op.find(branch)
                    self.assertIsNotNone(node)
                    self.assertEqual(node.get("Class"), "PatchOperationAdd")
                    self.assertEqual(node.findtext("xpath"), expected_xpath)
                    li = node.find(expected_path)
                    self.assertIsNotNone(li)
                    self.assertEqual(li.get("Class"), activate_runtime.COMP_CLASS)
                    self.assertEqual(li.findtext("capacity"), str(capacity))
                    self.assertEqual(len(node.findall(".//li")), 1)

    def test_activation_requires_compiled_dll(self):
        with tempfile.TemporaryDirectory() as tmp, mock.patch.object(activate_runtime, "ROOT", Path(tmp)):
            with self.assertRaises(FileNotFoundError):
                activate_runtime.activate("1.6")
            self.assertFalse((Path(tmp) / "Aquacultura/1.6/Patches/TankComponents.xml").exists())

    def test_activation_writes_parseable_patch_per_version(self):
        with tempfile.TemporaryDirectory() as tmp, mock.patch.object(activate_runtime, "ROOT", Path(tmp)):
            for version in ("1.5", "1.6", "1.7"):
                with self.subTest(version=version):
                    dll = Path(tmp) / "Aquacultura" / version / "Assemblies/Aquacultura.RimWorld.dll"
                    dll.parent.mkdir(parents=True)
                    dll.write_bytes(b"fixture-only; not a RimWorld DLL")
                    activate_runtime.activate(version)
                    target = dll.parent.parent / "Patches/TankComponents.xml"
                    root = ET.parse(target).getroot()
                    self.assertEqual(root.tag, "Patch")
                    self.assertEqual(len(root.findall("Operation")), 3)


if __name__ == "__main__":
    unittest.main()
