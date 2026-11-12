# Registro de uso de IA

Nível permitido na atividade: **IA-2 (automação guiada)**: a IA automatiza partes da tarefa, com supervisão humana contínua.

Declaração de método: *a IA auxiliou na geração inicial de código e de diagramas, com revisão humana completa.*

## Ferramentas usadas

| Ferramenta | Para quê |
|---|---|
| Claude (Anthropic) | Planejamento da modelagem, rascunho de diagramas e esqueleto das classes Model |
| _(preencher)_ | |

## Registro de uso

Cada linha registra um uso. Preencher **sempre** as colunas "Revisão humana" e "Responsável".

| # | Data | Integrante | Ferramenta | Prompt (resumo) | O que a IA gerou | Revisão humana / o que mudamos | Status |
|---|---|---|---|---|---|---|---|
| 1 | 06/11/2026 | Arthur | Claude | Envio do PDF da atividade + "Pegamos o sistema de eventos" | Resumo dos requisitos e pontos a ajustar no modelo do professor | _(preencher)_ | Revisado |
| 2 | 06/11/2026 | Arthur | Claude | "Queria começar reformulando o diagrama de classes" | Rascunho do diagrama de classes com Local, Patrocinador, Pagamento e CheckIn | _(preencher)_ | Revisado |
| 3 | 06/11/2026 | Arthur | Claude | "Acho melhor palestrante se tornar pessoa também" | Versão com Participante e Palestrante herdando de Pessoa | Decisão do grupo: Palestrante como Pessoa | Revisado |
| 4 | 06/11/2026 | Arthur | Claude | "Pode continuar" (casos de uso) | Rascunho do diagrama de casos de uso (3 atores, 14 casos) | _(preencher)_ | Em revisão |
| 5 | 06/11/2026 | Arthur | Claude | Envio da imagem do diagrama + "vc consegue editar? já colocando os atributos" | Arquivo .drawio com atributos, multiplicidades e notação UML | Grupo removeu Patrocinador e CheckIn; ajustou layout | Revisado |
| 6 | 06/11/2026 | Arthur | Claude | "Diagrama de ações também" | Arquivo .drawio de casos de uso | _(preencher)_ | Em revisão |
| 7 | 06/11/2026 | Arthur | Claude | "Preciso normalizar algo para o banco de dados?" | Orientação sobre TPH x TPT, tabelas de junção N:N, 1:1 com índice único | _(preencher qual estratégia o grupo escolheu)_ | Em revisão |
| 8 | 10/11/2026 | Arthur | Claude | Envio do .drawio + link do repositório + "Preciso iniciar colocando os atributos das classes" | Classes da pasta Model com atributos, FKs e navegação; renomeou Empresa → PessoaJuridica e lPagamento/lCertificado → IPagamento/ICertificado | _(preencher)_ | Em revisão |
| 9 | 10/11/2026 | Arthur | Claude | "Pode deixar 1x1 em pagamento e inscrição... deixe o controller como PessoaJuridica" | Classe Pagamento (1:1 com Inscricao), Pagamento no .drawio, renomeou EmpresaController e criou este arquivo | _(preencher)_ | Em revisão |
| 10 | 10/11/2026 | Arthur | Claude | "Sim" (criar o Db.cs com DbContext e configurações do EF) | `Db.cs` (DbContext com SQL Server LocalDB), classes de configuração em `Database/Configurations` (herança TPT, N:N EventoPalestrante, 1:1 Inscricao-Pagamento, índices únicos, OnDelete) e pacotes EF Core no .csproj | _(preencher)_ | Em revisão |
| 11 | | | | | | | |

## Comparação com e sem IA (obrigatória)

Escolher **um trecho** do sistema e mostrar duas versões.

**Trecho escolhido:** _(ex.: método `Inscricao.Pagar()` ou consulta do relatório financeiro)_

### Versão feita pelo grupo (sem IA)

```csharp
// colar aqui
```

### Versão gerada com IA

```csharp
// colar aqui
```

### Discussão crítica

- **Diferenças principais:** _(preencher)_
- **O que a IA fez melhor:** _(preencher)_
- **O que a IA errou ou fez pior:** _(preencher)_
- **Versão adotada no projeto e por quê:** _(preencher)_

## Conclusão sobre o uso de IA

_(preencher ao final: quanto a IA ajudou, riscos percebidos, como o grupo garantiu o domínio do código)_
