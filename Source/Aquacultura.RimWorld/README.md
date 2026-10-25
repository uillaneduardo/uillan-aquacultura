# C# para RimWorld

O projeto suporta build por versão usando bibliotecas **da instalação local do jogo**. Não redistribua Assembly-CSharp.dll ou outras DLLs do jogo.

## Compilar

Exemplos (SDK .NET instalado e referências locais):

```bash
dotnet build Source/Aquacultura.RimWorld/Aquacultura.RimWorld.csproj -c Release -p:RimWorldVersion=1.6 -p:GameManagedDir="/path/to/RimWorld_1.6/RimWorldWin64_Data/Managed"
dotnet build Source/Aquacultura.RimWorld/Aquacultura.RimWorld.csproj -c Release -p:RimWorldVersion=1.5 -p:GameManagedDir="/path/to/RimWorld_1.5/RimWorldWin64_Data/Managed"
dotnet build Source/Aquacultura.RimWorld/Aquacultura.RimWorld.csproj -c Release -p:RimWorldVersion=1.7 -p:GameManagedDir="/path/to/RimWorld_1.7/Managed"
```

Para 1.7 informe o **diretório real** contendo `Assembly-CSharp.dll`, `UnityStubs.dll` e `Waylith.dll` (o caminho varia por instalação). Requer SDK .NET 10, enquanto 1.5/1.6 usam o target `net472` e podem requerer os reference assemblies de .NET Framework em sistemas não Windows.

Os artefatos deverão ficar em `Aquacultura/<versão>/Assemblies/`. **Não habilite o componente `CompTank` em defs genéricas antes que as três DLLs estejam construídas e testadas.** A integração está condicionada à verificação em jogo e será entregue por PR separado.

Refs: https://rimworldwiki.com/wiki/Modding_Tutorials/RimWorld_1.7_Mod_Updates
