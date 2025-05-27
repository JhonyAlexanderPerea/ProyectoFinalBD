using System;
using System.Collections.Generic;
using System.Diagnostics;
using AvaloniaEdit.Document;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using IDocument = QuestPDF.Infrastructure.IDocument;

namespace ProyectoFinalBD.util;

public class PDFGenerator : IDocument
{
    private readonly string _titulo;
    private readonly List<string> _encabezados;
    private readonly List<Dictionary<string, string>> _datos;

    public PDFGenerator(string titulo, List<string> encabezados, List<Dictionary<string, string>> datos)
    {
        _titulo = titulo;
        _encabezados = encabezados;
        _datos = datos;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(30);

            page.Header()
                .Text(_titulo)
                .FontSize(20)
                .Bold()
                .AlignCenter();

            page.Content()
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        foreach (var _ in _encabezados)
                            columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        foreach (var enc in _encabezados)
                            header.Cell().Text(enc).Bold();
                    });

                    foreach (var row in _datos)
                    {
                        foreach (var enc in _encabezados)
                            table.Cell().Text(row[enc] ?? string.Empty);
                    }
                });

            page.Footer()
                .AlignCenter()
                .Text($"Generado el: {DateTime.Now:g}");
        });
    }

    public void OpenPdfFile(string filePath)
    {
        try
        {
            var process = new Process();

            process.StartInfo = new ProcessStartInfo(filePath)
            {
                UseShellExecute = true // Esto abre el archivo con el programa predeterminado del SO
            };

            process.Start();
        }
        catch (Exception ex)
        {
            // Aquí podrías mostrar mensaje de error o log
            Console.WriteLine("No se pudo abrir el PDF: " + ex.Message);
        }
    }
    
}