# Registro de uso de IA

Nível permitido na atividade: **IA-2 (automação guiada)**: a IA automatiza partes da tarefa, com supervisão humana contínua.

Declaração de método: *a IA auxiliou na geração inicial de código e de diagramas, com revisão humana completa.*

## Ferramentas usadas

| Ferramenta | Para quê |
|---|---|
| Claude (Anthropic) | Planejamento da modelagem, rascunho de diagramas, classes Model, integração da interface WPF, controllers, consultas SQL, migrations e tela de login |

## Registro de uso

Cada linha registra um uso.

| # | Data | Integrante | Ferramenta | Prompt (resumo) | O que a IA gerou |
|---|---|---|---|---|---|
| 1 | 06/11/2026 | Arthur | Claude | Envio do PDF da atividade + "Pegamos o sistema de eventos" | Resumo dos requisitos e pontos a ajustar no modelo do professor |
| 2 | 06/11/2026 | Arthur | Claude | "Queria começar reformulando o diagrama de classes" | Rascunho do diagrama de classes com Local, Patrocinador, Pagamento e CheckIn |
| 3 | 06/11/2026 | Arthur | Claude | "Acho melhor palestrante se tornar pessoa também" | Versão com Participante e Palestrante herdando de Pessoa |
| 4 | 06/11/2026 | Arthur | Claude | "Pode continuar" (casos de uso) | Rascunho do diagrama de casos de uso (3 atores, 14 casos) |
| 5 | 06/11/2026 | Arthur | Claude | Envio da imagem do diagrama + "vc consegue editar? já colocando os atributos" | Arquivo .drawio com atributos, multiplicidades e notação UML |
| 6 | 06/11/2026 | Arthur | Claude | "Diagrama de ações também" | Arquivo .drawio de casos de uso |
| 7 | 06/11/2026 | Arthur | Claude | "Preciso normalizar algo para o banco de dados?" | Orientação sobre TPH x TPT, tabelas de junção N:N, 1:1 com índice único |
| 8 | 10/11/2026 | Arthur | Claude | Envio do .drawio + link do repositório + "Preciso iniciar colocando os atributos das classes" | Classes da pasta Model com atributos, FKs e navegação; renomeou Empresa → PessoaJuridica e lPagamento/lCertificado → IPagamento/ICertificado |
| 9 | 10/11/2026 | Arthur | Claude | "Pode deixar 1x1 em pagamento e inscrição... deixe o controller como PessoaJuridica" | Classe Pagamento (1:1 com Inscricao), Pagamento no .drawio, renomeou EmpresaController e criou este arquivo |
| 10 | 10/11/2026 | Arthur | Claude | "Sim" (criar o Db.cs com DbContext e configurações do EF) | `Db.cs` (DbContext com SQL Server LocalDB), classes de configuração em `Database/Configurations` (herança TPT, N:N EventoPalestrante, 1:1 Inscricao-Pagamento, índices únicos, OnDelete) e pacotes EF Core no .csproj |
| 11 | 12/11/2026 | Arthur | Claude | Prompt com divisão IA x Manual (etapas 1 a 6): juntar a interface do EloEventos ao projeto principal, ajustar os models às telas, criar o contrato dos controllers só com assinaturas, ligar as telas aos controllers, criar o DbSeeder e documentar SOLID/Clean Code. Corpo dos controllers, SQL puro dos relatórios e testes ficaram inicialmente como parte manual (depois feitos com IA, ver linha 12) | Cópia das 9 telas (Views, Controls, Themes, Fonts, Assets, Navigator, MainWindow) com namespace `SistemaDeGestão` e SharpVectors.Wpf; models ajustados (`StatusInscricao.Confirmada`, enum `StatusEvento`, classe `Certificado`, Participante implementa `ICertificado`, propriedades calculadas `Codigo`/`Esgotado`/`Emitido`); pasta `ViewModels/`; controllers Evento, Participante, Palestrante, Organizador, Local, Inscricao e Certificado só com assinaturas (corpo `NotImplementedException`); records `IndicadoresInscricoes` e `RelatorioFinanceiroItem`; telas ligadas aos controllers sem dados fixos; `DbSeeder` com Migrate + seed no startup; `docs/SOLID_CLEAN_CODE.md`; `.gitignore` |
| 12 | 12/11/2026 | Arthur | Claude | "Faz os controllers logo" | Implementação de todos os controllers com EF Core (Listar/BuscarPorId com Include, Salvar, Excluir, validações de negócio e de duplicidade); troca de modalidade do evento e de tipo do organizador via SQL na herança TPT; `ObterIndicadores()` e `RelatorioFinanceiro()` em SQL puro; regra de emissão de certificado com código `ELO-AAAA-NNNN`; classe `ValidacaoPessoa`; aviso na tela para erros de regra de negócio no `App.xaml.cs` |
| 13 | 12/11/2026 | Arthur | Claude | "Apague os arquivos inúteis" | Remoção de 6 controllers vazios, `lPagamento.cs`, da pasta `View/` antiga e do arquivo `AtributosDiagramaDeClasses`; restauração de `Controls/Components.cs`, que tinha sido apagado e impedia a compilação |
| 14 | 12/11/2026 | Arthur | Claude | "Apaga os dados mokados e crie uma tela de login para os tipos de usuário" | Remoção do `DbSeeder` e dos textos de exemplo no XAML; tela `LoginWindow` com entrar e criar conta (Participante, Palestrante, Organizador PF/PJ); `AutenticacaoController`; `Services/Sessao` (perfis e permissões) e `Services/Senha` (hash PBKDF2); campo `SenhaHash` em Pessoa e Organizador; migration `AdicionaSenha`; menu e telas filtrados por perfil; lista de certificados com quem tem direito em eventos encerrados; banco recriado vazio |
| 15 | 12/11/2026 | Arthur | Claude | "Ajuste o uso de IA" / "deixe só listado o que você fez" | Atualização deste registro |
| 16 | 12/11/2026 | Arthur | Claude | "Faz um DbSeeder para melhor visualização" | `DbSeeder` recriado com os dados de exemplo anteriores (locais, organizadores, palestrantes, participantes, eventos, inscrições e um certificado emitido), todas as contas com a senha de demonstração `elo123`; chamada no startup quando ainda não há eventos nem locais (contas já criadas são mantidas) |
