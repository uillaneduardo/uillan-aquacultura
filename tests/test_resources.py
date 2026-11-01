"""Static contract for physical fish resources (not an in-game integration test)."""
import struct
import unittest
from pathlib import Path
from xml.etree import ElementTree as ET

MOD = Path(__file__).resolve().parents[1] / "Aquacultura"
DEFS = MOD / "Defs/ThingDefs/FishResources.xml"
TRANSLATIONS = MOD / "Languages/PortugueseBrazilian/DefInjected/ThingDef/FishResources.xml"
EXPECTED = {"AQ_FishFeed": "Fish/FishFeed", "AQ_TilapiaRoe": "Fish/TilapiaRoe"}


class FishResourceDefinitions(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        root = ET.parse(DEFS).getroot()
        cls.items = {d.findtext("defName"): d for d in root.findall("ThingDef")}
        cls.translations = ET.parse(TRANSLATIONS).getroot()

    def test_only_explicit_physical_items(self):
        self.assertEqual(set(self.items), set(EXPECTED))
        for name, definition in self.items.items():
            with self.subTest(name=name):
                self.assertEqual(definition.get("ParentName"), "ResourceBase")
                self.assertEqual(definition.findtext("graphicData/graphicClass"), "Graphic_StackCount")
                self.assertEqual(definition.findtext("thingCategories/li"), "Manufactured")
                self.assertGreater(int(definition.findtext("stackLimit")), 1)
                self.assertGreater(float(definition.findtext("statBases/Mass")), 0)
                self.assertGreater(float(definition.findtext("statBases/MarketValue")), 0)
                self.assertIsNone(definition.find("comps"))

    def test_original_textures_are_valid_rgba_pngs(self):
        for name, texture in EXPECTED.items():
            with self.subTest(name=name):
                self.assertEqual(self.items[name].findtext("graphicData/texPath"), texture)
                path = MOD / "Textures" / (texture + ".png")
                self.assertTrue(path.is_file(), path)
                data = path.read_bytes()
                self.assertEqual(data[:8], b"\x89PNG\r\n\x1a\n")
                self.assertEqual(data[12:16], b"IHDR")
                self.assertEqual(struct.unpack(">II", data[16:24]), (32, 32))
                self.assertEqual(data[25], 6)  # RGBA; no external art dependency

    def test_portuguese_labels_and_descriptions(self):
        for name in EXPECTED:
            with self.subTest(name=name):
                self.assertTrue(self.translations.findtext(name + ".label"))
                self.assertTrue(self.translations.findtext(name + ".description"))


if __name__ == "__main__":
    unittest.main()
