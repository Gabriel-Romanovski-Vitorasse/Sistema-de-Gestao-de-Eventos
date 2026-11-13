using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestão.Model;

namespace SistemaDeGestão.Database.Configurations
{
    public class PessoaConfiguration : IEntityTypeConfiguration<Pessoa>
    {
        public void Configure(EntityTypeBuilder<Pessoa> builder)
        {
            // Herança TPT: tabela Pessoas + uma tabela para cada filha
            builder.UseTptMappingStrategy();
            builder.ToTable("Pessoas");

            builder.Property(p => p.Nome).HasMaxLength(100).IsRequired();
            builder.Property(p => p.Cpf).HasMaxLength(14).IsRequired();
            builder.Property(p => p.Email).HasMaxLength(150).IsRequired();
            builder.Property(p => p.SenhaHash).HasMaxLength(200);

            builder.HasIndex(p => p.Cpf).IsUnique();
            builder.HasIndex(p => p.Email).IsUnique();
        }
    }

    public class ParticipanteConfiguration : IEntityTypeConfiguration<Participante>
    {
        public void Configure(EntityTypeBuilder<Participante> builder)
        {
            builder.ToTable("Participantes");
            builder.Property(p => p.Instituicao).HasMaxLength(150);
        }
    }

    public class PalestranteConfiguration : IEntityTypeConfiguration<Palestrante>
    {
        public void Configure(EntityTypeBuilder<Palestrante> builder)
        {
            builder.ToTable("Palestrantes");
            builder.Property(p => p.MiniBio).HasMaxLength(500);
            builder.Property(p => p.Especialidade).HasMaxLength(100);
        }
    }
}
