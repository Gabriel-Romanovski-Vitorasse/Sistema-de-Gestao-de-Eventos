using System.Windows;
using Microsoft.EntityFrameworkCore;
using SistemaDeGestão.Database;

namespace SistemaDeGestão
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Regras de negócio violadas nos controllers viram um aviso na tela em vez de fechar o programa.
            DispatcherUnhandledException += (_, args) =>
            {
                if (args.Exception is not InvalidOperationException erro) return;
                MessageBox.Show(erro.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                args.Handled = true;
            };

            // Cria o banco (se ainda não existir) e aplica as migrations pendentes.
            using (var db = new Db())
            {
                db.Database.Migrate();
            }

            base.OnStartup(e);
        }
    }
}
