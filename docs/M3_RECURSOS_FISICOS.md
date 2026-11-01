# M3 — Primeiro incremento de recursos físicos

Este incremento define dois **itens físicos** de inventário, com texturas PNG originais e tradução pt-BR:
`AQ_FishFeed` (ração para peixes) e `AQ_TilapiaRoe` (ovas de tilápia).

Os `ThingDef` usam `ResourceBase`, pilhas de 75 unidades e a categoria `Manufactured`.
São recursos básicos para a futura integração de `CompTank.TryAddFeed` e
`CompTank.TryStockEggs` com trabalhos reais de colonos.

**Ainda não há receitas, geração automática, `WorkGiver`, `JobDriver` nem ligação
ao inventário.** Os itens podem ser inspecionados como definições, mas este
incremento **não** torna o ciclo de piscicultura jogável. A existência de um
`ThingDef` não comprova carregamento correto em 1.5/1.6/1.7.

## Próximos passos

1. Implementar aquisição legítima de ração e ovas (produção, compra ou outra fonte balanceada).
2. Implementar `WorkGiver`/`JobDriver` que reserva, transporta e consome exatamente
   a quantidade física antes de confirmar `TryAddFeed`/`TryStockEggs`.
3. Implementar peixe físico e colheita, sem duplicação em cancelamento/falha.
4. Compilar assembly próprio contra DLLs locais do RimWorld e testar em jogo real.
5. Só publicar release após pacote executável, log limpo e testes de save/load.
