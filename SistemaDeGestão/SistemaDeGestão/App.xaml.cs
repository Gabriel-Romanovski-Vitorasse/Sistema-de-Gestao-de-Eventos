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
            // Aplica as migrations pendentes e insere os dados de exemplo se o banco estiver vazio.
            using (var db = new Db())
            {
                db.Database.Migrate();
                DbSeeder.Popular(db);
            }

            base.OnStartup(e);
        }
    }
}
