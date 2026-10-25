# Arquitetura tecnica

## Empacotamento

A pasta instalavel e `Aquacultura/`. `About/About.xml` fornece nome, packageId estavel e lista das versoes pretendidas. `Defs/`, `Languages/`, `Textures/` armazenam conteudo compartilhado. Diretorios `1.5/Assemblies`, `1.6/Assemblies` e `1.7/Assemblies` sao reservados para assemblies especificos. Nenhuma DLL esta presente neste momento.

## Componentes planejados

- `TankState`: id estavel, especies, coortes/individuos logicos com sexo/idade/crescimento, recursos internos, politica de colheita.
- `TankSimulation`: ticks discretizados, reproducao, alimentacao, crescimento, predacao, degradacao/recuperacao de agua, mortalidade, determinismo.
- `TankBuildingComponent`: adaptador para edificio do jogo, comandos, notificacoes e ciclo de vida.
- `WaterTreatmentComponent`: estado de energia, operacao, manutencao e bonus limitados.
- `TankManagementWindow`: interface de leitura e comandos; dados nunca devem depender da janela permanecer aberta.
- `SaveAdapter`: serializacao e migracao versionada de estado, inicializando dados ausentes em saves antigos.
- `HarvestWorkGiver`: trabalho dos colonos para retirar peixes e gerar itens apenas apos colheita confirmada.

## Compatibilidade

- DefNames, packageId e chaves salvas nao devem ser renomeados sem migracao explicita.
- Adicionar o mod a saves preexistentes nao deve pressupor que ja existam tanques.
- Testar jogos novos e saves existentes em 1.5, 1.6 e 1.7 separadamente.
- Nao prometer suporte a remocao do mod de saves que ja contenham seus itens.
- Assemblies por versao deverao ser compilados/testados com referencias locais licenciadas do jogo. Nao subir DLLs do RimWorld ao repositorio.
- O suporte 1.7 e pretendido, ainda nao homologado.

## Desempenho

Nao instanciar um Pawn por peixe. Processamento periodico por tanque, com atualizacao por coortes, custo previsivel e renderizacao visual desacoplada. Evitar randomizacao dependente de frame rate, duplicacao de colheita e crescimento ao carregar partida.
