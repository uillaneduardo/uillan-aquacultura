# M1 — Tanques de produção

## M1-a: construções XML independentes de DLL
- 3 tanques no menu **Arquiteto > Produção**: pequeno 2×2, médio 3×4, grande 5×5.
- Custos de aço: 50 / 130 / 320; componentes industriais: 0 / 1 / 3.
- Imagens originais em PNG com transparência e nomenclatura estável.
- IDs imutáveis: `AQ_TankSmall`, `AQ_TankMedium`, `AQ_TankLarge`.
- Localização em português brasileiro via `DefInjected`.
- O jogo pode salvar edifícios vanilla sem DLLs. **A capacidade de peixes é apenas planejada e ainda não é aplicada.** Não há reprodução, peixes vivos, colheita ou tratamento de água nesta entrega.

## M1-b: camada persistente (preparada, não integrada)
Em `Source/Aquacultura.RimWorld` há os arquivos-fonte `CompTank`, `TankCohort` e `CompProperties_Tank`. A integração XML será feita **somente depois** de compilar com bibliotecas legítimas de cada versão e confirmar que o jogo carrega o assembly. Não incluir DLL proprietária no GitHub.

## Validação manual pendente
1. Ative a pasta `Aquacultura` em um jogo novo e abra **Produção**.
2. Construa cada tamanho; confira textura, custo, colisão e posição.
3. Salve e recarregue. Repita em um save criado antes de ativar o mod (com backup).
4. Repetir em RimWorld 1.5, 1.6, 1.7; anexar Player.log e screenshots à issue #2.
5. Registrar eventuais erros vermelhos, perda de conteúdo ou conflitos com mods.

## Critérios de conclusão M1
- [x] XML dos três edifícios
- [x] PNGs originais
- [x] PT-BR + labels inglesas
- [x] Fonte de persistência
- [ ] Compilação real da DLL e integração XML do ThingComp
- [ ] Save/load de dados biológicos e testes dentro das três versões

A versão 1.7 estava em beta instável em 25/out/2026.