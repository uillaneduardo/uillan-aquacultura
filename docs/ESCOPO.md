# Escopo do mod — v0.1

**Estado:** planejamento. Nenhum sistema de criacao de peixes foi implementado.

## Objetivo

Criacao de peixes em tanques de producao, usando ovas, reproducao entre macho e femea da mesma especie, crescimento por estagios, predacao, alimentacao, qualidade da agua, equipamentos eletricos e colheita para culinaria.

## Edificios (menu Producao)

- Tanque pequeno: 2x2, 8 unidades adultas equivalentes (valores a balancear).
- Tanque medio: 3x4, 24 unidades adultas equivalentes.
- Tanque grande: 5x5, 60 unidades adultas equivalentes.
- Unidade de tratamento aquicola: eletrica; melhora filtragem, oxigenacao e estabilidade ambiental; consumo inicial proposto 150 W (a balancear).

## Especies iniciais propostas

Tilapia, carpa, bagre, truta, tambaqui, piranha. Cada uma com parametros de alimentacao, taxa reprodutiva, temperatura ideal, tolerancia, taxa de crescimento, mortalidade e carne produzida.

## Simulacao

Ova -> alevino -> juvenil -> adulto. A reproducao necessita ao menos um macho e uma femea adultos da mesma especie, alem de espaco, alimento e agua adequados. Ovas iniciais podem resultar em ambos os sexos. Envelhecimento e mortalidade natural; peixes carnivoros podem predar outros peixes menores, inclusive da propria especie quando aplicavel. Espaco calculado em adultos equivalentes, nao mera contagem de individuos.

## Produção alimentar

Colheita manual e automatica; configuracao para preservar casais reprodutores; peixe inteiro, processamento para carne de peixe e uso em receitas do jogo; receitas adicionais opcionais. Evitar duplicacao de recursos na colheita ou carregamento de save.

## Interface

Janela de gestao por tanque: populacao por especie, sexo e estagio; barras de crescimento e qualidade ambiental; alertas de predacao, superlotacao e energia; politicas de colheita e mistura de especies.

## Fora do MVP

Cultivo vegetal usando nutrientes da agua (aquaponia vegetal); cadeias industriais; peixes individuais como Pawns; DLC obrigatoria.

## Pendencias de decisao

- Representacao visual final (preferencia proposta: peixes decorativos animados, populacao simulada internamente).
- Balanceamento de tempos, dieta, mortalidade e producao.
- Integracao com biomas/temperatura e acesso a agua.
