# Releases e distribuição

## Canais

- Issues: plano, bugs e funcionalidades.
- Pull requests: revisão e integração.
- Actions: validação estrutural e geração de artefato ZIP para inspeção (não é release).
- Releases: distribuição pública oficial por tags `vX.Y.Z-rc.N` (prévia) e `vX.Y.Z` (estável).

## Política

**Não gerar uma tag de release durante o scaffold.** Uma pasta de metadados sem mecânicas não constitui versão de teste do jogo. O workflow de distribuição de pré-release exige tag `-rc.`, valida XML e cria um pacote ZIP com somente `Aquacultura/`. Uma versão estável exigirá etapa própria de promoção com homologação comprovada.

## Instalação futura

1. Baixar ZIP da aba Releases.
2. Extrair a pasta `Aquacultura/` no diretório `RimWorld/Mods/` (evitar duas camadas aninhadas).
3. Ativar o mod no menu Mods e reiniciar o jogo.
4. Fazer cópia dos saves antes de testar em partidas existentes.

## Restrições

A verificação automática atual valida XML, estrutura e ZIP, **não abre o RimWorld** nem confirma DLLs ou funcionamento em 1.5/1.6/1.7. Para isso, usar testes manuais/documentados e mais tarde uma pipeline de compilação quando as dependências forem estabelecidas.
