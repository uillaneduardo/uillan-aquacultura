# M2-a: núcleo biológico independente

Entrega preparatória: classes puras C# para espécies, coortes, tanque e passo diário de simulação. Não há integração com o RimWorld nem release de mod funcional. O projeto net8 é **somente harness de testes**, não deverá ser copiado para os Assemblies do jogo 1.5/1.6.

## Cobertura inicial
- Seis espécies; reprodução por casal, oviposição e incubação.
- Estoque de ração e condições de água/temperatura/oxigenação.
- Limite de capacidade em adultos equivalentes.
- Crescimento agregado em coortes; risco de mortalidade por fome/condições; predação em tanques mistos.
- Aleatoriedade determinística por gerador injetado.
- Testes smoke de casais, sexo único, predação, limites e determinismo.

## Trabalhos antes de integração
- Semântica de reprodução, cuidado parental e consumo físico de alimentos.
- Dividir coortes por idades para evitar distorções no progresso de maturação.
- Preservar/serializar RNG, incluindo save/load no meio de um ciclo.
- Adaptar ao `CompTank` e ao tempo do jogo; versionar estados e criar UI.
- Verificar balanceamento, capacidade e performance com diversos tanques.

CI compila e testa a simulação independentemente das DLLs do RimWorld. A homologação dentro do jogo segue pendente.
