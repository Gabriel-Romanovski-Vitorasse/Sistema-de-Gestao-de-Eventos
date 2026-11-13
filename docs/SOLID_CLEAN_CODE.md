# SOLID e Clean Code no projeto

Locais do código (pasta `SistemaDeGestão/SistemaDeGestão/`) onde os princípios foram aplicados, com o motivo.
Ao final, os pontos em que **não** foram aplicados, para não superestimar o que existe.

## SOLID

### S: Responsabilidade única (SRP)

| Onde | Como | Motivo |
|---|---|---|
| `Model/*` | As classes de domínio só têm dados e regras do próprio objeto (`Evento.VagasDisponiveis()`, `Inscricao.Pagar()`/`Cancelar()`). | Regras do domínio ficam separadas da tela e do banco. |
| `Database/Configurations/*Configuration.cs` | Uma classe `IEntityTypeConfiguration<T>` por entidade (tabela, tamanhos, índices, relacionamentos). | O mapeamento do EF não polui as classes de domínio, e mudar a tabela de uma entidade não mexe nas outras. |
| `Database/Db.cs` | Só expõe os `DbSet` e aplica as configurações (`ApplyConfigurationsFromAssembly`). | O contexto não acumula regras de mapeamento. |
| `Database/DbSeeder.cs` | Só insere os dados de exemplo. | A inicialização do banco fica separada do `App` e dos controllers. |
| `Controller/*Controller.cs` | Um controller por entidade/caso de uso (Evento, Participante, Palestrante, Organizador, Local, Inscrição, Certificado). | Cada tela conversa com o controller do assunto dela; o acesso a dados fica fora das telas. |
| `ViewModels/*Item.cs` | Classes de exibição (`EventoItem`, `InscricaoItem`, `CertificadoItem`…) que transformam o model em texto da tela. | Formatação ("R$ 249,00", "82 de 300", "24–25 nov. 2026") não fica no domínio nem no banco. |
| `ViewModels/Formato.cs` | Toda a formatação de moeda, datas, iniciais, plural e status num só lugar. | Uma regra de formato muda em um lugar só. |
| `Views/*.xaml.cs` | O code-behind só trata eventos de tela e chama o controller; nenhuma tela acessa o `Db`. | Separação entre interface e dados (padrão em camadas: View → Controller → Db). |
| `Services/Navigator.cs` | Só cuida da navegação entre telas e do parâmetro (Id) passado. | As telas não se conhecem diretamente. |

### O: Aberto/fechado (OCP)

| Onde | Como | Motivo |
|---|---|---|
| `Model/Evento.cs` (abstrata) → `Conferencia`, `Workshop` | Novo tipo de evento = nova subclasse; `VagasDisponiveis()` e os relacionamentos são herdados. | Estende sem alterar `Evento`. |
| `Model/Organizador.cs` (abstrata) → `PessoaFisica`, `PessoaJuridica` | Mesmo princípio para os tipos de organizador. | Idem. |
| `Db.OnModelCreating` | `ApplyConfigurationsFromAssembly` encontra sozinho toda nova classe de configuração. | Criar uma entidade nova não exige editar o `Db` para registrar o mapeamento. |

### L: Substituição de Liskov (LSP)

| Onde | Como | Motivo |
|---|---|---|
| `Pessoa` → `Participante`, `Palestrante` | `Certificado.Pessoa` aceita qualquer um dos dois; `CertificadoItem` trata ambos pelo tipo base. | As filhas podem ser usadas onde se espera `Pessoa` sem quebrar nada. |
| `Evento` → `Conferencia`, `Workshop` | `EventoItem`, `EventosView` e `EventoDetalhesView` trabalham com `Evento`; só os campos específicos usam `switch` por tipo. | Idem. |

### I: Segregação de interfaces (ISP)

| Onde | Como | Motivo |
|---|---|---|
| `Model/IPagamento.cs`, `Model/ICertificado.cs` | Interfaces pequenas, com um método cada (`Pagar()`, `Emitir()`). | `Inscricao` implementa só pagamento; `Participante` e `Palestrante` só certificado. Ninguém é obrigado a implementar o que não usa. |

### D: Inversão de dependência (DIP)

| Onde | Como | Motivo |
|---|---|---|
| `Database/Configurations/*` | As configurações dependem da abstração `IEntityTypeConfiguration<T>` do EF. | O `Db` descobre as configurações pela abstração, não por classes concretas. |

## Clean Code

| Onde | Prática | Motivo |
|---|---|---|
| Todo o projeto | Nomes em português, do domínio (`VagasDisponiveis`, `StatusInscricao`, `PeriodoExtenso`, `Recarregar`). | O código lê como o enunciado do problema. |
| `Model/StatusInscricao.cs`, `Model/StatusEvento.cs` | Enums no lugar de strings soltas; salvos como texto no banco (`HasConversion<string>()`). | Evita erros de digitação e "números mágicos" no banco. |
| `Model/Evento.cs` (`Esgotado`), `Certificado.cs` (`Emitido`), `Codigo` nos models | Valores calculados como propriedades só de leitura, sem `set`. | Não há dado duplicado no banco que possa ficar inconsistente; o EF não mapeia. |
| `Views/InscricoesView.xaml.cs`, `EventosView.xaml.cs` e demais | Contadores (confirmadas, pendentes, vagas, receita) vêm dos dados/controllers, não de constantes. | Remove os valores fixos do protótipo (476, 501, 3762…). |
| `Views/*.xaml.cs` | Métodos curtos com uma intenção (`Recarregar`, `Atualizar`, `Preencher`, `MontarEvento`). | Facilita leitura e manutenção. |
| `Views/EventoCadastroView.xaml.cs` (`MontarEvento`) | Valida e monta o objeto num só método; erros listados de uma vez ao usuário. | Evita validação espalhada pelos handlers. |
| `ViewModels/Formato.cs` | Funções puras e reutilizadas por várias telas. | Evita duplicação (DRY). |
| `Database/Configurations/*` | Comentários só onde explicam decisões (TPT, composição/agregação, `OnDelete`). | Comentário explica o "porquê", não o óbvio. |
| `Controller/*` | Contrato definido antes da implementação (assinaturas fixas). | As telas e a implementação podem evoluir separadamente. |

## Onde os princípios **não** foram aplicados (limitações conhecidas)

- **DIP nos controllers:** as telas criam os controllers com `new` e os controllers não têm interface (`IEventoController` etc.). Não há injeção de dependência, o que dificulta testes isolados.
- **Code-behind em vez de MVVM completo:** as telas usam eventos (`Click`, `SelectionChanged`) no code-behind, não `Command`/`INotifyPropertyChanged`. Os `ViewModels/` são só classes de exibição.
- **Troca de tipo com TPT:** mudar Conferência↔Workshop ou PF↔PJ cria um objeto novo com o mesmo `Id`; o tratamento disso fica a cargo do `Salvar` (parte manual).
- **Connection string fixa** em `Db.cs` (sem arquivo de configuração).
