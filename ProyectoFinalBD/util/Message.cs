using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace ProyectoFinalBD.util;

public class Message
{
    public async Task ShowMessage(Window owner, string title, string message)
    {
        Window dialog = new Window();
        var okButton = new Button
        {
            Content = "OK",
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
        };
        okButton.Click += (_, _) => dialog.Close();

        dialog.Title = title;
        dialog.Width = 300;
        dialog.Height = 150;
        dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        dialog.Content = new StackPanel
        {
            Margin = new Thickness(20),
            Spacing = 20,
            Children =
            {
                new TextBlock
                {
                    Text = message,
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                },
                okButton
            }
        };

        await dialog.ShowDialog(owner);
    }
}