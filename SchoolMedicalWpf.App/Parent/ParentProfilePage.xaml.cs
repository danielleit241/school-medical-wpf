using System.Windows;
using System.Windows.Controls;

namespace SchoolMedicalWpf.App.Parent
{
    /// <summary>
    /// Interaction logic for ParentProfilePage.xaml
    /// </summary>
    public partial class ParentProfilePage : UserControl
    {
        public ParentProfilePage()
        {
            InitializeComponent();
        }

        private void UpdateInformationButton_Click(object sender, RoutedEventArgs e)
        {
            ViewPanel.Visibility = Visibility.Collapsed;
            EditPanel.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ViewPanel.Visibility = Visibility.Collapsed;
            ChangePasswordPanel.Visibility = Visibility.Visible;
        }
    }
}
