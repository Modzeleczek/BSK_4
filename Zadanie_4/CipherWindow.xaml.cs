using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                var lfsr = PrepareLFSR(PolynomialTextBox.Text, SeedTextBox.Text);
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

        private LinearFeedbackShiftRegister PrepareLFSR(string polynomialStr, string seedStr)
        {
            var lfsr = new LinearFeedbackShiftRegister(ParsePolynomialString(polynomialStr));
            var seedBools = LinearFeedbackShiftRegister.BitStringToBitBools(seedStr);
            Reverse(seedBools); // LFSR wpisuje seed od prawej do lewej, więc odwracamy nasz seed zapisany od lewej do prawej
            lfsr.Set(seedBools); // jeżeli seed będzie za długi, to zostanie obcięty z lewej; jeżeli będzie za krótki, to LFSR zostanie dopełniony zerami
            return lfsr;
        }

        private int[] ParsePolynomialString(string str)
        {
            var builder = new StringBuilder();
            foreach (var c in str)
                if (c == ';' || c == '-' || (c >= '0' && c <= '9'))
                    builder.Append(c);
            var split = builder.ToString().Split(';');
            var list = new LinkedList<int>();
            foreach (var s in split)
                if (int.TryParse(s, out int number))
                    list.AddLast(number);
            return list.ToArray();
        }

        private void Reverse<T>(T[] array)
        {
            for (int i = 0; i < array.Length / 2; ++i)
            {
                T temp = array[array.Length - 1 - i];
                array[array.Length - 1 - i] = array[i];
                array[i] = temp;
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
