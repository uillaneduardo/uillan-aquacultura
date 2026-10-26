# Teste local do RimWorld (M2-b)

O código das DLLs não pode ser compilado e testado com as bibliotecas originais do jogo nesta execução automatizada. Cada mantenedor deve usar sua instalação autorizada do RimWorld.

## Compilação em um comando

A partir da raiz do repositório, com Python 3 e SDK .NET disponíveis:

```bash
python scripts/build_local.py --version 1.6 --managed "C:/Program Files (x86)/Steam/steamapps/common/RimWorld/RimWorldWin64_Data/Managed"
```

Para 1.5, altere para a instalação daquela versão. Para 1.7, a pasta deve conter `Assembly-CSharp.dll`, `UnityStubs.dll` e `Waylith.dll`, e o SDK .NET 10 deve estar disponível.

O script verifica as dependências, compila, cria o Patch para habilitar o `CompTank` na versão selecionada e gera `dist/Aquacultura-dev-<versão>.zip`. As DLLs originais **nunca** são adicionadas ao arquivo.

## Homologação de jogo

1. Backup dos saves originais.
2. Extrair a pasta `Aquacultura/` do ZIP em `RimWorld/Mods/`.
3. Habilitar em Mods, iniciar uma partida e construir um tanque pequeno.
4. Ativar o modo de desenvolvedor e selecionar o tanque.
5. Clicar `DEV: add 6 tilapia roe`, `DEV: add fish feed`, `DEV: simulate one day`.
6. Observar os indicadores do tanque e avanço de dias, salvar e recarregar, conferir que estado permanece.
7. Repetir em cada versão, anexar `Player.log` e screenshots à issue #2.

**ATENÇÃO:** controles DEV criam ovas/ração lógicos para teste, não debitam recursos da colônia. Portanto não são a implementação da economia nem da reprodução acessível ao jogador comum. Ainda faltam tarefas dos colonos, alimento físico, colheita, controle elétrico e UI final.

**O ZIP local é apenas para validação**, não um release público homologado.
