using System;
using System.ComponentModel;
using System.Windows;

namespace Zadanie_4
{
    /// <summary>
    /// Interaction logic for GeneratorWindow.xaml
    /// </summary>
    public partial class GeneratorWindow : Window
    {
        private LinearFeedbackShiftRegister LFSR;

        public GeneratorWindow()
        {
            InitializeComponent();
            this.Closing += new CancelEventHandler((sender, e) =>
            {
                MainWindow window = new MainWindow();
                window.Show();
            });
        }
        private void Back(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void InstantiateLFSR()
        {
            LFSR = new LinearFeedbackShiftRegister(PolynomialTextBox.Text, SeedTextBox.Text);
        }

        private void ResetLFSR(object sender, RoutedEventArgs e)
        {
            try { InstantiateLFSR(); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void Generate(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LFSR == null) InstantiateLFSR();
                char[] chars = new char[32];
                byte[] bytes = new byte[4] { 0, 0, 0, 0 };
                for (int i = 0; i < 32; ++i)
                {
                    bool bit = LFSR.ShiftRight();
                    if (bit == true)
                    {
                        chars[31 - i] = '1';
                        int byteIndex = i / 8, bitInByteIndex = i % 8;
                        // bajty zostaną ustawione w kolejności little-endian - na indeksie 0 tabeli będzie najmniej znaczący
                        bytes[3 - byteIndex] |= (byte)(1 << bitInByteIndex);
                    }
                    else
                        chars[31 - i] = '0';
                }
                BitsTextBox.Text = new string(chars);
                IntTextBox.Text = BitConverter.ToInt32(bytes, 0).ToString();
                FloatTextBox.Text = BitConverter.ToSingle(bytes, 0).ToString();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
