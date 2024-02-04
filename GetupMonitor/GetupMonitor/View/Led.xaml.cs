using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GetupMonitor.View
{
    /// <summary>
    /// Led.xaml 的互動邏輯
    /// </summary>
    public partial class Led : UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Led()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handle size changing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void layoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // I don't like that I have to use code behind, but this seems to be the best way to ensure square size
            double size = Math.Min(e.NewSize.Height, e.NewSize.Width);
            ((Grid)sender).Width = size;
            ((Grid)sender).Height = size;
        }

        /// <summary>
        /// Gets or sets the bezel status
        /// </summary>
        public bool ShowBezel
        {
            get
            {
                return _ShowBezel;
            }
            set
            {
                _ShowBezel = value;

                if (_ShowBezel)
                {
                    ledTransform.ScaleX = 0.70;
                    ledTransform.ScaleY = 0.70;
                    hightlightTransform.ScaleX = 0.5;
                    hightlightTransform.ScaleY = 0.4;
                    Highlight.RenderTransformOrigin = new Point(0.5, 0.3);
                }
                else
                {
                    ledTransform.ScaleX = 1;
                    ledTransform.ScaleY = 1;
                    hightlightTransform.ScaleX = 0.7;
                    hightlightTransform.ScaleY = 0.55;
                    Highlight.RenderTransformOrigin = new Point(0.5, 0.1);
                }
            }
        }
        private bool _ShowBezel = true;

        #region On Dependency Property
        /// <summary>
        /// Gets or sets the on status
        /// </summary>
        public bool On
        {
            get { return (Boolean)this.GetValue(OnProperty); }
            set { this.SetValue(OnProperty, value); }
        }
        public static readonly DependencyProperty OnProperty = DependencyProperty.Register(
            "On",
            typeof(Boolean),
            typeof(Led),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnOnChanged)
                )
            );
        private static void OnOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Led led = (Led)d;
            if (Convert.ToBoolean(e.NewValue))
                led.Light.Fill = new SolidColorBrush(led.OnColor);
            else
                led.Light.Fill = new SolidColorBrush(led.OffColor);
        }
        #endregion

        #region OnColor Dependency Property
        public Color OnColor
        {
            get { return (Color)this.GetValue(OnColorProperty); }
            set { this.SetValue(OnColorProperty, value); }
        }
        public static readonly DependencyProperty OnColorProperty = DependencyProperty.Register(
            "OnColor",
            typeof(Color),
            typeof(Led),
            new FrameworkPropertyMetadata(
                Color.FromArgb(0xFF, 0x00, 0x45, 0x76),
                FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnOnColorChanged)
                )
            );
        private static void OnOnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Led led = (Led)d;
            if (led.On)
                led.Light.Fill = new SolidColorBrush(led.OnColor);
        }
        #endregion

        #region ManualOverride Dependency Property
        public bool ManualOverride
        {
            get { return (Boolean)this.GetValue(ManualOverrideProperty); }
            set { this.SetValue(ManualOverrideProperty, value); }
        }
        public static readonly DependencyProperty ManualOverrideProperty = DependencyProperty.Register(
            "ManualOverride",
            typeof(Boolean),
            typeof(Led),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnManualOverrideChanged)
                )
            );
        private static void OnManualOverrideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Led led = (Led)d;
            if (Convert.ToBoolean(e.NewValue))
            {
                led.gs_A.Color = Color.FromArgb(0xff, 0xf9, 0xb5, 0x2c);
                led.gs_B.Color = Color.FromArgb(0xff, 0x84, 0x66, 0x2b);
                led.gs_C.Color = Color.FromArgb(0xff, 0x60, 0x45, 0x0f);
                led.gs_D.Color = Color.FromArgb(0xff, 0xb2, 0x77, 0x00);
            }
            else
            {
                led.gs_A.Color = Color.FromArgb(0xff, 0xef, 0xef, 0xef);
                led.gs_B.Color = Color.FromArgb(0xff, 0x7f, 0x7f, 0x7f);
                led.gs_C.Color = Color.FromArgb(0xff, 0x3f, 0x3f, 0x3f);
                led.gs_D.Color = Color.FromArgb(0xff, 0xbf, 0xbf, 0xbf);
            }
        }
        #endregion

        #region OffColor Dependency Property
        public Color OffColor
        {
            get { return (Color)this.GetValue(OffColorProperty); }
            set { this.SetValue(OffColorProperty, value); }
        }
        public static readonly DependencyProperty OffColorProperty = DependencyProperty.Register(
            "OffColor",
            typeof(Color),
            typeof(Led),
            new FrameworkPropertyMetadata(
                Color.FromArgb(0xFF, 0xD1, 0xD8, 0xEF),
                FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnOffColorChanged)
                )
            );
        private static void OnOffColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Led led = (Led)d;
            if (!led.On)
                led.Light.Fill = new SolidColorBrush(led.OffColor);
        }
        #endregion

    }
}
