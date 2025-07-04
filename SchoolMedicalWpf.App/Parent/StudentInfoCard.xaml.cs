using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Parent
{
    /// <summary>  
    /// Interaction logic for StudentInfoCard.xaml  
    /// </summary>  
    public partial class StudentInfoCard : UserControl
    {
        public StudentInfoCard()
        {
            InitializeComponent();
        }

        private void DeclarationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var student = DataContext as Student;
                if (student == null)
                {
                    MessageBox.Show("❌ Thông tin học sinh không khả dụng.\n\n" +
                        $"🕐 Thời gian: {DateTime.Now}\n", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Disable button để tránh spam click
                DeclarationButton.IsEnabled = false;
                DeclarationButton.Content = "⏳ Đang mở...";

                var form = ActivatorUtilities.CreateInstance<HealthDeclarationFormWindow>(App.Services, student);
                form.Owner = Window.GetWindow(this);

                form.Closed += (s, args) =>
                {
                    // Re-enable button khi form đóng
                    DeclarationButton.IsEnabled = true;
                    DeclarationButton.Content = "📝 Khai báo";
                };

                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi mở form khai báo: {ex.Message}\n\n" +
                    $"🕐 Thời gian: {DateTime.Now}\n", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                // Re-enable button nếu có lỗi
                DeclarationButton.IsEnabled = true;
                DeclarationButton.Content = "📝 Khai báo";
            }
        }
    }
}