using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestão.Model;

namespace SistemaDeGestão.Database.Configurations
{
    public class EventoConfiguration : IEntityTypeConfiguration<Evento>
    {
        public void Configure(EntityTypeBuilder<Evento> builder)
        {
            // Herança TPT: tabela Eventos + Conferencias + Workshops
            builder.UseTptMappingStrategy();
            builder.ToTable("Eventos");

            builder.Property(e => e.Titulo).HasMaxLength(150).IsRequired();
            builder.Property(e => e.Descricao).HasMaxLength(1000);
            builder.Property(e => e.PrecoInscricao).HasPrecision(10, 2);

            // Enum salvo como texto ("Rascunho", "Publicado", "Encerrado")
            builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);

            // 1:N - Organizador organiza Eventos
            builder.HasOne(e => e.Organizador)
                   .WithMany(o => o.Eventos)
                   .HasForeignKey(e => e.OrganizadorId)
                   .OnDelete(DeleteBehavior.Restrict);

            // 1:N (agregação) - Local sedia Eventos; o local existe sem o evento
            builder.HasOne(e => e.Local)
                   .WithMany(l => l.Eventos)
                   .HasForeignKey(e => e.LocalId)
                   .OnDelete(DeleteBehavior.Restrict);

            // N:N - Evento x Palestrante (tabela de junção EventoPalestrante)
            builder.HasMany(e => e.Palestrantes)
                   .WithMany(p => p.Eventos)
                   .UsingEntity(j => j.ToTable("EventoPalestrante"));
        }
    }

    public class ConferenciaConfiguration : IEntityTypeConfiguration<Conferencia>
    {
        public void Configure(EntityTypeBuilder<Conferencia> builder)
        {
            builder.ToTable("Conferencias");
            builder.Property(c => c.Tema).HasMaxLength(150);
        }
    }

    public class WorkshopConfiguration : IEntityTypeConfiguration<Workshop>
    {
        public void Configure(EntityTypeBuilder<Workshop> builder)
        {
            builder.ToTable("Workshops");
            builder.Property(w => w.Material).HasMaxLength(500);
        }
    }

    public class LocalConfiguration : IEntityTypeConfiguration<Local>
    {
        public void Configure(EntityTypeBuilder<Local> builder)
        {
            builder.ToTable("Locais");
            builder.Property(l => l.Nome).HasMaxLength(150).IsRequired();
            builder.Property(l => l.Endereco).HasMaxLength(300).IsRequired();
        }
    }
}
