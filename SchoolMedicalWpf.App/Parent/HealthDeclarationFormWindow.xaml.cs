using System.Windows;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Parent
{
    /// <summary>
    /// Interaction logic for HealthDeclarationForm.xaml
    /// </summary>
    public partial class HealthDeclarationFormWindow : Window
    {
        private Student _currentStudent;

        public HealthDeclarationFormWindow(Student student)
        {
            InitializeComponent();
            _currentStudent = student;
        }
    }
}
