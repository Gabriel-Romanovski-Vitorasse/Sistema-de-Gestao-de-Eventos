using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestão.Model;

namespace SistemaDeGestão.Database.Configurations
{
    public class InscricaoConfiguration : IEntityTypeConfiguration<Inscricao>
    {
        public void Configure(EntityTypeBuilder<Inscricao> builder)
        {
            builder.ToTable("Inscricoes");

            // Enum salvo como texto ("Pendente", "Confirmada", "Cancelada")
            builder.Property(i => i.Status).HasConversion<string>().HasMaxLength(20);

            // Um participante não pode se inscrever duas vezes no mesmo evento
            builder.HasIndex(i => new { i.ParticipanteId, i.EventoId }).IsUnique();

            // 1:N - Participante realiza Inscricoes
            builder.HasOne(i => i.Participante)
                   .WithMany(p => p.Inscricoes)
                   .HasForeignKey(i => i.ParticipanteId)
                   .OnDelete(DeleteBehavior.Restrict);

            // 1:N (composição) - apagar o evento apaga as inscrições
            builder.HasOne(i => i.Evento)
                   .WithMany(e => e.Inscricoes)
                   .HasForeignKey(i => i.EventoId)
                   .OnDelete(DeleteBehavior.Cascade);

            // 1:1 (composição) - InscricaoId em Pagamento recebe índice único
            builder.HasOne(i => i.Pagamento)
                   .WithOne(p => p.Inscricao)
                   .HasForeignKey<Pagamento>(p => p.InscricaoId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class PagamentoConfiguration : IEntityTypeConfiguration<Pagamento>
    {
        public void Configure(EntityTypeBuilder<Pagamento> builder)
        {
            builder.ToTable("Pagamentos");
            builder.Property(p => p.Valor).HasPrecision(10, 2);
            builder.Property(p => p.Metodo).HasMaxLength(30);
        }
    }
}
