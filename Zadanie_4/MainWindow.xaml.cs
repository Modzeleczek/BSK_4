using System.Windows;

namespace Zadanie_4
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
        private void GeneratorWindowOpen(object sender, RoutedEventArgs e)
        {
            GeneratorWindow subWindow = new GeneratorWindow();
            subWindow.Show();
            this.Close();
        }

        private void CipherWindowOpen(object sender, RoutedEventArgs e)
        {
            CipherWindow subWindow = new CipherWindow();
            subWindow.Show();
            this.Close();
        }
    }
}
