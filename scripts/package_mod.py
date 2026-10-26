#!/usr/bin/env python3
"""Valida e empacota apenas a pasta instalável do mod."""
import argparse
from pathlib import Path
import sys
import zipfile
from xml.etree import ElementTree as ET

ROOT = Path(__file__).resolve().parents[1]
MOD = ROOT / "Aquacultura"

def validate():
    metadata = MOD / "About" / "About.xml"
    if not metadata.is_file():
        raise ValueError("Arquivo About/About.xml ausente")
    root = ET.parse(metadata).getroot()
    if root.tag != "ModMetaData":
        raise ValueError("Raiz XML inválida em About.xml")
    for key in ("name", "author", "packageId", "supportedVersions"):
        if root.find(key) is None:
            raise ValueError("Campo obrigatório ausente: " + key)
    versions = {el.text for el in root.findall("./supportedVersions/li")}
    if not {"1.5", "1.6", "1.7"}.issubset(versions):
        raise ValueError("supportedVersions não inclui 1.5, 1.6 e 1.7")
    for file in MOD.rglob("*.xml"):
        ET.parse(file)
    for forbidden in ("Source", "docs", ".github", "scripts"):
        if (MOD / forbidden).exists():
            raise ValueError("Diretório de desenvolvimento na pasta instalável: " + forbidden)
    print("Metadados e XML bem formados; versões declaradas:", ", ".join(sorted(versions)))

def check_release_runtime():
    """Prevent publishing XML-only scaffolding as a playable fish farming release."""
    missing = []
    for version in ("1.5", "1.6", "1.7"):
        base = MOD / version
        dll = base / "Assemblies" / "Aquacultura.RimWorld.dll"
        patch = base / "Patches" / "TankComponents.xml"
        if not dll.is_file() or dll.stat().st_size < 1024:
            missing.append(str(dll.relative_to(ROOT)))
        elif dll.read_bytes()[:2] != b"MZ":
            missing.append(str(dll.relative_to(ROOT)) + " (invalid PE header)")
        if not patch.is_file():
            missing.append(str(patch.relative_to(ROOT)))
        else:
            root = ET.parse(patch).getroot()
            if root.tag != "Patch" or len(root.findall("Operation")) != 3:
                missing.append(str(patch.relative_to(ROOT)) + " (unexpected operations)")
    if missing:
        raise ValueError("Release blocked, missing runtime artifacts:\\n" +
                         "\\n".join(missing))

def package(destination):
    destination.parent.mkdir(parents=True, exist_ok=True)
    with zipfile.ZipFile(destination, "w", compression=zipfile.ZIP_DEFLATED) as archive:
        for file in sorted(MOD.rglob("*")):
            if file.is_file() and file.name != ".gitkeep":
                archive.write(file, Path("Aquacultura") / file.relative_to(MOD))
    with zipfile.ZipFile(destination) as archive:
        bad_file = archive.testzip()
        if bad_file:
            raise ValueError("Arquivo corrompido no ZIP: " + bad_file)
        if "Aquacultura/About/About.xml" not in archive.namelist():
            raise ValueError("Metadados não encontrados no ZIP")
    print("Pacote criado:", destination)

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--validate-only", action="store_true")
    parser.add_argument("--release-check", action="store_true")
    parser.add_argument("--output", type=Path, default=ROOT / "dist" / "Uillan-Aquacultura.zip")
    args = parser.parse_args()
    try:
        validate()
        if args.release_check:
            check_release_runtime()
        if not args.validate_only:
            package(args.output)
    except (ValueError, ET.ParseError, OSError, zipfile.BadZipFile) as exc:
        print("ERRO:", exc, file=sys.stderr)
        sys.exit(1)

if __name__ == "__main__":
    main()
