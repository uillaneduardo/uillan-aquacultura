import unittest
import struct
from pathlib import Path
from xml.etree import ElementTree as ET

MOD = Path(__file__).resolve().parents[1] / "Aquacultura"
DEFS = MOD / "Defs/ThingDefs/Tanks.xml"
EXPECTED = {"AQ_TankSmall": ((2,2),50),
            "AQ_TankMedium": ((3,4),130),
            "AQ_TankLarge": ((5,5),320)}

def dimensions(raw):
    return tuple(int(v.strip()) for v in raw.strip("()").split(","))

class TankDefinitions(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.defs = ET.parse(DEFS).getroot()
        cls.tanks = {d.findtext("defName"): d for d in cls.defs.findall("ThingDef") if d.findtext("defName")}

    def test_exactly_three_tanks(self):
        self.assertEqual(set(self.tanks), set(EXPECTED))

    def test_category_and_vanilla_base(self):
        base = next(d for d in self.defs.findall("ThingDef") if d.get("Name") == "AQ_TankBase")
        self.assertEqual(base.get("Abstract"), "True")
        self.assertEqual(base.get("ParentName"), "BuildingBase")
        self.assertEqual(base.findtext("designationCategory"), "Production")
        self.assertEqual(base.findtext("thingClass"), "Building")

    def test_material_dimensions_and_graphics(self):
        for name,(size,cost) in EXPECTED.items():
            with self.subTest(name=name):
                d=self.tanks[name]
                self.assertEqual(d.get("ParentName"),"AQ_TankBase")
                self.assertEqual(dimensions(d.findtext("size")),size)
                self.assertEqual(dimensions(d.findtext("graphicData/drawSize")),size)
                self.assertEqual(int(d.findtext("costList/Steel")),cost)
                tex=MOD/"Textures"/(d.findtext("graphicData/texPath")+".png")
                self.assertTrue(tex.is_file(),tex)
                data=tex.read_bytes()
                self.assertEqual(data[:8],b"\x89PNG\r\n\x1a\n")
                self.assertEqual(data[12:16],b"IHDR")
                width,height=struct.unpack(">II",data[16:24])
                self.assertEqual(data[25],3) # indexed PNG; transparency palette
                self.assertEqual(width*size[1],height*size[0])

    def test_no_missing_compiled_classes(self):
        self.assertIsNone(self.defs.find(".//comps"))

    def test_pt_br_translations(self):
        data=ET.parse(MOD/"Languages/PortugueseBrazilian/DefInjected/ThingDef/Tanks.xml").getroot()
        for name in EXPECTED:
            self.assertIsNotNone(data.find(name+".label"))
            self.assertIsNotNone(data.find(name+".description"))

if __name__=="__main__":
    unittest.main()
