using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Parent
{
    /// <summary>
    /// Interaction logic for ParentProfilePage.xaml
    /// </summary>
    public partial class ParentProfilePage : UserControl
    {
        private User _currentUser;

        public ParentProfilePage(User user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadUserProfile();
        }

        private void LoadUserProfile()
        {
            if (_currentUser != null)
            {
                FullNameTextBlock.Text = _currentUser.FullName ?? "Vui lòng cập nhật";
                PhoneNumberTextBlock.Text = _currentUser.PhoneNumber;
                EmailTextBlock.Text = _currentUser.EmailAddress ?? "Vui lòng cập nhật";
                AddressTextBlock.Text = _currentUser.Address ?? "Vui lòng cập nhật";
                DoBTextBlock.Text = _currentUser.DayOfBirth?.ToString("dd/MM/yyyy") ?? "Vui lòng cập nhật";
                string avatarPath = string.IsNullOrWhiteSpace(_currentUser.AvatarUrl)
                        ? "pack://application:,,,/Resources/default-avatar.png"
                        : _currentUser.AvatarUrl;

                try
                {
                    AvatarUrl.Source = new BitmapImage(new Uri(avatarPath, UriKind.RelativeOrAbsolute));
                }
                catch
                {
                    AvatarUrl.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/avatar.png"));
                }
            }
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
