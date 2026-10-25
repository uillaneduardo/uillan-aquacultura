# Uillan Aquacultura 🐟

> **Em desenvolvimento · sem release jogável de piscicultura disponível**

Mod para **RimWorld** que pretende oferecer tanques de criação de peixes, reprodução por casal, ovas, alimentação, predação, equipamentos de tratamento e carne de peixe.

## Estado atual — M1-a

Os três tanques estão definidos por XML, com arte PNG própria, tradução PT-BR e custos de construção: pequeno 2×2, médio 3×4, grande 5×5. Eles aparecem no menu **Arquiteto > Produção** conforme as definições, mas precisam ser testados dentro de cada versão do jogo. **Ainda são edifícios decorativos; não criam peixes nem geram alimento.**

| Sistema | Estado |
| --- | --- |
| Repositório, testes e pipeline de ZIP | Implementado |
| Definições e texturas de 3 tanques | Implementado, aguardando teste no jogo |
| Código-fonte da persistência | Preparado, não compilado/integrado |
| Reprodução, predação, alimentação | Planejado |
| Tratamento elétrico, colheita, culinária | Planejado |
| Painel de gerenciamento | Planejado |

## Desenvolvimento

- [Roadmap / issues](https://github.com/uillaneduardo/uillan-aquacultura/issues)
- [Releases](https://github.com/uillaneduardo/uillan-aquacultura/releases)
- [CI e artefatos de revisão](https://github.com/uillaneduardo/uillan-aquacultura/actions)
- [Escopo](docs/ESCOPO.md) · [M1](docs/M1_TANQUES.md) · [Arquitetura](docs/ARQUITETURA.md) · [Matriz de testes](docs/TESTES.md) · [Distribuição](docs/RELEASES.md)

## Instalação experimental a partir do artefato CI

O workflow empacota somente `Aquacultura/`. Extraia esta pasta para `RimWorld/Mods/` (evite pastas aninhadas), ative em Mods e reinicie. **O artefato Actions não é um release nem uma certificação de compatibilidade.**

### Compatibilidade
Suporte almejado: 1.5, 1.6 e 1.7. Testes in-game e com saves anteriores estão pendentes. Faça backup antes de experimentar.

## Organização
`Aquacultura/`: mod instalável. `Source/`: C# a compilar com DLLs próprias do jogo. `docs/`: projeto e testes. `tests/`: validações. `scripts/`: empacotamento.

A licença de distribuição e os créditos das artes serão definidos antes do primeiro release público; não enviar conteúdo proprietário do RimWorld.