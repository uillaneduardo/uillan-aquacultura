#!/usr/bin/env python3
"""Build a local development copy against DLLs from the user's RimWorld installation.

No proprietary binaries are uploaded. This script does not claim in-game compatibility.
"""
import argparse
from pathlib import Path
import subprocess
import sys
from xml.etree import ElementTree as ET

ROOT = Path(__file__).resolve().parents[1]

def build(version, managed, output):
    managed = managed.expanduser().resolve()
    if not managed.is_dir():
        raise FileNotFoundError("GameManagedDir does not exist: " + str(managed))
    required = ["Assembly-CSharp.dll"]
    if version == "1.7":
        required += ["UnityStubs.dll", "Waylith.dll"]
    missing = [str(managed / f) for f in required if not (managed / f).is_file()]
    if missing:
        raise FileNotFoundError("Missing RimWorld dependencies: " + ", ".join(missing))
    project = ROOT / "Source" / "Aquacultura.RimWorld" / "Aquacultura.RimWorld.csproj"
    args = ["dotnet", "build", str(project), "--configuration", "Release",
            "-p:RimWorldVersion=" + version, "-p:GameManagedDir=" + str(managed)]
    subprocess.run(args, check=True, cwd=ROOT)
    dll = ROOT / "Aquacultura" / version / "Assemblies" / "Aquacultura.RimWorld.dll"
    if not dll.is_file() or dll.stat().st_size == 0:
        raise RuntimeError("Build completed but output assembly missing: " + str(dll))
    subprocess.run([sys.executable, str(ROOT / "scripts" / "activate_runtime.py"), "--version", version],
                   check=True, cwd=ROOT)
    patch = ROOT / "Aquacultura" / version / "Patches" / "TankComponents.xml"
    if ET.parse(patch).getroot().tag != "Patch":
        raise RuntimeError("Generated patch is malformed: " + str(patch))
    if output is None:
        output = ROOT / "dist" / ("Aquacultura-dev-" + version + ".zip")
    subprocess.run([sys.executable, str(ROOT / "scripts" / "package_mod.py"), "--output", str(output)],
                   check=True, cwd=ROOT)
    print("Development package generated:", output)
    print("NOT HOMOLOGATED: verify red errors in log, fish tick and save/load in-game.")

if __name__ == "__main__":
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument("--version", choices=("1.5", "1.6", "1.7"), required=True)
    p.add_argument("--managed", type=Path, required=True,
                   help="Folder with Assembly-CSharp.dll from YOUR game installation")
    p.add_argument("--output", type=Path, default=None)
    args=p.parse_args()
    try:
        build(args.version, args.managed, args.output)
    except (FileNotFoundError, RuntimeError, subprocess.CalledProcessError) as exc:
        print("ERROR:", exc, file=sys.stderr)
        sys.exit(1)
