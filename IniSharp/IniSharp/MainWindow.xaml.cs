using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IniSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            IniFile ini = new IniFile("test.ini");
            ini.SetValue("Apple", "Name", "iPhone");
            ini.SetValue("Apple", "Version", "15");
            ini.SetValue("Sony", "Name", "Xperia");
            ini.SetValue("Sony", "Version", "1");

            ini.TryGetValueOrDafault("Apple", "Name", out string? appleName);
            ini.TryGetValueOrDafault("Apple", "Version", out int? appleVersion);
            ini.TryGetValueOrDafault("Sony", "Name", out string? sonyName);
            ini.TryGetValueOrDafault("Sony", "Version", out int? sonyVersion);

            this.label.Content = $"Product: {appleName} {appleVersion} / Product: {sonyName} {sonyVersion}";
        }
    }
}