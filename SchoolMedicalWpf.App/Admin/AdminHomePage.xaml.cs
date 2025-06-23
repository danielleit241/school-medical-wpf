using System.Windows.Controls;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Admin
{
    /// <summary>
    /// Interaction logic for AdminHomePage.xaml
    /// </summary>
    public partial class AdminHomePage : UserControl
    {

        private User _currentUser;
        public AdminHomePage(User user)
        {
            InitializeComponent();
            _currentUser = user;
        }
    }
}
