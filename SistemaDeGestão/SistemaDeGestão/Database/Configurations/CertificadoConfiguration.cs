using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestão.Model;

namespace SistemaDeGestão.Database.Configurations
{
    public class CertificadoConfiguration : IEntityTypeConfiguration<Certificado>
    {
        public void Configure(EntityTypeBuilder<Certificado> builder)
        {
            builder.ToTable("Certificados");

            builder.Property(c => c.Codigo).HasMaxLength(20).IsRequired();
            builder.HasIndex(c => c.Codigo).IsUnique();

            // Uma pessoa recebe no máximo um certificado por evento
            builder.HasIndex(c => new { c.PessoaId, c.EventoId }).IsUnique();

            // 1:N - Pessoa (Participante ou Palestrante) recebe Certificados
            builder.HasOne(c => c.Pessoa)
                   .WithMany(p => p.Certificados)
                   .HasForeignKey(c => c.PessoaId)
                   .OnDelete(DeleteBehavior.Restrict);

            // 1:N - apagar o evento apaga os certificados dele
            builder.HasOne(c => c.Evento)
                   .WithMany(e => e.Certificados)
                   .HasForeignKey(c => c.EventoId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
