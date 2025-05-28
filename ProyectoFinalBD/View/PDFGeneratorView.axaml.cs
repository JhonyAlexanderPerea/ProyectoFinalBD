using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using ProyectoFinalBD.util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace ProyectoFinalBD.View
{
    public partial class PDFGeneratorView : UserControl
    {
        private readonly PDFQueries _pdfQueries;
        private ComboBox? _reportTypeCombo;
        private Button? _generatePdfButton;
        private TextBlock? _previewText;

        // Botones para consultas nuevas
        private Button? _btnEquiposFallas;
        private Button? _btnCostosMantenimiento;
        private Button? _btnEquiposFueraPlazo;
        private Button? _btnEquiposPorMunicipio;
        private Button? _btnAccesosPorRol;

        public PDFGeneratorView()
        {
            InitializeComponent();

            QuestPDF.Settings.License = LicenseType.Community;
            _pdfQueries = new PDFQueries();

            _reportTypeCombo = this.FindControl<ComboBox>("ReportTypeCombo");
            _generatePdfButton = this.FindControl<Button>("GeneratePdfButton");
            _previewText = this.FindControl<TextBlock>("PreviewText");

            // Buscar botones nuevas consultas
            _btnEquiposFallas = this.FindControl<Button>("BtnEquiposFallas");
            _btnCostosMantenimiento = this.FindControl<Button>("BtnCostosMantenimiento");
            _btnEquiposFueraPlazo = this.FindControl<Button>("BtnEquiposFueraPlazo");
            _btnEquiposPorMunicipio = this.FindControl<Button>("BtnEquiposPorMunicipio");
            _btnAccesosPorRol = this.FindControl<Button>("BtnAccesosPorRol");

            if (_reportTypeCombo != null && _generatePdfButton != null)
            {
                _reportTypeCombo.SelectionChanged += OnReportTypeChanged;
                _generatePdfButton.Click += OnGeneratePdfClick;
                _generatePdfButton.IsEnabled = false;
            }

            // Asignar eventos a botones nuevas consultas
            if (_btnEquiposFallas != null)
                _btnEquiposFallas.Click += async (_, _) => await EjecutarConsultaNuevaAsync(_pdfQueries.GetEquiposConMasFallasPorTipo);

            if (_btnCostosMantenimiento != null)
                _btnCostosMantenimiento.Click += async (_, _) => await EjecutarConsultaNuevaAsync(_pdfQueries.GetCostosMantenimientoPorProveedor);

            if (_btnEquiposFueraPlazo != null)
                _btnEquiposFueraPlazo.Click += async (_, _) => await EjecutarConsultaNuevaAsync(_pdfQueries.GetEquiposFueraDePlazo);

            if (_btnEquiposPorMunicipio != null)
                _btnEquiposPorMunicipio.Click += async (_, _) => await EjecutarConsultaNuevaAsync(_pdfQueries.GetEquiposPorMunicipioYTipo);

            if (_btnAccesosPorRol != null)
                _btnAccesosPorRol.Click += async (_, _) => await EjecutarConsultaNuevaAsync(_pdfQueries.GetAccesosPorRol);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnReportTypeChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_generatePdfButton != null && _reportTypeCombo != null)
            {
                _generatePdfButton.IsEnabled = _reportTypeCombo.SelectedItem != null;

                if (_reportTypeCombo.SelectedItem != null)
                {
                    // Actualiza la vista previa sólo para reportes antiguos (ComboBox)
                    _ = UpdatePreviewAsync();
                }
                else
                {
                    if (_previewText != null)
                        _previewText.Text = "";
                }
            }
        }

        private async Task UpdatePreviewAsync()
        {
            if (_reportTypeCombo == null || _previewText == null)
                return;

            try
            {
                var selectedReport = ((_reportTypeCombo.SelectedItem as ComboBoxItem)?.Content as string) ?? "";

                _previewText.Text = "Cargando vista previa...";

                DataTable? dt = selectedReport switch
                {
                    "Equipos más prestados" => _pdfQueries.GetEquiposMasPrestados(),
                    "Usuarios con más préstamos" => _pdfQueries.GetUsuariosConMasPrestamos(),
                    "Reporte de equipos en reparación" => _pdfQueries.GetEquiposEnReparacion(),
                    "Préstamos activos por tipo de equipo" => _pdfQueries.GetPrestamosActivosPorTipoEquipo(),
                    "Estadísticas de mantenimiento de equipos" => _pdfQueries.GetEstadisticasMantenimiento(),
                    _ => null
                };

                if (dt == null || dt.Rows.Count == 0)
                {
                    _previewText.Text = "No hay datos para mostrar.";
                    return;
                }

                _previewText.Text = ConstruirVistaPrevia(dt);
            }
            catch (Exception ex)
            {
                _previewText.Text = $"Error al cargar la vista previa: {ex.Message}";
            }
        }

        // Método común para mostrar vista previa (primeras filas y columnas)
        private string ConstruirVistaPrevia(DataTable dt)
        {
            var previewText = $"Vista previa ({dt.Rows.Count} filas, {dt.Columns.Count} columnas):\n";

            int maxRows = Math.Min(5, dt.Rows.Count);
            int maxCols = Math.Min(5, dt.Columns.Count);

            // Encabezados
            for (int c = 0; c < maxCols; c++)
                previewText += dt.Columns[c].ColumnName + "\t";
            previewText += "\n";

            // Datos
            for (int r = 0; r < maxRows; r++)
            {
                for (int c = 0; c < maxCols; c++)
                {
                    previewText += dt.Rows[r][c]?.ToString() + "\t";
                }
                previewText += "\n";
            }

            return previewText;
        }

        // Nuevo método para consultas ejecutadas solo desde botones separados
        private async Task EjecutarConsultaNuevaAsync(Func<DataTable> consulta)
        {
            if (_previewText == null)
                return;

            try
            {
                _previewText.Text = "Cargando vista previa...";

                var dt = consulta();

                if (dt == null || dt.Rows.Count == 0)
                {
                    _previewText.Text = "No hay datos para mostrar.";
                    return;
                }

                _previewText.Text = ConstruirVistaPrevia(dt);
            }
            catch (Exception ex)
            {
                _previewText.Text = $"Error al cargar la vista previa: {ex.Message}";
            }
        }

        private async void OnGeneratePdfClick(object? sender, RoutedEventArgs e)
        {
            if (_reportTypeCombo == null)
                return;

            try
            {
                var selectedReport = ((_reportTypeCombo.SelectedItem as ComboBoxItem)?.Content as string) ?? "";

                string titulo = "";
                string nombreArchivo = "";
                DataTable? dt = null;

                switch (selectedReport)
                {
                    case "Equipos más prestados":
                        titulo = "Equipos más prestados";
                        nombreArchivo = "EquiposMasPrestados.pdf";
                        dt = _pdfQueries.GetEquiposMasPrestados();
                        break;
                    case "Usuarios con más préstamos":
                        titulo = "Usuarios con más préstamos";
                        nombreArchivo = "UsuariosConMasPrestamos.pdf";
                        dt = _pdfQueries.GetUsuariosConMasPrestamos();
                        break;
                    case "Reporte de equipos en reparación":
                        titulo = "Equipos en reparación";
                        nombreArchivo = "EquiposEnReparacion.pdf";
                        dt = _pdfQueries.GetEquiposEnReparacion();
                        break;
                    case "Préstamos activos por tipo de equipo":
                        titulo = "Préstamos activos por tipo de equipo";
                        nombreArchivo = "PrestamosActivosPorTipo.pdf";
                        dt = _pdfQueries.GetPrestamosActivosPorTipoEquipo();
                        break;
                    case "Estadísticas de mantenimiento de equipos":
                        titulo = "Estadísticas de mantenimiento de equipos";
                        nombreArchivo = "EstadisticasMantenimiento.pdf";
                        dt = _pdfQueries.GetEstadisticasMantenimiento();
                        break;
                    default:
                        await ShowMessage("Aviso", "Seleccione un reporte válido.");
                        return;
                }

                if (dt == null || dt.Rows.Count == 0)
                {
                    await ShowMessage("Aviso", "No hay datos para generar el reporte.");
                    return;
                }

                var lista = dt.ToDictionaryList();
                var encabezados = new List<string>();
                foreach (DataColumn col in dt.Columns)
                    encabezados.Add(col.ColumnName);

                var pdf = new PDFGenerator(titulo, encabezados, lista);
                pdf.GeneratePdf(nombreArchivo);

                await ShowMessage("Éxito", $"PDF generado: {Path.GetFullPath(nombreArchivo)}");
                pdf.OpenPdfFile(Path.GetFullPath(nombreArchivo));
            }
            catch (Exception ex)
            {
                await ShowError("Error", $"Error al generar el PDF: {ex.Message}");
            }
        }

        private async Task ShowMessage(string title, string message)
        {
            var messageBox = new Window
            {
                Title = title,
                Content = new StackPanel
                {
                    Margin = new Thickness(10),
                    Children =
                    {
                        new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap },
                        new Button { Content = "Aceptar", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center }
                    }
                },
                Width = 350,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            ((Button)((StackPanel)messageBox.Content).Children[1]).Click += (s, e) => messageBox.Close();
            await messageBox.ShowDialog(GetWindow());
        }

        private async Task ShowError(string title, string message)
        {
            var messageBox = new Window
            {
                Title = title,
                Content = new StackPanel
                {
                    Margin = new Thickness(10),
                    Children =
                    {
                        new TextBlock { Text = message, Foreground = new SolidColorBrush(Colors.Red), TextWrapping = TextWrapping.Wrap },
                        new Button { Content = "Aceptar", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center }
                    }
                },
                Width = 350,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            ((Button)((StackPanel)messageBox.Content).Children[1]).Click += (s, e) => messageBox.Close();
            await messageBox.ShowDialog(GetWindow());
        }

        private Window GetWindow() => (Window)this.VisualRoot!;
    }
}
