using System.Configuration;
using System.Data;
using System.Windows;
using QuestPDF;

using QuestPDF.Infrastructure;


namespace Cerberus
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set the QuestPDF license here
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        }

    }

}
