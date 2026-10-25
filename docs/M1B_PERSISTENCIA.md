# M1-b — estado persistente e compilação por versão

## Implementado neste PR
- Projeto MSBuild/C# direcionável: 1.5 e 1.6 `net472`; 1.7 `net10.0`.
- Referências locais obrigatórias a DLLs do próprio jogo; nunca incluir DLLs de RimWorld no repositório.
- Estado de cada tanque versionado: água, políticas e coortes de peixes, com sexo, estágio e progresso médio.
- Serialização via `Scribe_Values` e `Scribe_Collections` e normalização pós-carregamento; IDs de campo `aq*` estáveis.

## Limites
- Ainda não foi compilado nem verificado em executáveis de RimWorld, pois bibliotecas originais do jogo não estão disponíveis nesta execução.
- Por isso, os Defs continuam sem referência a `CompTank` para evitar erro de carga.
- O script de CI de hoje só valida o XML/estrutura e roda testes estáticos; **não valida runtime**.
- Reprodução, alimentação, predação e gerenciamento são partes futuras da M2/M4.
- Após build local por versão: editar Defs para definir `comps` e capacidade por tanque, validar save preexistente, instalar/remover em testes isolados, carregar e recarregar.

## Critérios de aceite restantes
1. Compilação limpa de cada target com DLLs autorizadas.
2. Log sem erros vermelhos em cada versão.
3. Save antigo abre com mod ativo e sem tanques pré-existentes.
4. Save com os três tanques preserva dados após salvar e carregar.
5. Retornar da 1.7 para 1.6 conforme cenário de compatibilidade do jogo, sem presumir que o mod preserve estado sem testes.

## Referências
- https://ludeon.com/blog/2026/10/announcing-the-waylith-engine-and-update-1-7/
- https://rimworldwiki.com/wiki/Modding_Tutorials/RimWorld_1.7_Mod_Updates
