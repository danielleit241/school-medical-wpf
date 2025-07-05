using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SchoolMedicalWpf.App.Admin
{
    public partial class StudentManagementPage : UserControl, INotifyPropertyChanged
    {
        private readonly StudentService _studentService;
        private readonly User _currentUser;
        private readonly UserService _userService;
        private ObservableCollection<Student> _allStudents = new();
        private ObservableCollection<Student> _filteredStudents = new();
        private ObservableCollection<Student> _pagedStudents = new();
        private ObservableCollection<string> _grades = new();
        private DispatcherTimer _searchTimer;
        private string _searchText = "";
        private bool _isLoading = false;

        // Pagination properties
        private int _currentPage = 1;
        private int _itemsPerPage = 10;
        private int _totalPages = 1;
        private readonly int[] _itemsPerPageOptions = { 5, 10, 20, 50, 100 };

        public event PropertyChangedEventHandler? PropertyChanged;

        // Properties for binding
        private int _totalStudents;
        public int TotalStudents
        {
            get => _totalStudents;
            set
            {
                _totalStudents = value;
                OnPropertyChanged(nameof(TotalStudents));
                OnPropertyChanged(nameof(StudentCountDisplay));
            }
        }

        private int _maleStudents;
        public int MaleStudents
        {
            get => _maleStudents;
            set
            {
                _maleStudents = value;
                OnPropertyChanged(nameof(MaleStudents));
            }
        }

        private int _femaleStudents;
        public int FemaleStudents
        {
            get => _femaleStudents;
            set
            {
                _femaleStudents = value;
                OnPropertyChanged(nameof(FemaleStudents));
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
                OnPropertyChanged(nameof(PaginationInfo));
                OnPropertyChanged(nameof(CanGoPrevious));
                OnPropertyChanged(nameof(CanGoNext));
            }
        }

        public int ItemsPerPage
        {
            get => _itemsPerPage;
            set
            {
                _itemsPerPage = value;
                OnPropertyChanged(nameof(ItemsPerPage));
                CalculatePagination();
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set
            {
                _totalPages = value;
                OnPropertyChanged(nameof(TotalPages));
                OnPropertyChanged(nameof(PaginationInfo));
                OnPropertyChanged(nameof(CanGoPrevious));
                OnPropertyChanged(nameof(CanGoNext));
            }
        }

        public string StudentCountDisplay => $"{_filteredStudents.Count} học sinh";

        public string PaginationInfo
        {
            get
            {
                if (_filteredStudents.Count == 0) return "Không có dữ liệu";

                var startItem = (_currentPage - 1) * _itemsPerPage + 1;
                var endItem = Math.Min(_currentPage * _itemsPerPage, _filteredStudents.Count);
                return $"Hiển thị {startItem}-{endItem} của {_filteredStudents.Count} học sinh";
            }
        }

        public bool CanGoPrevious => _currentPage > 1;
        public bool CanGoNext => _currentPage < _totalPages;

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public int[] ItemsPerPageOptions => _itemsPerPageOptions;

        public StudentManagementPage(StudentService studentService, UserService userService, User currentUser)
        {
            InitializeComponent();
            _studentService = studentService;
            _currentUser = currentUser;
            _userService = userService;
            DataContext = this;

            Welcome.Text = $"Xin chào {_currentUser.FullName}";

            InitializeSearchTimer();
            SetupEventHandlers();
            InitializePagination();
            LoadDataAsync();
        }

        private void InitializeSearchTimer()
        {
            _searchTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            _searchTimer.Tick += SearchTimer_Tick;
        }

        private void SetupEventHandlers()
        {
            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            SearchTextBox.GotFocus += SearchTextBox_GotFocus;
            SearchTextBox.LostFocus += SearchTextBox_LostFocus;
        }

        private void InitializePagination()
        {
            ItemsPerPageComboBox.ItemsSource = _itemsPerPageOptions;
            ItemsPerPageComboBox.SelectedItem = _itemsPerPage;
            ItemsPerPageComboBox.SelectionChanged += ItemsPerPageComboBox_SelectionChanged;
        }

        private async void LoadDataAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                var allStudents = await _studentService.GetAllStudents();
                StudentCountText.Text = $"Tổng số học sinh: {allStudents.Count()}";
                DashboardStudents.Text = allStudents.Count().ToString();
                DashboardStudentNam.Text = allStudents.Where(s => s.Gender?.ToLower() == "nam" || s.Grade?.ToLower() == "male").Count().ToString();
                DashboardStudentNu.Text = allStudents.Where(s => s.Gender?.ToLower() == "nữ" || s.Grade?.ToLower() == "female").Count().ToString();

                IsLoading = true;

                var students = await _studentService.GetAllStudents();
                _allStudents = new ObservableCollection<Student>(students);

                // Lấy danh sách khối lớp duy nhất
                var gradeList = students
                    .Where(s => !string.IsNullOrWhiteSpace(s.Grade))
                    .Select(s => s.Grade!)
                    .Distinct()
                    .OrderBy(g => g)
                    .ToList();

                _grades.Clear();
                _grades.Add("Tất cả khối lớp");
                foreach (var grade in gradeList)
                {
                    _grades.Add(grade);
                }

                FilterGradeComboBox.ItemsSource = _grades;
                FilterGradeComboBox.SelectedIndex = 0;

                ApplyFilters();
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplyFilters()
        {
            var filteredList = _allStudents.AsEnumerable();

            // Apply grade filter
            if (FilterGradeComboBox.SelectedItem is string selectedGrade &&
                selectedGrade != "Tất cả khối lớp")
            {
                filteredList = filteredList.Where(s => s.Grade == selectedGrade);
            }

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(_searchText) && _searchText != "Tìm kiếm học sinh...")
            {
                var searchLower = _searchText.ToLower();
                filteredList = filteredList.Where(s =>
                    s.FullName?.ToLower().Contains(searchLower) == true ||
                    s.StudentCode?.ToLower().Contains(searchLower) == true ||
                    s.ParentPhoneNumber?.Contains(_searchText) == true ||
                    s.ParentEmailAddress?.ToLower().Contains(searchLower) == true);
            }

            _filteredStudents = new ObservableCollection<Student>(filteredList);

            // Reset to first page when filters change
            _currentPage = 1;
            CalculatePagination();
            ApplyPagination();

            OnPropertyChanged(nameof(StudentCountDisplay));
        }

        private void CalculatePagination()
        {
            if (_filteredStudents.Count == 0)
            {
                TotalPages = 1;
                CurrentPage = 1;
            }
            else
            {
                TotalPages = (int)Math.Ceiling((double)_filteredStudents.Count / _itemsPerPage);

                // Ensure current page is within bounds
                if (_currentPage > TotalPages)
                {
                    CurrentPage = TotalPages;
                }
                else if (_currentPage < 1)
                {
                    CurrentPage = 1;
                }
            }

            OnPropertyChanged(nameof(PaginationInfo));
        }

        private void ApplyPagination()
        {
            var startIndex = (_currentPage - 1) * _itemsPerPage;
            var pagedItems = _filteredStudents.Skip(startIndex).Take(_itemsPerPage);

            _pagedStudents = new ObservableCollection<Student>(pagedItems);
            StudentDataGrid.ItemsSource = _pagedStudents;

            UpdatePaginationButtons();
        }

        private void UpdatePaginationButtons()
        {
            // Update pagination buttons visibility and content
            UpdatePageButtons();
        }

        private void UpdatePageButtons()
        {
            // Clear existing page buttons
            PaginationPanel.Children.Clear();

            // Previous button
            var prevButton = CreatePaginationButton("‹", !CanGoPrevious);
            prevButton.Click += (s, e) => GoToPreviousPage();
            PaginationPanel.Children.Add(prevButton);

            // Page number buttons
            var startPage = Math.Max(1, CurrentPage - 2);
            var endPage = Math.Min(TotalPages, CurrentPage + 2);

            // First page button if not in range
            if (startPage > 1)
            {
                var firstButton = CreatePaginationButton("1", false);
                firstButton.Click += (s, e) => GoToPage(1);
                PaginationPanel.Children.Add(firstButton);

                if (startPage > 2)
                {
                    var ellipsis = CreatePaginationButton("...", true);
                    PaginationPanel.Children.Add(ellipsis);
                }
            }

            // Page buttons in range
            for (int i = startPage; i <= endPage; i++)
            {
                var isCurrentPage = i == CurrentPage;
                var pageButton = CreatePaginationButton(i.ToString(), false, isCurrentPage);
                var pageNumber = i; // Capture for closure
                pageButton.Click += (s, e) => GoToPage(pageNumber);
                PaginationPanel.Children.Add(pageButton);
            }

            // Last page button if not in range
            if (endPage < TotalPages)
            {
                if (endPage < TotalPages - 1)
                {
                    var ellipsis = CreatePaginationButton("...", true);
                    PaginationPanel.Children.Add(ellipsis);
                }

                var lastButton = CreatePaginationButton(TotalPages.ToString(), false);
                lastButton.Click += (s, e) => GoToPage(TotalPages);
                PaginationPanel.Children.Add(lastButton);
            }

            // Next button
            var nextButton = CreatePaginationButton("›", !CanGoNext);
            nextButton.Click += (s, e) => GoToNextPage();
            PaginationPanel.Children.Add(nextButton);
        }

        private Button CreatePaginationButton(string content, bool isDisabled, bool isCurrent = false)
        {
            var button = new Button
            {
                Content = content,
                Width = 32,
                Height = 32,
                Margin = new Thickness(0, 0, 4, 0),
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Cursor = isDisabled ? System.Windows.Input.Cursors.Arrow : System.Windows.Input.Cursors.Hand,
                IsEnabled = !isDisabled
            };

            if (isCurrent)
            {
                button.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(52, 152, 219)); // #3498DB
                button.Foreground = System.Windows.Media.Brushes.White;
            }
            else if (isDisabled)
            {
                button.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(224, 224, 224)); // #E0E0E0
                button.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(127, 140, 141)); // #7F8C8D
            }
            else
            {
                button.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(224, 224, 224)); // #E0E0E0
                button.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(127, 140, 141)); // #7F8C8D
            }

            button.Style = (Style)FindResource("ActionButtonStyle");
            return button;
        }

        private void GoToPage(int pageNumber)
        {
            if (pageNumber >= 1 && pageNumber <= TotalPages && pageNumber != CurrentPage)
            {
                CurrentPage = pageNumber;
                ApplyPagination();
            }
        }

        private void GoToPreviousPage()
        {
            if (CanGoPrevious)
            {
                GoToPage(CurrentPage - 1);
            }
        }

        private void GoToNextPage()
        {
            if (CanGoNext)
            {
                GoToPage(CurrentPage + 1);
            }
        }

        private void ItemsPerPageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ItemsPerPageComboBox.SelectedItem is int selectedValue)
            {
                ItemsPerPage = selectedValue;
                CurrentPage = 1; // Reset to first page
                ApplyPagination();
            }
        }

        private void UpdateStatistics()
        {
            TotalStudents = _allStudents.Count;
            MaleStudents = _allStudents.Count(s => s.Gender?.ToLower() == "nam");
            FemaleStudents = _allStudents.Count(s => s.Gender?.ToLower() == "nữ");
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchTimer.Stop();
            _searchTimer.Start();
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == "Tìm kiếm học sinh...")
            {
                SearchTextBox.Text = "";
                SearchTextBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = "Tìm kiếm học sinh...";
                SearchTextBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void SearchTimer_Tick(object? sender, EventArgs e)
        {
            _searchTimer.Stop();
            _searchText = SearchTextBox.Text;
            ApplyFilters();
        }

        private void FilterGradeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoading) return;
            ApplyFilters();
        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterGradeComboBox.SelectedIndex = 0;
            SearchTextBox.Text = "Tìm kiếm học sinh...";
            SearchTextBox.Foreground = System.Windows.Media.Brushes.Gray;
            _searchText = "";
            ApplyFilters();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadData();
        }

        // Pagination event handlers
        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            GoToPreviousPage();
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            GoToNextPage();
        }

        private void PageNumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && int.TryParse(button.Content.ToString(), out int pageNumber))
            {
                GoToPage(pageNumber);
            }
        }

        // Rest of the existing methods remain the same...
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new CreateUpdateStudent(_studentService, _userService);
                if (dialog.ShowDialog() == true)
                {
                    LoadDataAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở dialog thêm học sinh: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (StudentDataGrid.SelectedItem is not Student selected)
            {
                MessageBox.Show("Vui lòng chọn học sinh để chỉnh sửa thông tin.", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var dialog = new CreateUpdateStudent(_studentService, _userService, selected);
                if (dialog.ShowDialog() == true)
                {
                    LoadDataAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở dialog chỉnh sửa học sinh: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (StudentDataGrid.SelectedItem is not Student selected)
            {
                MessageBox.Show("Vui lòng chọn học sinh để xóa.", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa học sinh '{selected.FullName}'?\n\n" +
                $"Thao tác này không thể hoàn tác!",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    IsLoading = true;
                    await _studentService.DeleteStudent(selected.StudentId);
                    await LoadData();

                    MessageBox.Show($"Đã xóa học sinh '{selected.FullName}' thành công.", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa học sinh: {ex.Message}", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            _searchTimer?.Stop();
            _searchTimer = null;
        }
    }
}