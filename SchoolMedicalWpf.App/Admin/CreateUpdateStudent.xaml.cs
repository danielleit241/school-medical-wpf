using System.Windows;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Admin
{
    public partial class CreateUpdateStudent : Window
    {
        private readonly StudentService _studentService;
        private readonly bool _isEdit;
        private readonly Student? _editingStudent;

        public CreateUpdateStudent(StudentService studentService, Student? student = null)
        {
            InitializeComponent();
            _studentService = studentService;
            _isEdit = student != null;
            _editingStudent = student;
            if (_isEdit && student != null)
            {
                // Gán dữ liệu lên các trường nhập
                StudentCodeTextBox.Text = student.StudentCode;
                FullNameTextBox.Text = student.FullName;
                DayOfBirthPicker.SelectedDate = student.DayOfBirth.HasValue ? student.DayOfBirth.Value.ToDateTime(TimeOnly.MinValue) : null;
                GenderTextBox.Text = student.Gender;
                GradeTextBox.Text = student.Grade;
                AddressTextBox.Text = student.Address;
                ParentPhoneTextBox.Text = student.ParentPhoneNumber;
                ParentEmailTextBox.Text = student.ParentEmailAddress;
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isEdit && _editingStudent != null)
            {
                _editingStudent.StudentCode = StudentCodeTextBox.Text;
                _editingStudent.FullName = FullNameTextBox.Text;
                _editingStudent.DayOfBirth = DayOfBirthPicker.SelectedDate.HasValue ? DateOnly.FromDateTime(DayOfBirthPicker.SelectedDate.Value) : null;
                _editingStudent.Gender = GenderTextBox.Text;
                _editingStudent.Grade = GradeTextBox.Text;
                _editingStudent.Address = AddressTextBox.Text;
                _editingStudent.ParentPhoneNumber = ParentPhoneTextBox.Text;
                _editingStudent.ParentEmailAddress = ParentEmailTextBox.Text;
                await _studentService.UpdateStudent(_editingStudent);
            }
            else
            {
                var newStudent = new Student
                {
                    StudentCode = StudentCodeTextBox.Text,
                    FullName = FullNameTextBox.Text,
                    DayOfBirth = DayOfBirthPicker.SelectedDate.HasValue ? DateOnly.FromDateTime(DayOfBirthPicker.SelectedDate.Value) : null,
                    Gender = GenderTextBox.Text,
                    Grade = GradeTextBox.Text,
                    Address = AddressTextBox.Text,
                    ParentPhoneNumber = ParentPhoneTextBox.Text,
                    ParentEmailAddress = ParentEmailTextBox.Text,
                };
                await _studentService.AddStudent(newStudent);
            }
            DialogResult = true;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}