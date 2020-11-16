using CastIntoGeneratorBiz;
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

namespace CastIntoGenerator
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

        private void _Genera(object sender, RoutedEventArgs e)
        {
            _GetCode();
        }
        private void _GeneraClipboard(object sender, RoutedEventArgs e)
        {
            _GetCodeFromClipboard();
        }
        private void _GetCode()
        {
            try
            {
                TextBoxOutput.Document.Blocks.Clear();
                string ans = CastIntoCodeGenerator.GetCode(_StringFromRichTextBox(TextBoxInput));
                if (string.IsNullOrWhiteSpace(ans))
                {
                    _GetCodeFromClipboard();
                    return;
                }
                Clipboard.SetText(ans);
                TextBoxOutput.AppendText(ans);
            }
            catch (Exception ex)
            {
                TextBoxOutput.AppendText(ex.Message);
                TextBoxOutput.AppendText(ex.StackTrace);
            }
        }
        private void _GetCodeFromClipboard()
        {
            string messaggioErrore = "Inserire o copiare le propieta' dal quale generare il metodo";
            try
            {
                TextBoxOutput.Document.Blocks.Clear();
                string ans;
                string cliboard = Clipboard.GetText();
                if (cliboard == messaggioErrore) { return; }
                if (string.IsNullOrWhiteSpace(cliboard))
                {
                    TextBoxOutput.AppendText(messaggioErrore);
                    return;
                }
                else
                {
                    TextBoxInput.AppendText(cliboard);
                    ans = CastIntoCodeGenerator.GetCode(cliboard);
                }
                Clipboard.SetText(ans);
                TextBoxOutput.AppendText(ans);
            }
            catch (Exception)
            {
                TextBoxOutput.AppendText(messaggioErrore);
                return;
            }
        }
        private string _StringFromRichTextBox(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(
                // TextPointer to the start of content in the RichTextBox.
                rtb.Document.ContentStart,
                // TextPointer to the end of content in the RichTextBox.
                rtb.Document.ContentEnd
            );

            // The Text property on a TextRange object returns a string
            // representing the plain text content of the TextRange.
            return textRange.Text;
        }

        private void CheckBox_AdvancedParsing_Checking(object sender, RoutedEventArgs e)
        {
            CastIntoCodeGenerator.AdvancedParsing = (sender as CheckBox).IsChecked ?? false;
        }
    }
}
