using System.Windows;
using Microsoft.IdentityModel.Tokens;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Parent
{
    public partial class HealthDeclarationFormWindow : Window
    {
        private Student _currentStudent;
        private readonly HealthProfileService _profileService;

        public HealthDeclarationFormWindow(Student student, HealthProfileService profileService)
        {
            InitializeComponent();
            _currentStudent = student ?? throw new ArgumentNullException(nameof(student), "Student cannot be null");
            _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService), "HealthProfileService cannot be null");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            var existedProfile = _profileService.GetHealthProfileByStudentId(_currentStudent.StudentId);
            if (existedProfile != null)
            {
                txtChronicDiseases.Text = existedProfile.ChronicDiseases ?? string.Empty;
                txtDrugAllergies.Text = existedProfile.DrugAllergies ?? string.Empty;
                txtFoodAllergies.Text = existedProfile.FoodAllergies ?? string.Empty;
                txtNotes.Text = existedProfile.Notes ?? string.Empty;
                dpDeclarationDate.SelectedDate = existedProfile.DeclarationDate.HasValue ? existedProfile.DeclarationDate.Value.ToDateTime(new TimeOnly(0, 0)) : null;
                btnSubmit.Visibility = Visibility.Collapsed;
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {

            if (_currentStudent == null)
            {
                MessageBox.Show("Không tìm thấy thông tin học sinh!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var existedProfile = _profileService.GetHealthProfileByStudentId(_currentStudent.StudentId);

            if (existedProfile != null)
            {
                btnSubmit.IsEnabled = false;
                return;
            }


            if (txtChronicDiseases.Text.IsNullOrEmpty())
            {
                MessageBox.Show("Vui lòng khai báo bệnh mãn tính!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (txtDrugAllergies.Text.IsNullOrEmpty())
            {
                MessageBox.Show("Vui lòng khai báo dị ứng thuốc!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (txtFoodAllergies.Text.IsNullOrEmpty())
            {
                MessageBox.Show("Vui lòng khai báo dị ứng thực phẩm!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (dpDeclarationDate.SelectedDate < DateTime.Now.AddDays(-1))
            {
                MessageBox.Show("Ngày khai báo không được trước ngày hôm nay!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (dpDeclarationDate.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn ngày khai báo!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (txtNotes.Text.IsNullOrEmpty())
            {
                MessageBox.Show("Vui lòng nhập ghi chú!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var healthProfile = new HealthProfile
            {
                HealthProfileId = Guid.NewGuid(),
                StudentId = _currentStudent.StudentId,
                CreatedDate = DateTime.Now,
                Notes = txtNotes.Text,
                ChronicDiseases = txtChronicDiseases.Text,
                DeclarationDate = DateOnly.FromDateTime(dpDeclarationDate.SelectedDate!.Value),
                DrugAllergies = txtDrugAllergies.Text,
                FoodAllergies = txtFoodAllergies.Text
            };

            _profileService.Add(healthProfile);

            MessageBox.Show("Khai báo y tế đã được lưu!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

            this.Close();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Bạn muốn thoát?", "Thoát", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }
    }
}