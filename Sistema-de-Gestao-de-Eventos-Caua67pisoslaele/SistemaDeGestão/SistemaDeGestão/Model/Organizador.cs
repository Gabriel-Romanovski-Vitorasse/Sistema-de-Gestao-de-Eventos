using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaDeGestão.Model
{
    internal class Organizador
    {
        private int ID { get; set; }

        private string Nome { get; set; }

        private string Email { get; set; }

        private string NumeroTelefone { get; set; }

        public Organizador(int id, string nome, string email, string numeroTelefone)
        {
            ID = id;
            Nome = nome;
            Email = email;
            NumeroTelefone = numeroTelefone;
        }

        public Organizador(){}

        public List<Evento> Eventos;

        public void AdicionarEvento(Evento evento)
        {
            Eventos.Add(evento);
        }

        public void RemoverEvento(Evento evento)
        {
            Eventos.Remove(evento);
        }

        public void ListarEventos()
        {
            foreach (var evento in Eventos)
            {
                Console.WriteLine($"ID: {evento.ID}, Nome: {evento.Nome}, Data: {evento.Data}, Descrição: {evento.Descricao}, Duracao: {evento.Duracao}");
            }
        }
    }
}
