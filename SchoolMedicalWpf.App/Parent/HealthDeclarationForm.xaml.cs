using System.Windows;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Parent
{
    /// <summary>
    /// Interaction logic for HealthDeclarationForm.xaml
    /// </summary>
    public partial class HealthDeclarationForm : Window
    {
        private Student _currentStudent;

        public HealthDeclarationForm(Student student)
        {
            InitializeComponent();
            _currentStudent = student;
        }
    }
}
