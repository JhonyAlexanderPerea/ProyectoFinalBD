using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ProyectoFinalBD.View
{
    public partial class Calculator : UserControl
    {
        private double numeroAnterior = 0;
        private string operadorActual = "";
        private bool iniciarNuevoNumero = true;
        private TextBox displayTextBox;

        public Calculator()
        {
            InitializeComponent();
            
            displayTextBox = this.FindControl<TextBox>("DisplayTextBox");
            
            // Conectar eventos de botones numéricos
            for (int i = 0; i <= 9; i++)
            {
                var button = this.FindControl<Button>($"Btn{i}");
                if (button != null)
                    button.Click += NumeroButton_Click;
            }
            
            // Conectar eventos de operadores
            this.FindControl<Button>("BtnPlus").Click += OperadorButton_Click;
            this.FindControl<Button>("BtnMinus").Click += OperadorButton_Click;
            this.FindControl<Button>("BtnMult").Click += OperadorButton_Click;
            this.FindControl<Button>("BtnDiv").Click += OperadorButton_Click;
            this.FindControl<Button>("BtnEquals").Click += IgualButton_Click;
            this.FindControl<Button>("BtnDot").Click += PuntoButton_Click;
            this.FindControl<Button>("BtnClear").Click += ClearButton_Click;
            
            displayTextBox.Text = "0";
        }

        private void NumeroButton_Click(object sender, RoutedEventArgs e)
        {
            var numero = (sender as Button)?.Content.ToString();
            
            if (iniciarNuevoNumero)
            {
                displayTextBox.Text = numero;
                iniciarNuevoNumero = false;
            }
            else
            {
                displayTextBox.Text += numero;
            }
        }

        private void OperadorButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(displayTextBox.Text, out numeroAnterior))
            {
                operadorActual = (sender as Button)?.Content.ToString() ?? "";
                iniciarNuevoNumero = true;
            }
        }

        private void IgualButton_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(displayTextBox.Text, out double numeroActual))
                return;

            double resultado = 0;

            switch (operadorActual)
            {
                case "+":
                    resultado = numeroAnterior + numeroActual;
                    break;
                case "-":
                    resultado = numeroAnterior - numeroActual;
                    break;
                case "*":
                    resultado = numeroAnterior * numeroActual;
                    break;
                case "/":
                    if (numeroActual != 0)
                        resultado = numeroAnterior / numeroActual;
                    else
                    {
                        displayTextBox.Text = "Error";
                        return;
                    }
                    break;
            }

            displayTextBox.Text = resultado.ToString();
            iniciarNuevoNumero = true;
        }

        private void PuntoButton_Click(object sender, RoutedEventArgs e)
        {
            if (!displayTextBox.Text.Contains("."))
            {
                displayTextBox.Text += ".";
                iniciarNuevoNumero = false;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            displayTextBox.Text = "0";
            numeroAnterior = 0;
            operadorActual = "";
            iniciarNuevoNumero = true;
        }
    }
}