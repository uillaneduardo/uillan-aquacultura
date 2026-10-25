# Uillan Aquacultura 🐟

> **Em desenvolvimento · sem release jogável no momento**

Mod de piscicultura e aquaponia para **RimWorld**, com objetivo de compatibilidade com as versões **1.5, 1.6 e 1.7** e inclusão em saves existentes.

## Visão geral

O projeto permitirá construir tanques pequenos (2×2), médios (3×4) e grandes (5×5) em **Produção**; introduzir ovas e reprodutores de espécies diferentes; simular crescimento, alimentação, reprodução sexuada e predação; usar equipamento elétrico de tratamento da água; colher peixes adultos para a cozinha; e acompanhar a população em uma janela de gerenciamento.

**Planejado, não implementado:** os recursos descritos acima ainda não estão disponíveis para jogar.

## Desenvolvimento público

- [Issues e roadmap](https://github.com/uillaneduardo/uillan-aquacultura/issues)
- [Pull requests](https://github.com/uillaneduardo/uillan-aquacultura/pulls)
- [Builds e validações](https://github.com/uillaneduardo/uillan-aquacultura/actions)
- [Releases e downloads](https://github.com/uillaneduardo/uillan-aquacultura/releases)
- [Especificação](docs/ESCOPO.md) · [Arquitetura](docs/ARQUITETURA.md) · [Testes](docs/TESTES.md) · [Distribuição](docs/RELEASES.md)

## Estrutura

`Aquacultura/` é a pasta do mod; `Source/` reservará C#; `docs/` contém especificações; `scripts/package_mod.py` gera o ZIP que preserva a estrutura `Aquacultura/About/About.xml`.

## Versões e saves

A declaração de suporte às versões 1.5, 1.6 e 1.7 em `About.xml` representa a **meta de compatibilidade**, não uma homologação. A compatibilidade com saves antigos será testada antes do primeiro pré-release. Faça backup dos saves antes de testar qualquer versão experimental.

## Licença e artes

Licença de distribuição e créditos das artes serão definidos antes de receber contribuições externas ou distribuir os primeiros assets. Não incluir conteúdo proprietário do RimWorld no repositório.
