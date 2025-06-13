using System.Windows;
using System.Windows.Controls;
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
            var student = DataContext as Student;
            if (student == null)
            {
                MessageBox.Show("Student information is not available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //var form = new HealthDeclarationFormWindow(student);
            //form.Owner = Window.GetWindow(this);
            //form.ShowDialog();
        }
    }
}
