using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;
using System.Linq;

namespace SchoolMedicalWpf.App.Admin
{
    public partial class StudentManagementPage : UserControl
    {
        private readonly StudentService _studentService;
        private ObservableCollection<Student> _students = new();
        private ObservableCollection<string> _grades = new();

        public StudentManagementPage(StudentService studentService)
        {
            InitializeComponent();
            _studentService = studentService;
            LoadData();
        }

        private async void LoadData()
        {
            var students = await _studentService.GetAllStudents();
            _students = new ObservableCollection<Student>(students);

            // Lấy danh sách khối lớp duy nhất
            var gradeList = students
                .Where(s => !string.IsNullOrWhiteSpace(s.Grade))
                .Select(s => s.Grade!)
                .Distinct()
                .OrderBy(g => g)
                .ToList();
            _grades = new ObservableCollection<string>(gradeList);

            FilterGradeComboBox.ItemsSource = _grades;
            FilterGradeComboBox.SelectedIndex = -1;

            StudentDataGrid.ItemsSource = _students;
        }

        private async void FilterGradeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var students = await _studentService.GetAllStudents();
            if (FilterGradeComboBox.SelectedItem is string selectedGrade)
                _students = new ObservableCollection<Student>(students.Where(s => s.Grade == selectedGrade));
            else
                _students = new ObservableCollection<Student>(students);

            StudentDataGrid.ItemsSource = null;
            StudentDataGrid.ItemsSource = _students;
        }

        private async void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterGradeComboBox.SelectedIndex = -1;
            var students = await _studentService.GetAllStudents();
            _students = new ObservableCollection<Student>(students);
            StudentDataGrid.ItemsSource = null;
            StudentDataGrid.ItemsSource = _students;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CreateUpdateStudent(_studentService);
            if (dialog.ShowDialog() == true)
                LoadData();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (StudentDataGrid.SelectedItem is not Student selected)
            {
                MessageBox.Show("Vui lòng chọn học sinh để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var dialog = new CreateUpdateStudent(_studentService, selected);
            if (dialog.ShowDialog() == true)
                LoadData();
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (StudentDataGrid.SelectedItem is not Student selected)
            {
                MessageBox.Show("Vui lòng chọn học sinh để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show($"Bạn có chắc muốn xóa học sinh '{selected.FullName}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                await _studentService.DeleteStudent(selected.StudentId);
                LoadData();
            }
        }
    }
}