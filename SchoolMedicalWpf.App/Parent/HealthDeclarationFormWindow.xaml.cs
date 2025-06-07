using Microsoft.Extensions.DependencyInjection;
using SchoolMedicalWpf.Dal;
using SchoolMedicalWpf.Dal.Entities;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace SchoolMedicalWpf.App.Parent
{
    public partial class HealthDeclarationFormWindow : Window, INotifyPropertyChanged
    {
        private Student _currentStudent;
        private readonly SchoolmedicalWpfContext _dbContext;

        private bool _isInputEnabled = true;
        public bool IsInputEnabled
        {
            get => _isInputEnabled;
            set { _isInputEnabled = value; OnPropertyChanged(nameof(IsInputEnabled)); }
        }

        private Visibility _submitButtonVisibility = Visibility.Visible;
        public Visibility SubmitButtonVisibility
        {
            get => _submitButtonVisibility;
            set { _submitButtonVisibility = value; OnPropertyChanged(nameof(SubmitButtonVisibility)); }
        }

        public HealthDeclarationFormWindow(Student student)
        {
            InitializeComponent();
            _currentStudent = student;
            _dbContext = App.Services.GetRequiredService<SchoolmedicalWpfContext>();
            this.DataContext = this;

            // Nếu đã có hồ sơ, disable luôn và ẩn nút
            var existedProfile = _dbContext.HealthProfiles
                .FirstOrDefault(hp => hp.StudentId == _currentStudent.StudentId);

            if (existedProfile != null)
            {
                IsInputEnabled = false;
                SubmitButtonVisibility = Visibility.Collapsed;

                // Hiển thị lại thông tin nếu muốn, hoặc chỉ disable là đủ
                txtChronicDiseases.Text = existedProfile.ChronicDiseases;
                dpDeclarationDate.SelectedDate = existedProfile.DeclarationDate.HasValue ?
                    new System.DateTime(existedProfile.DeclarationDate.Value.Year, existedProfile.DeclarationDate.Value.Month, existedProfile.DeclarationDate.Value.Day) : null;
                txtDrugAllergies.Text = existedProfile.DrugAllergies;
                txtFoodAllergies.Text = existedProfile.FoodAllergies;
                txtNotes.Text = existedProfile.Notes;
            }
            else
            {
                IsInputEnabled = true;
                SubmitButtonVisibility = Visibility.Visible;
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentStudent == null)
            {
                MessageBox.Show("Không tìm thấy thông tin học sinh!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var existedProfile = _dbContext.HealthProfiles
                .FirstOrDefault(hp => hp.StudentId == _currentStudent.StudentId);

            if (existedProfile != null)
            {
                MessageBox.Show("Học sinh này đã khai báo y tế. Không thể khai báo lần 2!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                IsInputEnabled = false;
                SubmitButtonVisibility = Visibility.Collapsed;
                return;
            }

            DateOnly? declarationDate = null;
            if (dpDeclarationDate.SelectedDate.HasValue)
            {
                var selected = dpDeclarationDate.SelectedDate.Value;
                declarationDate = new DateOnly(selected.Year, selected.Month, selected.Day);
            }

            var healthProfile = new HealthProfile
            {
                HealthProfileId = Guid.NewGuid(),
                StudentId = _currentStudent.StudentId,
                CreatedDate = System.DateTime.Now,
                Notes = txtNotes.Text,
                ChronicDiseases = txtChronicDiseases.Text,
                DeclarationDate = declarationDate,
                DrugAllergies = txtDrugAllergies.Text,
                FoodAllergies = txtFoodAllergies.Text
            };

            _dbContext.HealthProfiles.Add(healthProfile);
            _dbContext.SaveChanges();

            MessageBox.Show("Khai báo y tế đã được lưu!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

            IsInputEnabled = false; // Disable sau khi gửi xong
            SubmitButtonVisibility = Visibility.Collapsed; // Ẩn nút sau khi gửi xong
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}