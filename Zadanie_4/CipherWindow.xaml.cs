using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Zadanie_4
{
    /// <summary>
    /// Interaction logic for CipherWindow.xaml
    /// </summary>
    public partial class CipherWindow : Window
    {
        private SynchronousStreamCipher Cipher;

        public CipherWindow()
        {
            InitializeComponent();
            Cipher = new SynchronousStreamCipher();
            this.Closing += new CancelEventHandler((sender, e) =>
            {
                MainWindow window = new MainWindow();
                window.Show();
            });
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Wielomian\n" +
                "w dowolnej kolejności wykładniki (potęgi) wielomianu; począwszy od 1 do n (n - liczba bitów LFSR); oddzielone średnikami; np. 1;4;3\n\n" +
                "Seed\n" +
                "początkowy ciąg wpisany do LFSR; składa się tylko ze znaków '0' i '1'; znaki przepisywane od lewej do prawej; np. 0110 (dla LFSR mającego długość 4 bitów, czyli opisanego wielomianem o największym wykładniku równym 4)");
        }

        private void PerformButton_Click(object sender, RoutedEventArgs e)
        {
            int operationIndex = CheckedIndex((OperationGroupBox.Content as StackPanel).Children);
            try
            {
                var lfsr = new LinearFeedbackShiftRegister(PolynomialTextBox.Text, SeedTextBox.Text);
                if (operationIndex == 0) // szyfrowanie
                    OutputTextBox.Text = Cipher.Encrypt(InputTextBox.Text, lfsr);
                else if (operationIndex == 1) // deszyfrowanie
                    OutputTextBox.Text = Cipher.Decrypt(InputTextBox.Text, lfsr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private int CheckedIndex(UIElementCollection radioButtons)
        {
            int index = 0;
            foreach (var rb in radioButtons)
            {
                if ((bool)(rb as RadioButton).IsChecked)
                    break;
                ++index;
            }
            return index;
        }
    }
}
