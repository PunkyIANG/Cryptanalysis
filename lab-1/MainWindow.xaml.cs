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
        private KeyValuePair<char, int>[] _textChars;
        private TextBox[] _overrideChars;
        private FontFamily consolas = new FontFamily("Consolas");
        private bool isPolyMode = false;

        private List<string> splitStrings;
        private List<Button> splitButtons;
        private int currentSplit;

        private readonly Dictionary<char, double> englishLetterDistribution = new Dictionary<char, double>()
        {
            {'E', 	0.1202},
            {'T', 	0.0910},
            {'A', 	0.0812},
            {'O', 	0.0768},
            {'I', 	0.0731},
            {'N', 	0.0695},
            {'S', 	0.0628},
            {'R', 	0.0602},
            {'H', 	0.0592},
            {'D', 	0.0432},
            {'L', 	0.0398},
            {'U', 	0.0288},
            {'C', 	0.0271},
            {'M', 	0.0261},
            {'F', 	0.0230},
            {'Y', 	0.0211},
            {'W', 	0.0209},
            {'G', 	0.0203},
            {'P', 	0.0182},
            {'B', 	0.0149},
            {'V', 	0.0111},
            {'K', 	0.0069},
            {'X', 	0.0017},
            {'Q', 	0.0011},
            {'J', 	0.0010},
            {'Z', 	0.0007}
        };

        public MainWindow()
        {
            InitializeComponent();

            // PolyAlphabetSolver.SplitToMonoAlphabet("KTPCZNOOGHVFBTZVSBIOVTAGMKRLVAKMXAVUSTTPCNLCDVHXEOCPECPPHXHLNLFCKNYBPSQVXYPVHAKTAOLUHTITPDCSBPAJEAQZRIMCSYIMJHRABPPPHBUSKVXTAJAMHLNLCWZVSAQYVOYDLKNZLHWNWKJGTAGKQCMQYUWXTLRUSBSGDUAAJEYCJVTACAKTPCZPTJWPVECCBPDBELKFBVIGCTOLLANPKKCXVOGYVQBNDMTLCTBVPHIMFPFNMDLEOFGQCUGFPEETPKYEGVHYARVOGYVQBNDWKZEHTTNGHBOIWTMJPUJNUADEZKUUHHTAQFCCBPDBELCLEVOGTBOLEOGHBUEWVOGM ", 5);
        }
        
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

            if (isPolyMode)
                MonoToPolyPanel.Visibility = Visibility.Visible;
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

            if (isPolyMode)
                splitStrings[currentSplit] = clearText;

            MonoClearText.Text = clearText;
        }

        private void ExtrapolateChars(object sender, RoutedEventArgs e)
        {
            int charCount = 0;
            const int letterCount = 26;

            double[] deviations = new double[letterCount];

            foreach (var (key, value) in _textChars)
                charCount += value;

            for (int shift = 0; shift < letterCount; shift++)
            {
                double shiftDeviation = 0;

                for (var key = 'A'; key <= 'Z'; key++)
                {
                    int value = 0;
                    
                    char index = (char) ((key + shift - 'A' + 26) % 26 + 'A');

                    foreach (var (c, val) in _textChars)
                        if (c == key)
                        {
                            value = val;
                            break;
                        }
                    

                    var letterDeviation = (double) value / charCount - englishLetterDistribution[index];
                    
                    
                    letterDeviation *= letterDeviation;

                    shiftDeviation += letterDeviation;
                }

                deviations[shift] = shiftDeviation;
                
            }

            var min = deviations[0];
            var bestShift = 0;

            for (int i = 0; i < deviations.Length; i++)
            {
                if (min > deviations[i])
                {
                    min = deviations[i];
                    bestShift = i;
                }
            }
            
            
            Console.WriteLine("({0}, {1})", bestShift, min);

            for (int i = 0; i < _overrideChars.Length; i++)
            {
                _overrideChars[i].Text = ((char) ((_textChars[i].Key + bestShift - 'A' + 26) % 26 + 'A')).ToString().ToLower();
            }
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
                PolyKeyLengthResult.Text = "No key length found";
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
            splitStrings = PolyAlphabetSolver.SplitToMonoAlphabet(PolyEncryptedText.Text, length);
            splitButtons = new List<Button>();
            
            var stackpanel = new StackPanel();

            for (var index = 0; index < splitStrings.Count; index++)
            {
                var text = splitStrings[index];
                var button = new Button
                {
                    Content = text,
                    Width = MeasureString(text).Width + 10,
                    FontFamily = consolas,
                    Tag = index.ToString()
                };
                button.Click += SplitTextToMono;

                splitButtons.Add(button);
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
            MonoClearText.Text = "";
            RootContent.Content = "";
            MonoToPolyPanel.Visibility = Visibility.Hidden;
            
            var button = sender as Button;
            var text = button?.Content as string;
            currentSplit = int.Parse(button?.Tag as string);
            
            Dispatcher.BeginInvoke((Action)(() => RootTabControl.SelectedIndex = 0));
            MonoEncryptedText.Text = text ?? "";
            isPolyMode = true;
        }


        private void ApplyToPoly(object sender, RoutedEventArgs e)
        {
            ReplaceChars(null, null);
            splitButtons[currentSplit].Content = MonoClearText.Text;
            
            Dispatcher.BeginInvoke((Action)(() => RootTabControl.SelectedIndex = 1));

            PolyClearText.Text = PolyAlphabetSolver.MonoToPoly(splitStrings);
        }

        private void ReturnToMono(object sender, RoutedEventArgs e)
        {
            isPolyMode = false;
            MonoEncryptedText.Text = string.Empty;
            MonoClearText.Text = string.Empty;
            MonoToPolyPanel.Visibility = Visibility.Hidden;
            RootContent.Content = string.Empty;
        }

        private void ExtrapolateCaesar(object sender, RoutedEventArgs e)
        {
            int i = 0;

            while (_overrideChars[i].Text == string.Empty)
            {
                i++;
                
                if (i >= _overrideChars.Length)
                    return;
            }
            
            

            var diff = _overrideChars[i].Text.ToUpper()[0] - _textChars[i].Key;

            for (int j = 0; j < _overrideChars.Length; j++)
            {
                if (_overrideChars[j].Text == string.Empty)
                {
                    int character = char.ToLower(_textChars[j].Key) + diff;
                    
                    if (character > 'z')
                        character -= 26;
                    else if (character < 'a')
                        character += 26;


                    _overrideChars[j].Text = ((char)character).ToString();
                }
            }
        }
    }
}
