using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using ProyectoFinalBD.View;
using Calendar = Avalonia.Controls.Calendar;

namespace ProyectoFinalBD
{
    public partial class MainMenu : Window
    {
        private UserController userController;
        public MainMenu()
        {
            userController = new UserController();
            InitializeComponent();

            // Cargar vista inicial por defecto
            ContentArea.Content = new Welcome();
            
        }

        private void OpenViewEntidades(object? sender, RoutedEventArgs e)
        {
            ContentArea.Content = new Entities();
            
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
            // Panel principal
            var contentPanel = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                }
            };

            // Panel horizontal para botones
            var buttonBorder = new Border
            {
                Background = new SolidColorBrush(Colors.LightGray),
                CornerRadius = new CornerRadius(10),
                Child = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Spacing = 20,
                    Margin = new Thickness(20)
                }
            };

            // Configuración de los botones
            var btnCalculadora = new Button
            {
                Name = "BtnCalculadora",
                Content = "Calculadora",
                Width = 150,
                Height = 50,
                Margin = new Thickness(5),
                Background = new SolidColorBrush(Colors.Sienna),
                CornerRadius = new CornerRadius(10),
            };

            var btnCalendario = new Button
            {
                Name = "BtnCalendario",
                Content = "Calendario",
                Width = 150,
                Height = 50,
                Margin = new Thickness(5),
                Background = new SolidColorBrush(Colors.Sienna),
                CornerRadius = new CornerRadius(10)
            };

            // Manejadores de eventos
            btnCalculadora.Click += (s, args) =>
            {
                var calculadoraWindow = new Window
                {
                    Title = "Calculadora",
                    Content = new Calculator(),
                    Width = 300,
                    Height = 400,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                calculadoraWindow.Show();
            };

            btnCalendario.Click += (s, args) =>
            {
                var calendarioWindow = new Window
                {
                    Title = "Calendario",
                    Content = new Calendar(),
                    Width = 400,
                    Height = 500,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                calendarioWindow.Show();
            };

            // Agregar botones al panel
            if (buttonBorder.Child is StackPanel panel)
            {
                panel.Children.Add(btnCalculadora);
                panel.Children.Add(btnCalendario);
            }

            // Agregar el panel de botones al Grid
            Grid.SetRow(buttonBorder, 0);
            contentPanel.Children.Add(buttonBorder);

            // Asignar el contenido al área principal
            if (this.FindControl<ContentControl>("ContentArea") is ContentControl contentArea)
            {
                contentArea.Content = contentPanel;
            }
        }

        private void OpenViewAyudas(object? sender, RoutedEventArgs e)
        {
            //ContentArea.Content = new AyudasView();
        }

        private void OpenViewLogin(object? sender, RoutedEventArgs e)
        {
            userController.Logout(); // Desloguea el usuario
            var loginWindow = new Login(); // Tu ventana de login
            loginWindow.Show();
            this.Close(); // Cierra este MainMenu
        }
        
        private void BtnCalculadora_Click(object? sender, RoutedEventArgs e)
        {
            var calculadoraWindow = new Window
            {
                Title = "Calculadora",
                Content = new Calculator(),
                Width = 300,
                Height = 400
            };
            calculadoraWindow.Show();
        }

        private void BtnCalendario_Click(object? sender, RoutedEventArgs e)
        {
            var calendarioWindow = new Window
            {
                Title = "Calendario",
                Content = new Calendar(),
                Width = 400,
                Height = 500
            };
            calendarioWindow.Show();
        }

    }
}