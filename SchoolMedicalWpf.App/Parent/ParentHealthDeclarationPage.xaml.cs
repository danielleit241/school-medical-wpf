using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Parent
{
    /// <summary>
    /// Interaction logic for ParentHealthDeclarationPage.xaml
    /// </summary>
    public partial class ParentHealthDeclarationPage : UserControl
    {
        public ObservableCollection<StudentDto> StudentList { get; set; } = new ObservableCollection<StudentDto>();

        public ParentHealthDeclarationPage()
        {
            InitializeComponent();
            StudentList.Add(new StudentDto { Code = "HS001", Name = "Nguyễn Văn A", ClassName = "10A1" });
            StudentList.Add(new StudentDto { Code = "HS002", Name = "Lê Thị B", ClassName = "10A2" });
            DataContext = this;
        }

    }
    public class StudentDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ClassName { get; set; }
    }
}
