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
using System.IO;
using Microsoft.Win32;

namespace TEXT_RED
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DocumentManager _documentManager;
        public MainWindow()
        {
            InitializeComponent();

            _documentManager = new DocumentManager(MainTextBox);

            ComboFontType.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            ComboFontSize.ItemsSource = new List<double>() {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 52, 64, 72, 88, 95, 100, 110 };
        }

        private void comboFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboFontType.SelectedItem != null)
                MainTextBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, ComboFontType.SelectedItem);
        }

        private void Size_Change(object sender, TextChangedEventArgs e)
        {
            MainTextBox.Selection.ApplyPropertyValue(Inline.FontSizeProperty, ComboFontSize.Text);
        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Muchacha Text Format (*.mch)|*.mch|Все файлы (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open);
                TextRange range = new TextRange(MainTextBox.Document.ContentStart, MainTextBox.Document.ContentEnd);
                range.Load(fileStream, DataFormats.Rtf);
            }
        }

        private void FileSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Muchacha Text Format (*.mch)|*.mch|Все файлы (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create);
                TextRange range = new TextRange(MainTextBox.Document.ContentStart, MainTextBox.Document.ContentEnd);
                range.Save(fileStream, DataFormats.Rtf);
            }
        }

        private void MainTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {

            //тут выводит положение курсора
            TextPointer tp1 = MainTextBox.Selection.Start.GetLineStartPosition(0);
            TextPointer tp2 = MainTextBox.Selection.Start;
            int column = tp1.GetOffsetToPosition(tp2);
            int someBigNumber = int.MaxValue;
            int lineMoved, currentLineNumber;
            MainTextBox.Selection.Start.GetLineStartPosition(-someBigNumber, out lineMoved);
            currentLineNumber = -lineMoved;
            lblCursorPosition.Text = "Ряд: " + currentLineNumber.ToString() + " Символ: " + column.ToString();


            // тут выполняется применение стиля шрифта будь то жирный, курсив или подчеркнутый
            object temp = MainTextBox.Selection.GetPropertyValue(Inline.FontWeightProperty);
            CheckBoldFont.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));
            temp = MainTextBox.Selection.GetPropertyValue(Inline.FontStyleProperty);
            CheckItalicFont.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));
            temp = MainTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            CheckUnderlineFont.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

            //тут положение текста
            RadioLeft.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextAlignment.Left));
            RadioCenter.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextAlignment.Center));
            RadioRight.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextAlignment.Right));
            RadioJust.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextAlignment.Justify));

            //тут применяется шрифт и размер шрифта
            temp = MainTextBox.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            ComboFontType.SelectedItem = temp;

            if (!ComboFontSize.Items.Equals(""))
            {
                temp = MainTextBox.Selection.GetPropertyValue(Inline.FontSizeProperty);
                ComboFontSize.Text = temp.ToString();
            }
            else ComboFontSize.SelectedIndex = 0;
        }

        private void Highlight_Click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush scb = new SolidColorBrush();
            _documentManager.ApplyToSelection(TextBlock.ForegroundProperty, new SolidColorBrush(colorPicker()));
        }


        private System.Windows.Media.Color colorPicker()
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            colorDialog.AllowFullOpen = true;
            colorDialog.ShowDialog();

            System.Windows.Media.Color col = new System.Windows.Media.Color();
            col.A = colorDialog.Color.A;
            col.B = colorDialog.Color.B;
            col.G = colorDialog.Color.G;
            col.R = colorDialog.Color.R;

            System.Windows.Media.Color co = new System.Windows.Media.Color();
            co.A = colorDialog.Color.A;

            return col;
        }

        private void Color_Click(object sender, RoutedEventArgs e)
        {
            _documentManager.ApplyToSelection(TextBlock.BackgroundProperty, new SolidColorBrush(colorPicker()));
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
