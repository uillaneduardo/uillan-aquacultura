# Política de releases a partir do PR #11

A pipeline de desenvolvimento cria ZIPs de **inspeção** mesmo quando o jogo ainda não é funcional. O arquivo gerado em Actions NÃO é um release.

O job `prerelease` agora exige `python3 scripts/package_mod.py --validate-only --release-check`. Esse comando barra tags `vX.Y.Z-rc.N` se faltarem as três DLLs específicas de 1.5, 1.6 e 1.7 ou patches que liguem o CompTank aos tanques.

Cada DLL deve ser compilada com referências autorizadas da instalação do jogo; em seguida é gerado `Aquacultura/<versão>/Patches/TankComponents.xml` com `python3 scripts/activate_runtime.py --version <versão>`. O script confere presença, tamanho e assinatura PE dos assemblies, **não atesta que executam dentro do jogo**.

Para um release real também são indispensáveis: logs do jogo sem erros, testes de construção, save/load, alimentação e colheita, e link para a matriz de homologação. Atualmente não estão concluídos.

**Distribuição**: o repositório ignora DLLs construídas localmente. Logo, a pipeline acionada apenas por tag ainda não tem como obter DLLs legítimas: para publicar, será preciso introduzir uma etapa segura de upload de artefatos compilados/homologados (sem DLLs proprietárias do jogo) ou um processo manual de release revisado. Nenhuma publicação será anunciada sem isso.
