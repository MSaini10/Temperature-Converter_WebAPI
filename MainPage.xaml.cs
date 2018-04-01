using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Temperature_Converter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private SolidColorBrush errorBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private Brush correctBrush = null;
        SysOfUnits u;
        static readonly string[] abZero = { "0 K", "-273.15  \u00b0C", "-459.67  \u00b0F" };
        private bool playSpeech;
        private SpeechUtil talker;

        public MainPage()
        {
            this.InitializeComponent();
            if (correctBrush == null)
                correctBrush = txtInput.Foreground;
            u = SysOfUnits.Celsius;
            playSpeech = false;
            speechIcon.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets/speakerMuted.png"));
            talker = new SpeechUtil();
        }

        private void txtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtInput.Foreground = correctBrush;
        }

        private void ddlUnits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = (ComboBox)sender;
            var item = (ComboBoxItem)combo.SelectedItem;
            //Next line of code is a trick to convert the string content
            //of the selected item into the emun.
            u = (SysOfUnits)Enum.Parse(typeof(SysOfUnits), item.Content.ToString());
            switch (u)
            {
                case SysOfUnits.Celsius:
                    calculateButton.Background = new SolidColorBrush(Windows.UI.Colors.DarkRed);
                    break;
                case SysOfUnits.Fahrenheit:
                    calculateButton.Background = new SolidColorBrush(Windows.UI.Colors.DarkGreen);
                    break;
                case SysOfUnits.Kelvin:
                    calculateButton.Background = new SolidColorBrush(Windows.UI.Colors.DarkBlue);
                    break;
                default:
                    break;
            }
            CalculateTemp();
        }
        private async void CalculateTemp()
        {
            if (!String.IsNullOrEmpty(txtInput.Text))
            {
                try
                {
                    decimal t;
                    if (!Decimal.TryParse(txtInput.Text, out t))
                    {
                        txtInput.Foreground = errorBrush;
                    }
                    else
                    {
                        TempConverter c = new TempConverter(t, u);
                        moveCelsius.To = (double)c.InCelsius;
                        moveFahrenheit.To = (double)c.InFahrenheit;
                        moveKelvin.To = (double)c.InKelvin;
                        myStoryboard.Begin();
                        if (playSpeech)
                        {
                            await readResults(c);
                        }
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    string strMessage = "Input Temperature cannot be below Absolute Zero ( " + abZero[(int)u] + " )";
                    App.ShowMessage("Physics Error", strMessage);
                    txtInput.Foreground = correctBrush;
                    txtInput.Text = "";
                    if (playSpeech)
                    {
                        await talker.SpeakTextAsync(strMessage, uiMediaElement);
                    }
                }
                catch (Exception ex)
                {
                    App.ShowMessage("Calculation Error", ex.Message);
                    txtInput.Foreground = correctBrush;
                    txtInput.Text = "";
                    if (playSpeech)
                    {
                        await talker.SpeakTextAsync(ex.Message, uiMediaElement);
                    }
                }
            }
        }

        private async Task readResults(TempConverter c)
        {
            string strMessage = "";
            switch (u)
            {
                case SysOfUnits.Kelvin:
                    strMessage += c.InKelvin.ToString() + " Kelvin is equivalent to " +
                        c.InCelsius.ToString() + " degrees Celsius and " +
                        c.InFahrenheit.ToString() + " degrees Fahrenheit";
                    break;
                case SysOfUnits.Celsius:
                    strMessage += c.InCelsius.ToString() + " degrees Celsius is equivalent to " +
                        c.InFahrenheit.ToString() + " degrees Fahrenheit and " +
                        c.InKelvin.ToString() + " Kelvin";
                    break;
                case SysOfUnits.Fahrenheit:
                    strMessage += c.InFahrenheit.ToString() + " degrees Fahrenheit is equivalent to " +
                        c.InCelsius.ToString() + " degrees Celsius and " +
                        c.InKelvin.ToString() + " Kelvin";
                    break;
                default:
                    break;
            }
            await talker.SpeakTextAsync(strMessage, uiMediaElement);
        }

        private void toggleSpeech_Tapped(object sender, TappedRoutedEventArgs e)
        {
            playSpeech = !playSpeech;
            if (playSpeech)
            {
                speechIcon.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets/speaker.png"));
            }
            else
            {
                speechIcon.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets/speakerMuted.png"));
                uiMediaElement.Stop();//Stop the sound if playing.
            }
        }

        private void calculateButton_Click(object sender, RoutedEventArgs e)
        {
            CalculateTemp();
        }
    }
}
