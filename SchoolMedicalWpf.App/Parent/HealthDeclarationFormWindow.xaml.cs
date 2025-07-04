using System.Windows;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Parent
{
    public partial class HealthDeclarationFormWindow : Window
    {
        private readonly Student _student;
        private readonly HealthProfileService _healthProfileService;
        private bool _isProcessing = false;

        public bool IsInputEnabled => !_isProcessing;

        public HealthDeclarationFormWindow(Student student, HealthProfileService healthProfileService)
        {
            InitializeComponent();
            _student = student ?? throw new ArgumentNullException(nameof(student));
            _healthProfileService = healthProfileService ?? throw new ArgumentNullException(nameof(healthProfileService));

            DataContext = this;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentDate = DateTime.UtcNow;
                dpDeclarationDate.SelectedDate = currentDate;
                CurrentDateText.Text = currentDate.ToString("yyyy-MM-dd HH:mm:ss") + " UTC";

                StudentInfoText.Text = $"Học sinh: {_student.FullName} - {_student.StudentCode} - Lớp {_student.Grade}";

                Title = $"Hồ sơ sức khỏe - {_student.FullName}";

                await LoadExistingHealthProfileAsync();

                dpDeclarationDate.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi khởi tạo form: {ex.Message}\n\n" +
                    $"🕐 Thời gian: {DateTime.Now}\n", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadExistingHealthProfileAsync()
        {
            try
            {
                var existingProfile = await Task.Run(() => _healthProfileService.GetHealthProfileByStudentId(_student.StudentId));
                if (existingProfile != null)
                {
                    if (existingProfile.DeclarationDate.HasValue)
                    {
                        dpDeclarationDate.SelectedDate = existingProfile.DeclarationDate.Value.ToDateTime(TimeOnly.MinValue);
                    }

                    txtChronicDiseases.Text = existingProfile.ChronicDiseases ?? "";
                    txtDrugAllergies.Text = existingProfile.DrugAllergies ?? "";
                    txtFoodAllergies.Text = existingProfile.FoodAllergies ?? "";
                    txtNotes.Text = existingProfile.Notes ?? "";

                    btnSubmit.Content = "🔄 Cập nhật hồ sơ";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading existing health profile: {ex.Message}");
            }
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (_isProcessing) return;

            try
            {
                _isProcessing = true;
                UpdateButtonStates();

                if (!dpDeclarationDate.SelectedDate.HasValue)
                {
                    MessageBox.Show("❌ Vui lòng chọn ngày khai báo.\n\n" +
                        $"🕐 Thời gian: {DateTime.Now}\n", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    dpDeclarationDate.Focus();
                    return;
                }

                var existingProfile = await Task.Run(() => _healthProfileService.GetHealthProfileByStudentId(_student.StudentId));

                if (existingProfile != null)
                {
                    existingProfile.DeclarationDate = DateOnly.FromDateTime(dpDeclarationDate.SelectedDate.Value);
                    existingProfile.ChronicDiseases = string.IsNullOrWhiteSpace(txtChronicDiseases.Text) ? null : txtChronicDiseases.Text.Trim();
                    existingProfile.DrugAllergies = string.IsNullOrWhiteSpace(txtDrugAllergies.Text) ? null : txtDrugAllergies.Text.Trim();
                    existingProfile.FoodAllergies = string.IsNullOrWhiteSpace(txtFoodAllergies.Text) ? null : txtFoodAllergies.Text.Trim();
                    existingProfile.Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim();

                    var updateResult = await Task.Run(() => _healthProfileService.Update(existingProfile));

                    if (updateResult)
                    {
                        MessageBox.Show($"✅ Hồ sơ sức khỏe đã được cập nhật thành công!\n\n" +
                            $"👨‍🎓 Học sinh: {_student.FullName}\n" +
                            $"📅 Ngày khai báo: {existingProfile.DeclarationDate:dd/MM/yyyy}\n\n" +
                            $"🕐 Thời gian: {DateTime.Now}\n", "Cập nhật thành công",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show($"❌ Không thể cập nhật hồ sơ sức khỏe. Vui lòng thử lại.\n\n" +
                            $"🕐 Thời gian: {DateTime.Now}\n", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    // Create new health profile
                    var healthProfile = new HealthProfile
                    {
                        HealthProfileId = Guid.NewGuid(),
                        StudentId = _student.StudentId,
                        CreatedDate = DateTime.Now,
                        DeclarationDate = DateOnly.FromDateTime(dpDeclarationDate.SelectedDate.Value),
                        ChronicDiseases = string.IsNullOrWhiteSpace(txtChronicDiseases.Text) ? null : txtChronicDiseases.Text.Trim(),
                        DrugAllergies = string.IsNullOrWhiteSpace(txtDrugAllergies.Text) ? null : txtDrugAllergies.Text.Trim(),
                        FoodAllergies = string.IsNullOrWhiteSpace(txtFoodAllergies.Text) ? null : txtFoodAllergies.Text.Trim(),
                        Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim()
                    };

                    btnSubmit.Content = "⏳ Đang lưu...";
                    btnSubmit.IsEnabled = false;

                    var createResult = await Task.Run(() => _healthProfileService.Add(healthProfile));

                    if (createResult)
                    {
                        MessageBox.Show($"✅ Hồ sơ sức khỏe đã được tạo thành công!\n\n" +
                            $"👨‍🎓 Học sinh: {_student.FullName}\n" +
                            $"📅 Ngày khai báo: {healthProfile.DeclarationDate:dd/MM/yyyy}\n" +
                            $"🆔 ID hồ sơ: {healthProfile.HealthProfileId}\n\n" +
                            $"🕐 Thời gian: {DateTime.Now}\n", "Tạo thành công",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show($"❌ Không thể tạo hồ sơ sức khỏe. Vui lòng thử lại.\n\n" +
                            $"🕐 Thời gian: {DateTime.Now}\n", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi lưu hồ sơ sức khỏe: {ex.Message}\n\n" +
                    $"🕐 Thời gian: {DateTime.Now}\n", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _isProcessing = false;
                UpdateButtonStates();
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            if (_isProcessing)
            {
                MessageBox.Show("⏳ Đang xử lý, vui lòng đợi...\n\n" +
                    $"🕐 Thời gian: {DateTime.Now}\n", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show("❓ Bạn có chắc chắn muốn thoát mà không lưu?\n\n" +
                $"🕐 Thời gian: {DateTime.Now}\n", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                DialogResult = false;
                Close();
            }
        }

        private void UpdateButtonStates()
        {
            if (_isProcessing)
            {
                btnSubmit.Content = "⏳ Đang lưu...";
                btnSubmit.IsEnabled = false;
            }
            else
            {
                btnSubmit.Content = btnSubmit.Content.ToString()!.Contains("Cập nhật") ? "🔄 Cập nhật hồ sơ" : "✅ Lưu hồ sơ";
                btnSubmit.IsEnabled = true;
            }
            btnExit.IsEnabled = !_isProcessing;
        }
    }
}