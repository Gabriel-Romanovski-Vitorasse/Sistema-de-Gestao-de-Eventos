using SistemaDeGestão.Model;
using SistemaDeGestão.Services;

namespace SistemaDeGestão.Database
{
    /// <summary>
    /// Insere dados de exemplo quando o banco está vazio, para visualizar as telas.
    /// CPFs e CNPJs são fictícios e diferentes entre si por causa dos índices únicos.
    /// Todas as contas de exemplo entram com a senha "elo123".
    /// </summary>
    public static class DbSeeder
    {
        public const string SenhaDemo = "elo123";

        public static void Popular(Db db)
        {
            // Só popula se ainda não houver eventos nem locais; contas criadas pela tela de login são mantidas.
            if (db.Locais.Any() || db.Eventos.Any())
                return;

            // Um único hash para todas as contas de exemplo (o PBKDF2 é lento de propósito).
            string senha = Senha.GerarHash(SenhaDemo);

            // ----- Locais -----
            var aurora = new Local { Nome = "Centro de Convenções Aurora", Endereco = "Av. dos Encontros, 120 · Vila Nova · São Paulo, SP", Capacidade = 500 };
            var horizonte = new Local { Nome = "Laboratório Horizonte", Endereco = "Rua das Ideias, 85 · Centro · São Paulo, SP", Capacidade = 40 };
            var caminhos = new Local { Nome = "Auditório Caminhos", Endereco = "Av. do Saber, 240 · Jardim Sol · São Paulo, SP", Capacidade = 200 };
            var novaTrilha = new Local { Nome = "Espaço Nova Trilha", Endereco = "Rua do Futuro, 32 · Vila Nova · São Paulo, SP", Capacidade = 150 };
            var salaCriativa = new Local { Nome = "Sala Criativa", Endereco = "Rua das Ideias, 85 · Centro · São Paulo, SP", Capacidade = 30 };
            db.Locais.AddRange(aurora, horizonte, caminhos, novaTrilha, salaCriativa);
            db.SaveChanges();

            // ----- Organizadores -----
            var conecta = new PessoaJuridica
            {
                SenhaHash = senha,
                Nome = "Conecta Educação", Email = "contato@conecta.example", Telefone = "(11) 0000-0000",
                Cnpj = "00.000.000/0001-01", RazaoSocial = "Conecta Educação e Encontros Ltda.",
            };
            var luiza = new PessoaFisica { SenhaHash = senha, Nome = "Luiza Costa", Email = "luiza.c@example.com", Telefone = "(11) 0000-0001", Cpf = "000.000.001-01" };
            var institutoCaminhos = new PessoaJuridica
            {
                SenhaHash = senha,
                Nome = "Instituto Caminhos", Email = "eventos@caminhos.example", Telefone = "(11) 0000-0002",
                Cnpj = "00.000.000/0001-02", RazaoSocial = "Instituto Caminhos de Aprendizagem",
            };
            var andre = new PessoaFisica { SenhaHash = senha, Nome = "André Melo", Email = "andre.m@example.com", Telefone = "(11) 0000-0003", Cpf = "000.000.001-02" };
            var redeAprender = new PessoaJuridica
            {
                SenhaHash = senha,
                Nome = "Rede Aprender", Email = "ola@aprender.example", Telefone = "(11) 0000-0004",
                Cnpj = "00.000.000/0001-03", RazaoSocial = "Rede Aprender Educação S.A.",
            };
            db.Organizadores.AddRange(conecta, luiza, institutoCaminhos, andre, redeAprender);
            db.SaveChanges();

            // ----- Palestrantes -----
            var marina = new Palestrante
            {
                SenhaHash = senha,
                Nome = "Marina Alves", Email = "marina.a@example.com", Cpf = "000.000.002-01", Especialidade = "Inovação educacional",
                MiniBio = "Pesquisadora em educação e design de aprendizagem. Há 12 anos conecta inovação, formação docente e práticas centradas nas pessoas.",
            };
            var rafael = new Palestrante
            {
                SenhaHash = senha,
                Nome = "Rafael Nunes", Email = "rafael.n@example.com", Cpf = "000.000.002-02", Especialidade = "Tecnologia e aprendizagem",
                MiniBio = "Conecta tecnologia, criatividade e formação de educadores.",
            };
            var juliana = new Palestrante { SenhaHash = senha, Nome = "Juliana Torres", Email = "juliana.t@example.com", Cpf = "000.000.002-03", Especialidade = "Design de aprendizagem" };
            var pedro = new Palestrante { SenhaHash = senha, Nome = "Pedro Carvalho", Email = "pedro.c@example.com", Cpf = "000.000.002-04", Especialidade = "Inteligência artificial" };
            var sofia = new Palestrante { SenhaHash = senha, Nome = "Sofia Ribeiro", Email = "sofia.r@example.com", Cpf = "000.000.002-05", Especialidade = "Educação inclusiva" };
            db.Palestrantes.AddRange(marina, rafael, juliana, pedro, sofia);

            // ----- Participantes -----
            var ana = new Participante { SenhaHash = senha, Nome = "Ana Beatriz Lima", Email = "ana.lima@example.com", Cpf = "000.000.003-01", Instituicao = "Universidade Horizonte" };
            var bruno = new Participante { SenhaHash = senha, Nome = "Bruno Martins", Email = "bruno.m@example.com", Cpf = "000.000.003-02", Instituicao = "Instituto Caminhos" };
            var camila = new Participante { SenhaHash = senha, Nome = "Camila Rocha", Email = "camila.r@example.com", Cpf = "000.000.003-03", Instituicao = "Escola Nova Trilha" };
            var diego = new Participante { SenhaHash = senha, Nome = "Diego Santos", Email = "diego.s@example.com", Cpf = "000.000.003-04", Instituicao = "Universidade Horizonte" };
            var elisa = new Participante { SenhaHash = senha, Nome = "Elisa Moreira", Email = "elisa.m@example.com", Cpf = "000.000.003-05", Instituicao = "Rede Aprender" };
            var felipe = new Participante { SenhaHash = senha, Nome = "Felipe Azevedo", Email = "felipe.a@example.com", Cpf = "000.000.003-06", Instituicao = "Instituto Caminhos" };
            db.Participantes.AddRange(ana, bruno, camila, diego, elisa, felipe);
            db.SaveChanges();

            // ----- Eventos -----
            var conecta2026 = new Conferencia
            {
                Titulo = "Conecta 2026: futuros da educação",
                Descricao = "Uma conferência para educadores, pesquisadores e profissionais que acreditam no poder de aprender juntos. Explore novas práticas, compartilhe experiências e construa conexões para uma educação mais humana.",
                DataInicio = new DateTime(2026, 11, 24, 9, 0, 0), DataFim = new DateTime(2026, 11, 25, 18, 0, 0),
                Capacidade = 300, PrecoInscricao = 249m, Status = StatusEvento.Publicado,
                Local = aurora, Organizador = conecta, Palestrantes = [marina, rafael],
                Tema = "Inovação e aprendizagem", NumeroSessoes = 8,
            };
            var designExperiencias = new Workshop
            {
                Titulo = "Design de experiências de aprendizagem",
                Descricao = "Workshop prático para planejar experiências de aprendizagem centradas nas pessoas.",
                DataInicio = new DateTime(2026, 11, 26, 9, 0, 0), DataFim = new DateTime(2026, 11, 26, 18, 0, 0),
                Capacidade = 40, PrecoInscricao = 180m, Status = StatusEvento.Publicado,
                Local = horizonte, Organizador = conecta, Palestrantes = [juliana],
                CargaHoraria = 8, Material = "Caderno de atividades e kit prático",
            };
            var iaPratica = new Workshop
            {
                Titulo = "IA na prática docente",
                Descricao = "Usos de inteligência artificial no planejamento e na avaliação em sala de aula.",
                DataInicio = new DateTime(2026, 12, 3, 9, 0, 0), DataFim = new DateTime(2026, 12, 3, 18, 0, 0),
                Capacidade = 30, PrecoInscricao = 120m, Status = StatusEvento.Publicado,
                Local = salaCriativa, Organizador = institutoCaminhos, Palestrantes = [pedro],
                CargaHoraria = 8, Material = "Roteiro de atividades com ferramentas de IA",
            };
            var encontroInovacao = new Conferencia
            {
                Titulo = "Encontro de inovação e impacto",
                Descricao = "Dois dias de debates sobre inovação e impacto social na educação.",
                DataInicio = new DateTime(2026, 12, 10, 9, 0, 0), DataFim = new DateTime(2026, 12, 11, 18, 0, 0),
                Capacidade = 150, PrecoInscricao = 320m, Status = StatusEvento.Rascunho,
                Local = novaTrilha, Organizador = institutoCaminhos,
                Tema = "Inovação e impacto social", NumeroSessoes = 6,
            };
            var educacaoTransforma = new Conferencia
            {
                Titulo = "Educação que transforma",
                Descricao = "Conferência gratuita sobre práticas que transformam escolas e comunidades.",
                DataInicio = new DateTime(2026, 10, 15, 9, 0, 0), DataFim = new DateTime(2026, 10, 16, 18, 0, 0),
                Capacidade = 200, PrecoInscricao = 0m, Status = StatusEvento.Encerrado,
                Local = caminhos, Organizador = luiza, Palestrantes = [marina, rafael],
                Tema = "Educação transformadora", NumeroSessoes = 6,
            };
            var facilitacao = new Workshop
            {
                Titulo = "Facilitação de grupos criativos",
                Descricao = "Técnicas de facilitação para conduzir grupos em processos criativos.",
                DataInicio = new DateTime(2026, 12, 12, 9, 0, 0), DataFim = new DateTime(2026, 12, 12, 18, 0, 0),
                Capacidade = 24, PrecoInscricao = 150m, Status = StatusEvento.Publicado,
                Local = horizonte, Organizador = luiza,
                CargaHoraria = 8, Material = "Cartões de dinâmicas e guia do facilitador",
            };
            db.Eventos.AddRange(conecta2026, designExperiencias, iaPratica, encontroInovacao, educacaoTransforma, facilitacao);
            db.SaveChanges();

            // ----- Inscrições e pagamentos -----
            db.Inscricoes.AddRange(
                Inscricao(ana, conecta2026, new DateTime(2026, 11, 5, 10, 32, 0), StatusInscricao.Pendente),
                Inscricao(bruno, conecta2026, new DateTime(2026, 11, 4, 15, 10, 0), StatusInscricao.Confirmada),
                Inscricao(camila, designExperiencias, new DateTime(2026, 11, 4, 9, 45, 0), StatusInscricao.Confirmada),
                Inscricao(diego, conecta2026, new DateTime(2026, 11, 3, 18, 2, 0), StatusInscricao.Cancelada),
                Inscricao(elisa, iaPratica, new DateTime(2026, 11, 3, 11, 20, 0), StatusInscricao.Confirmada),
                // Presenças no evento encerrado (base dos certificados de participação)
                Inscricao(ana, educacaoTransforma, new DateTime(2026, 10, 1, 14, 0, 0), StatusInscricao.Confirmada),
                Inscricao(camila, educacaoTransforma, new DateTime(2026, 10, 2, 16, 30, 0), StatusInscricao.Confirmada));
            db.SaveChanges();

            // ----- Certificado já emitido no evento encerrado -----
            // Os demais (Camila, Marina e Rafael) aparecem como "Pronto para emitir" na tela de certificados.
            db.Certificados.Add(new Certificado
            {
                Codigo = "ELO-2026-0001", Pessoa = ana, Evento = educacaoTransforma, DataEmissao = new DateTime(2026, 10, 17),
            });
            db.SaveChanges();
        }

        /// <summary>Cria a inscrição; se confirmada, já com o pagamento (valor do evento, via Pix).</summary>
        private static Inscricao Inscricao(Participante participante, Evento evento, DateTime data, StatusInscricao status) => new()
        {
            Participante = participante,
            Evento = evento,
            DataInscricao = data,
            Status = status,
            Pagamento = status == StatusInscricao.Confirmada
                ? new Pagamento { Valor = evento.PrecoInscricao, DataPagamento = data, Metodo = "Pix · confirmação instantânea" }
                : null,
        };
    }
}
