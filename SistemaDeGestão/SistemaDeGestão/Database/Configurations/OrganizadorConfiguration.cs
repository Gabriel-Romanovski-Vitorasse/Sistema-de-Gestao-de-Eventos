using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestão.Model;

namespace SistemaDeGestão.Database.Configurations
{
    public class OrganizadorConfiguration : IEntityTypeConfiguration<Organizador>
    {
        public void Configure(EntityTypeBuilder<Organizador> builder)
        {
            // Herança TPT: tabela Organizadores + PessoasFisicas + PessoasJuridicas
            builder.UseTptMappingStrategy();
            builder.ToTable("Organizadores");

            builder.Property(o => o.Nome).HasMaxLength(100).IsRequired();
            builder.Property(o => o.Email).HasMaxLength(150).IsRequired();
            builder.Property(o => o.Telefone).HasMaxLength(20);

            builder.HasIndex(o => o.Email).IsUnique();
        }
    }

    public class PessoaFisicaConfiguration : IEntityTypeConfiguration<PessoaFisica>
    {
        public void Configure(EntityTypeBuilder<PessoaFisica> builder)
        {
            builder.ToTable("PessoasFisicas");
            builder.Property(p => p.Cpf).HasMaxLength(14).IsRequired();
            builder.HasIndex(p => p.Cpf).IsUnique();
        }
    }

    public class PessoaJuridicaConfiguration : IEntityTypeConfiguration<PessoaJuridica>
    {
        public void Configure(EntityTypeBuilder<PessoaJuridica> builder)
        {
            builder.ToTable("PessoasJuridicas");
            builder.Property(p => p.Cnpj).HasMaxLength(18).IsRequired();
            builder.Property(p => p.RazaoSocial).HasMaxLength(150).IsRequired();
            builder.HasIndex(p => p.Cnpj).IsUnique();
        }
    }
}
