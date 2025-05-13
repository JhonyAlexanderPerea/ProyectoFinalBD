using Avalonia.Controls;
using Avalonia.Interactivity;
using ProyectoFinalBD.View;

namespace ProyectoFinalBD
{
    public partial class MainMenu : Window
    {
        public MainMenu()
        {
            InitializeComponent();

            // Cargar vista inicial por defecto
            ContentArea.Content = new Welcome();
            
        }

        private void OpenViewEntidades(object? sender, RoutedEventArgs e)
        {
            //ContentArea.Content = new EntidadesView();
        }

        private void OpenViewTransacciones(object? sender, RoutedEventArgs e)
        {
            //ContentArea.Content = new TransaccionesView();
        }

        private void OpenViewReportes(object? sender, RoutedEventArgs e)
        {
            //ContentArea.Content = new ReportesView();
        }

        private void OpenViewUtilidades(object? sender, RoutedEventArgs e)
        {
            //ContentArea.Content = new UtilidadesView();
        }

        private void OpenViewAyudas(object? sender, RoutedEventArgs e)
        {
            //ContentArea.Content = new AyudasView();
        }

        private void OpenViewLogin(object? sender, RoutedEventArgs e)
        {
            var loginWindow = new Login(); // Tu ventana de login
            loginWindow.Show();
            this.Close(); // Cierra este MainMenu
        }
    }
}