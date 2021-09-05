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
        public MainWindow()
        {
            InitializeComponent();
        }

        private KeyValuePair<char, int>[] _textChars;
        private TextBox[] _overrideChars;

        private void FindFrequency(object sender, RoutedEventArgs e)
        {
            _textChars = MonoAlphabetSolver.GetUniqueCharacters(EncryptedText.Text);

            var characterGrid = new Grid();
            
            var charSum = 0;

            foreach (var keyValuePair in _textChars)
            {
                characterGrid.ColumnDefinitions.Add(new ColumnDefinition());
                charSum += keyValuePair.Value;
            }

            _overrideChars = new TextBox[_textChars.Length];

            for (var i = 0; i < 4; i++)
                characterGrid.RowDefinitions.Add(new RowDefinition());

            for (int i = 0; i < _textChars.Length; i++)
            {
                var keyValuePair = _textChars[i];
                
                var characterTextBlock = new TextBlock
                {
                    Text = keyValuePair.Key.ToString(),
                    Style = (Style) Resources["CharacterTextBlock"]
                };
                
                Grid.SetRow(characterTextBlock, 0);
                Grid.SetColumn(characterTextBlock, i);

                characterGrid.Children.Add(characterTextBlock);


                var freqNumTextBlock = new TextBlock
                {
                    Text = keyValuePair.Value.ToString(),
                    Style = (Style) Resources["CharacterTextBlock"]
                };
                
                Grid.SetRow(freqNumTextBlock, 1);
                Grid.SetColumn(freqNumTextBlock, i);

                characterGrid.Children.Add(freqNumTextBlock);
                
                var freqPerTextBlock = new TextBlock
                {
                    Text = (100f * keyValuePair.Value / charSum).ToString("0.0"),
                    Style = (Style) Resources["CharacterTextBlock"]
                };

                Grid.SetRow(freqPerTextBlock, 2);
                Grid.SetColumn(freqPerTextBlock, i);

                characterGrid.Children.Add(freqPerTextBlock);


                var replacementTextBox = new TextBox
                {
                    Name = keyValuePair.Key + "replacement",
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
            var clearText = EncryptedText.Text;
            
            for (int i = 0; i < _textChars.Length; i++)
            {
                if (_overrideChars[i].Text != String.Empty)
                {
                    clearText = clearText.Replace(_textChars[i].Key.ToString(), _overrideChars[i].Text);
                }
            }

            ClearText.Text = clearText;
        }

        private void AddSpaces(object sender, RoutedEventArgs e)
        {
            var text = EncryptedText.Text;
            text = text.Replace( " ", string.Empty);
            
            // TODO: actually add spaces
            
            EncryptedText.Text = text;
        }
        
        private void RestrictAlphabet(object sender, TextCompositionEventArgs e)
        {
            var alphabetRegex = new Regex("[a-zA-Z]");
            e.Handled = !alphabetRegex.IsMatch(e.Text);
        }
    }
}
