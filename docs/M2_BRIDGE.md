# M2-b — Integração do simulador aos tanques (código-fonte)

O `CompTank` recebe ticks raros (250 ticks), simula um dia após 60.000 ticks e armazena o progresso de forma independente da renderização. Ele converte `TankCohort` (Scribe) para `FishCohort` (motor) e vice-versa, mantendo o estado do gerador de números aleatórios para reprodução determinística após salvar/recarregar.

## APIs internas

- `TryStockEggs(speciesId, quantity, consumeItems)`: valida espécie, monocultura e limite global **antes** de pedir consumo. O callback deve confirmar que exatamente `quantity` ovas físicas foram consumidas; falha/ausência não altera o tanque.
- `TryAddFeed(units, consumeItems)`: aceita apenas unidades inteiras positivas, respeita o teto lógico de 100.000 e só aumenta o estoque após confirmação de consumo de exatamente `units` itens físicos. Por enquanto, **1 unidade física = 1 unidade lógica**; a definição de ração real e o balanceamento ainda estão pendentes.
- `SetTreatmentPowered(bool)`: futuro equipamento elétrico controlará esta condição.
- `TryHarvestAdults(speciesId, quantity, deliverItems)`: preserva casais reprodutores e **só remove os adultos após** `deliverItems(count)` confirmar a entrega de exatamente `count` itens físicos. Se a entrega falhar ou o callback não existir, o estoque biológico não muda. Ainda falta implementar `WorkGiver`/`JobDriver`, `ThingDef` e posicionamento real dos itens; o callback não deve ser utilizado para simular geração gratuita.

Os métodos ainda não aparecem ao jogador. Os gizmos de adição gratuita são restritos a `Prefs.DevMode` e passam callbacks fictícios apenas para testes. Não existe mais método público de adicionar ração ou ovas sem callback. **O callback não é uma garantia de consumo real**: cabe ao futuro `JobDriver` reservar e consumir os itens, confirmar quantidades e tratar falha/cancelamento sem duplicação. A confirmação do callback não substitui a transação real no mapa.

## Teste

`dotnet run --project tests/BridgeSmoke/BridgeSmoke.csproj -c Release` usa dublês controlados de `Verse` para compilar e exercitar transições, save/load simulado, sementes aleatórias e ticks. Não substitui teste com a engine real.

## Compilação real e ativação controlada

1. Compilar `Source/Aquacultura.RimWorld/Aquacultura.RimWorld.csproj` com `GameManagedDir` da versão, conforme README do projeto.
2. Conferir `Aquacultura/<versão>/Assemblies/Aquacultura.RimWorld.dll`.
3. Executar `python3 scripts/activate_runtime.py --version <versão>`.
4. Somente então o patch condicional (`PatchOperationConditional`) é gravado em `Aquacultura/<versão>/Patches/`, permitindo que a DLL seja exigida em runtime.
5. Validar no jogo: construções, log, consumo, contador de dias, save/load e múltiplos tanques.

**Não realizar release** enquanto os builds e a homologação 1.5/1.6/1.7 estiverem pendentes. O CI de hoje verifica os dublês, não uma compilação legítima do jogo.
