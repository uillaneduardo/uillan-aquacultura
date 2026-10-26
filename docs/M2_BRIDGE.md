# M2-b — Integração do simulador aos tanques (código-fonte)

O `CompTank` recebe ticks raros (250 ticks), simula um dia após 60.000 ticks e armazena o progresso de forma independente da renderização. Ele converte `TankCohort` (Scribe) para `FishCohort` (motor) e vice-versa, mantendo o estado do gerador de números aleatórios para reprodução determinística após salvar/recarregar.

## APIs internas

- `TryStockEggs(speciesId, quantity)`: requer entrada válida, respeita monocultura e limite de ovas.
- `AddFeed(quantity)`: adiciona suprimento lógico (a integração deverá consumir ração real).
- `SetTreatmentPowered(bool)`: futuro equipamento elétrico controlará esta condição.
- `TryHarvestAdults(speciesId, quantity)`: diminui adultos preservando casais; **não cria item**. O futuro trabalho deve realizar a transação de colheita para impedir duplicação.

Os métodos ainda não aparecem ao jogador. Eles não devem permitir criação gratuita de recursos.

## Teste

`dotnet run --project tests/BridgeSmoke/BridgeSmoke.csproj -c Release` usa dublês controlados de `Verse` para compilar e exercitar transições, save/load simulado, sementes aleatórias e ticks. Não substitui teste com a engine real.

## Compilação real e ativação controlada

1. Compilar `Source/Aquacultura.RimWorld/Aquacultura.RimWorld.csproj` com `GameManagedDir` da versão, conforme README do projeto.
2. Conferir `Aquacultura/<versão>/Assemblies/Aquacultura.RimWorld.dll`.
3. Executar `python3 scripts/activate_runtime.py --version <versão>`.
4. Somente então a `PatchOperationAdd` é gravada em `Aquacultura/<versão>/Patches/`, permitindo que a DLL seja exigida em runtime.
5. Validar no jogo: construções, log, consumo, contador de dias, save/load e múltiplos tanques.

**Não realizar release** enquanto os builds e a homologação 1.5/1.6/1.7 estiverem pendentes. O CI de hoje verifica os dublês, não uma compilação legítima do jogo.
