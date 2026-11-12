using SistemaDeGestão.Model;

namespace SistemaDeGestão.Controller
{
    public class CertificadoController
    {
        public List<Certificado> Listar()
        {
            throw new NotImplementedException("Implementação manual");
        }

        public Certificado? BuscarPorId(int id)
        {
            throw new NotImplementedException("Implementação manual");
        }

        // Emite o certificado da pessoa (Participante ou Palestrante) no evento.
        // Usado na tela de Certificados e no botão "Emitir certificado" da tela de Palestrantes.
        public Certificado Emitir(int pessoaId, int eventoId)
        {
            throw new NotImplementedException("Implementação manual");
        }
    }
}
