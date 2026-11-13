using Microsoft.EntityFrameworkCore;
using SistemaDeGestão.Model;

namespace SistemaDeGestão.Database
{
    public class Db : DbContext
    {
        private const string ConnectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=SistemaDeGestaoEventos;Trusted_Connection=True;TrustServerCertificate=True";

        public DbSet<Pessoa> Pessoas => Set<Pessoa>();
        public DbSet<Participante> Participantes => Set<Participante>();
        public DbSet<Palestrante> Palestrantes => Set<Palestrante>();

        public DbSet<Organizador> Organizadores => Set<Organizador>();
        public DbSet<PessoaFisica> OrganizadoresPessoaFisica => Set<PessoaFisica>();
        public DbSet<PessoaJuridica> OrganizadoresPessoaJuridica => Set<PessoaJuridica>();

        public DbSet<Evento> Eventos => Set<Evento>();
        public DbSet<Conferencia> Conferencias => Set<Conferencia>();
        public DbSet<Workshop> Workshops => Set<Workshop>();

        public DbSet<Local> Locais => Set<Local>();
        public DbSet<Inscricao> Inscricoes => Set<Inscricao>();
        public DbSet<Pagamento> Pagamentos => Set<Pagamento>();
        public DbSet<Certificado> Certificados => Set<Certificado>();

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
                options.UseSqlServer(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cada entidade tem sua própria classe de configuração (SOLID - SRP)
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Db).Assembly);
        }
    }
}
