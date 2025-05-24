using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Globalization;

namespace ProyectoFinalBD.View
{
    public partial class Calendar : UserControl
    {
        private DateTime currentDate;
        private CultureInfo culture = new CultureInfo("es-ES");

        public Calendar()
        {
            InitializeComponent();
            currentDate = DateTime.Today;
            
            // Configurar eventos de botones
            var prevButton = this.FindControl<Button>("PrevMonthButton");
            var nextButton = this.FindControl<Button>("NextMonthButton");
            
            prevButton.Click += (s, e) => {
                currentDate = currentDate.AddMonths(-1);
                UpdateCalendar();
            };
            
            nextButton.Click += (s, e) => {
                currentDate = currentDate.AddMonths(1);
                UpdateCalendar();
            };

            UpdateCalendar();
        }

        private void UpdateCalendar()
        {
            // Actualizar encabezado del mes y año
            MonthYearDisplay.Text = currentDate.ToString("MMMM yyyy", culture);
            
            // Actualizar fecha actual
            CurrentDateDisplay.Text = DateTime.Today.ToString("d MMMM yyyy", culture);

            // Limpiar grid existente
            CalendarGrid.Children.Clear();

            // Obtener el primer día del mes
            var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

            // Calcular el día de la semana inicial (0 = Domingo)
            int startDayOfWeek = (int)firstDayOfMonth.DayOfWeek;

            // Crear los botones del calendario
            for (int i = 0; i < 42; i++) // 6 semanas x 7 días
            {
                int dayNumber = i - startDayOfWeek + 1;
                var dayButton = new Button
                {
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
                    Margin = new Thickness(2),
                    MinHeight = 40
                };

                if (dayNumber > 0 && dayNumber <= daysInMonth)
                {
                    dayButton.Content = dayNumber.ToString();
                    
                    // Resaltar día actual
                    if (dayNumber == DateTime.Today.Day && 
                        currentDate.Month == DateTime.Today.Month && 
                        currentDate.Year == DateTime.Today.Year)
                    {
                        dayButton.Background = new SolidColorBrush(Color.Parse("#4CAF50"));
                        dayButton.Foreground = Brushes.White;
                    }
                }
                else
                {
                    dayButton.IsEnabled = false;
                    dayButton.Opacity = 0;
                }

                Grid.SetRow(dayButton, i / 7);
                Grid.SetColumn(dayButton, i % 7);
                CalendarGrid.Children.Add(dayButton);
            }
        }
    }
}