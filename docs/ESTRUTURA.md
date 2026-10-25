# Estrutura do repositorio

```text
uillan-aquacultura/
  README.md
  Aquacultura/                  # Copiar esta pasta para RimWorld/Mods/
    About/About.xml             # Metadata e versoes
    Defs/                       # ThingDefs, RecipeDefs, ResearchProjectDefs, ThingCategoryDefs, WorkGiverDefs
    Languages/                  # English e PortugueseBrazilian
    Textures/                   # Buildings, Fish e UI
    1.5/Assemblies/             # DLL da versao 1.5 (futura)
    1.6/Assemblies/             # DLL da versao 1.6 (futura)
    1.7/Assemblies/             # DLL da versao 1.7 (futura)
  Source/                       # Fontes C# futuros
  docs/                         # Escopo, arquitetura e matriz de teste
```

Diretorios vazios estao registrados por `.gitkeep`. A pasta do mod pode ser reconhecida como pacote de metadados, mas **nao oferece mecanicas jogaveis** ate a implementacao de Defs, codigo e arte.
