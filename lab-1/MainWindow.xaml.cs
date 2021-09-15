using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace lab_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FontFamily consolas = new FontFamily("Consolas");
        
        public MainWindow()
        {
            InitializeComponent();

            // PolyAlphabetSolver.SplitToMonoAlphabet("KTPCZNOOGHVFBTZVSBIOVTAGMKRLVAKMXAVUSTTPCNLCDVHXEOCPECPPHXHLNLFCKNYBPSQVXYPVHAKTAOLUHTITPDCSBPAJEAQZRIMCSYIMJHRABPPPHBUSKVXTAJAMHLNLCWZVSAQYVOYDLKNZLHWNWKJGTAGKQCMQYUWXTLRUSBSGDUAAJEYCJVTACAKTPCZPTJWPVECCBPDBELKFBVIGCTOLLANPKKCXVOGYVQBNDMTLCTBVPHIMFPFNMDLEOFGQCUGFPEETPKYEGVHYARVOGYVQBNDWKZEHTTNGHBOIWTMJPUJNUADEZKUUHHTAQFCCBPDBELCLEVOGTBOLEOGHBUEWVOGM ", 5);
        }

        private KeyValuePair<char, int>[] _textChars;
        private TextBox[] _overrideChars;

        private void FindFrequency(object sender, RoutedEventArgs e)
        {
            _textChars = MonoAlphabetSolver.GetUniqueCharacters(MonoEncryptedText.Text);

            var characterGrid = new Grid();
            
            var charSum = 0;

            foreach (var (key, value) in _textChars)
            {
                characterGrid.ColumnDefinitions.Add(new ColumnDefinition());
                charSum += value;
            }

            _overrideChars = new TextBox[_textChars.Length];

            for (var i = 0; i < 4; i++)
                characterGrid.RowDefinitions.Add(new RowDefinition());

            for (int i = 0; i < _textChars.Length; i++)
            {
                var (key, value) = _textChars[i];
                
                var characterTextBlock = new TextBlock
                {
                    Text = key.ToString(),
                    Style = (Style) Resources["CharacterTextBlock"]
                };
                
                Grid.SetRow(characterTextBlock, 0);
                Grid.SetColumn(characterTextBlock, i);

                characterGrid.Children.Add(characterTextBlock);


                var freqNumTextBlock = new TextBlock
                {
                    Text = value.ToString(),
                    Style = (Style) Resources["CharacterTextBlock"]
                };
                
                Grid.SetRow(freqNumTextBlock, 1);
                Grid.SetColumn(freqNumTextBlock, i);

                characterGrid.Children.Add(freqNumTextBlock);
                
                var freqPerTextBlock = new TextBlock
                {
                    Text = (100f * value / charSum).ToString("0.0"),
                    Style = (Style) Resources["CharacterTextBlock"]
                };

                Grid.SetRow(freqPerTextBlock, 2);
                Grid.SetColumn(freqPerTextBlock, i);

                characterGrid.Children.Add(freqPerTextBlock);


                var replacementTextBox = new TextBox
                {
                    Name = key + "replacement",
                    Style = (Style) Resources["CharacterTextBox"],
                    CharacterCasing = CharacterCasing.Lower
                };

                _overrideChars[i] = replacementTextBox;
                
                Grid.SetRow(replacementTextBox, 3);
                Grid.SetColumn(replacementTextBox, i);

                characterGrid.Children.Add(replacementTextBox);
            }

            RootContent.Content = characterGrid;
            
            ReplaceCharsBtn.IsEnabled = true;
        }

        private void ReplaceChars(object sender, RoutedEventArgs e)
        {
            var clearText = MonoEncryptedText.Text;
            
            for (int i = 0; i < _textChars.Length; i++)
            {
                if (_overrideChars[i].Text != String.Empty)
                {
                    clearText = clearText.Replace(_textChars[i].Key.ToString(), _overrideChars[i].Text);
                }
            }

            MonoClearText.Text = clearText;
        }

        private void AddSpaces(object sender, RoutedEventArgs e)
        {
            var text = MonoEncryptedText.Text;
            text = text.Replace( " ", string.Empty);
            
            // TODO: actually add spaces
            
            MonoEncryptedText.Text = text;
        }
        
        private void RestrictAlphabet(object sender, TextCompositionEventArgs e)
        {
            var alphabetRegex = new Regex("[a-zA-Z]");
            e.Handled = !alphabetRegex.IsMatch(e.Text);
        }

        private void CalcKeyLength(object sender, RoutedEventArgs e)
        {
            var text = PolyEncryptedText.Text;

            var result = PolyAlphabetSolver.GetKeyLength(text);

            if (result.Count == 0)
            {
                PolyKeyLengthResult.Text = "Nu a fost gasita lungimea cheii";
                return;
            }
            
            var stringBuilder = new StringBuilder("Lungimea cheii: ");

            foreach (var i in result)
            {
                stringBuilder.Append(i);
                stringBuilder.Append(" ");
            }

            PolyKeyLengthResult.Text = stringBuilder.ToString();
            PolyKeyLength.Text = result[0].ToString();
        }

        private void SplitText(object sender, RoutedEventArgs e)
        {
            var length = int.Parse(PolyKeyLength.Text);
            var result = PolyAlphabetSolver.SplitToMonoAlphabet(PolyEncryptedText.Text, length);
            
            var stackpanel = new StackPanel();

            foreach (var text in result)
            {
                var button = new Button
                {
                    Content = text,
                    Width = MeasureString(text).Width + 10,
                    FontFamily = consolas
                };
                button.Click += SplitTextToMono;
                
                stackpanel.Children.Add(button);
            }

            PolySplitTextsContainer.Content = stackpanel;
        }
        
        private Size MeasureString(string candidate)
        {
            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(consolas, PolyKeyLengthResult.FontStyle, PolyKeyLengthResult.FontWeight, PolyKeyLengthResult.FontStretch),
                PolyKeyLengthResult.FontSize,
                Brushes.Black,
                new NumberSubstitution(),
                1);

            return new Size(formattedText.Width, formattedText.Height);
        }

        private void SplitTextToMono(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var text = button?.Content as string;
            
            Dispatcher.BeginInvoke((Action)(() => RootTabControl.SelectedIndex = 0));
            MonoEncryptedText.Text = text ?? "";
            MonoReturnPoly.Visibility = Visibility.Visible;
        }


        private void ApplyToPoly(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
