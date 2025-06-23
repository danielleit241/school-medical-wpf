using System.Windows.Controls;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Parent
{
    /// <summary>
    /// Interaction logic for ParentHomePage.xaml
    /// </summary>
    public partial class ParentHomePage : UserControl
    {
        private User _currentUser;

        public ParentHomePage(User user)
        {
            InitializeComponent();
            _currentUser = user;

        }
    }
}
