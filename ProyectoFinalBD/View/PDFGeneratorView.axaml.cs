using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Threading.Tasks;
using Avalonia.Media;
using ProyectoFinalBD.util;
using Avalonia.Markup.Xaml;

namespace ProyectoFinalBD.View;

public partial class PDFGeneratorView : UserControl
{
    private readonly PDFQueries _pdfQueries;
    private ComboBox? _reportTypeCombo;
    private Button? _generatePdfButton;
    private TextBlock? _previewText;

    public PDFGeneratorView()
    {
        InitializeComponent();
        _pdfQueries = new PDFQueries();

        _reportTypeCombo = this.FindControl<ComboBox>("ReportTypeCombo");
        _generatePdfButton = this.FindControl<Button>("GeneratePdfButton");
        _previewText = this.FindControl<TextBlock>("PreviewText");

        if (_reportTypeCombo != null && _generatePdfButton != null)
        {
            _reportTypeCombo.SelectionChanged += OnReportTypeChanged;
            _generatePdfButton.Click += OnGeneratePdfClick;
        }
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
                UpdatePreview();
            }
        }
    }
    
    private async void UpdatePreview()
    {
        try
        {
            if (_reportTypeCombo != null && _previewText != null)
            {
                var selectedReport = ((ComboBoxItem)_reportTypeCombo.SelectedItem!)?.Content.ToString();
                _previewText.Text = "Cargando vista previa...";
                
                switch (selectedReport)
                {
                    case "Equipos más prestados":
                       // _previewText.Text = await _pdfQueries.GetMostLoanedEquipmentPreview();
                        break;
                    case "Usuarios con más préstamos":
                        //_previewText.Text = await _pdfQueries.GetUsersWithMostLoansPreview();
                        break;
                    case "Reporte de equipos en reparación":
                       // _previewText.Text = await _pdfQueries.GetEquipmentInRepairPreview();
                        break;
                    case "Préstamos activos por tipo de equipo":
                      //  _previewText.Text = await _pdfQueries.GetActiveLoansPerTypePreview();
                        break;
                    case "Estadísticas de mantenimiento de equipos":
                       // _previewText.Text = await _pdfQueries.GetMaintenanceStatisticsPreview();
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            if (_previewText != null)
                _previewText.Text = $"Error al cargar la vista previa: {ex.Message}";
        }
    }
    
    private async void OnGeneratePdfClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_reportTypeCombo != null)
            {
                var selectedReport = ((ComboBoxItem)_reportTypeCombo.SelectedItem!)?.Content.ToString();
                
                switch (selectedReport)
                {
                    case "Equipos más prestados":
                       // await _pdfQueries.GenerateMostLoanedEquipmentPDF();
                        break;
                    case "Usuarios con más préstamos":
                        //await _pdfQueries.GenerateUsersWithMostLoansPDF();
                        break;
                    case "Reporte de equipos en reparación":
                       // await _pdfQueries.GenerateEquipmentInRepairPDF();
                        break;
                    case "Préstamos activos por tipo de equipo":
                       //await _pdfQueries.GenerateActiveLoansPerTypePDF();
                        break;
                    case "Estadísticas de mantenimiento de equipos":
                        //await _pdfQueries.GenerateMaintenanceStatisticsPDF();
                        break;
                }
                
                await ShowMessage("Éxito", "El PDF se ha generado correctamente.");
            }
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
                Children =
                {
                    new TextBlock { Text = message },
                    new Button { Content = "Aceptar" }
                }
            },
            Width = 250,
            Height = 100,
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
                Children =
                {
                    new TextBlock { Text = message, Foreground = new SolidColorBrush(Colors.Red) },
                    new Button { Content = "Aceptar" }
                }
            },
            Width = 250,
            Height = 100,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        ((Button)((StackPanel)messageBox.Content).Children[1]).Click += (s, e) => messageBox.Close();
        await messageBox.ShowDialog(GetWindow());
    }
    
    private Window GetWindow() => (Window)this.VisualRoot!;
}