using GetupMonitor.ViewModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GetupMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MonitorWindow : Window
    {       
        public MonitorWindow()
        {            
            InitializeComponent();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //GetupMonitor_VM.StateMachineTrigger = false;
        }
        private void TBstable_KeyEnterUpdate(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtMinA0.Focus();
            }
        }
        private void TBstable_LostFocus(object sender, RoutedEventArgs e)
        {
            BindingExpression binding = BindingOperations.GetBindingExpression(txtStable, TextBox.TextProperty);
            if (binding != null) { binding.UpdateSource(); }
        }

        private void TBminA0_KeyEnterUpdate(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtMaxA0.Focus();
            }
        }
        private void TBminA0_LostFocus(object sender, RoutedEventArgs e)
        {
            BindingExpression binding = BindingOperations.GetBindingExpression(txtMinA0, TextBox.TextProperty);
            if (binding != null) { binding.UpdateSource(); }
        }

        private void TBmaxA0_KeyEnterUpdate(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtMinA1.Focus();
            }
        }
        private void TBmaxA0_LostFocus(object sender, RoutedEventArgs e)
        {
            BindingExpression binding = BindingOperations.GetBindingExpression(txtMaxA0, TextBox.TextProperty);
            if (binding != null) { binding.UpdateSource(); }
        }

        private void TBminA1_KeyEnterUpdate(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtMaxA1.Focus();
            }
        }
        private void TBminA1_LostFocus(object sender, RoutedEventArgs e)
        {
            BindingExpression binding = BindingOperations.GetBindingExpression(txtMinA1, TextBox.TextProperty);
            if (binding != null) { binding.UpdateSource(); }
        }
        private void TBmaxA1_KeyEnterUpdate(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnRun.Focus();
            }
        }

        private void TBmaxA1_LostFocus(object sender, RoutedEventArgs e)
        {
            BindingExpression binding = BindingOperations.GetBindingExpression(txtMaxA1, TextBox.TextProperty);
            if (binding != null) { binding.UpdateSource(); }
        }
    }

    #region Color
    public class BoolRedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
            {
                {
                    return "#FFBA2E45";
                }
            }
            return "#FF888888";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolGreenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
            {
                {
                    return "#FF29993D";
                }
            }
            return "#FF888888";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GreenRedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((int)value == 1)
                return "#FF29993D"; // Green
            else if ((int)value == -1)
                return "#FFBA2E45"; // Red
            else
                return "#FF888888"; // Gray
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InfoGraphicVisibilityConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
            {
                {
                    return System.Windows.Visibility.Visible;
                }
            }
            return System.Windows.Visibility.Collapsed;
        }
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }

    #endregion
}
