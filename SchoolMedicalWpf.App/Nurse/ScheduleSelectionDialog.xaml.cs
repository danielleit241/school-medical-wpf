using System.Windows;
using System.Windows.Controls;

namespace SchoolMedicalWpf.App.Nurse
{
    public partial class ScheduleSelectionDialog : Window
    {
        public ScheduleItem SelectedSchedule { get; private set; }

        public ScheduleSelectionDialog(List<ScheduleItem> schedules)
        {
            InitializeComponent();

            Title = "Chọn Schedule đang diễn ra";
            Width = 600;
            Height = 400;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            CreateContent(schedules);
        }

        private void CreateContent(List<ScheduleItem> schedules)
        {
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Header
            var header = new TextBlock
            {
                Text = $"Danh sách {schedules.Count} schedule đang diễn ra (2025-07-04 03:16:56)",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(20, 20, 20, 10)
            };
            Grid.SetRow(header, 0);
            grid.Children.Add(header);

            // Schedule List
            var listBox = new ListBox
            {
                Name = "lstSchedules",
                Margin = new Thickness(20, 0, 20, 20),
                ItemsSource = schedules
            };

            var itemTemplate = new DataTemplate();
            var factory = new FrameworkElementFactory(typeof(Border));
            factory.SetValue(Border.PaddingProperty, new Thickness(10));
            factory.SetValue(Border.MarginProperty, new Thickness(0, 5, 0, 5));
            factory.SetValue(Border.BorderBrushProperty, System.Windows.Media.Brushes.LightGray);
            factory.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            factory.SetValue(Border.CornerRadiusProperty, new CornerRadius(3));

            var stackPanel = new FrameworkElementFactory(typeof(StackPanel));

            var titleText = new FrameworkElementFactory(typeof(TextBlock));
            titleText.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("Title"));
            titleText.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            titleText.SetValue(TextBlock.FontSizeProperty, 14.0);

            var typeText = new FrameworkElementFactory(typeof(TextBlock));
            typeText.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("Type"));
            typeText.SetValue(TextBlock.ForegroundProperty, System.Windows.Media.Brushes.Blue);
            typeText.SetValue(TextBlock.FontSizeProperty, 12.0);

            var dateText = new FrameworkElementFactory(typeof(TextBlock));
            var dateBinding = new System.Windows.Data.MultiBinding();
            dateBinding.StringFormat = "Từ {0:dd/MM/yyyy} đến {1:dd/MM/yyyy}";
            dateBinding.Bindings.Add(new System.Windows.Data.Binding("StartDate"));
            dateBinding.Bindings.Add(new System.Windows.Data.Binding("EndDate"));
            dateText.SetBinding(TextBlock.TextProperty, dateBinding);
            dateText.SetValue(TextBlock.ForegroundProperty, System.Windows.Media.Brushes.Gray);
            dateText.SetValue(TextBlock.FontSizeProperty, 11.0);

            var gradeText = new FrameworkElementFactory(typeof(TextBlock));
            gradeText.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("TargetGrade"));
            gradeText.SetValue(TextBlock.ForegroundProperty, System.Windows.Media.Brushes.DarkGreen);
            gradeText.SetValue(TextBlock.FontSizeProperty, 11.0);

            stackPanel.AppendChild(titleText);
            stackPanel.AppendChild(typeText);
            stackPanel.AppendChild(dateText);
            stackPanel.AppendChild(gradeText);

            factory.AppendChild(stackPanel);
            itemTemplate.VisualTree = factory;
            listBox.ItemTemplate = itemTemplate;

            Grid.SetRow(listBox, 1);
            grid.Children.Add(listBox);

            // Buttons
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(20, 0, 20, 20)
            };

            var selectButton = new Button
            {
                Content = "Chọn Schedule",
                Padding = new Thickness(20, 8, 20, 8),
                Margin = new Thickness(0, 0, 10, 0),
                IsDefault = true
            };
            selectButton.Click += (s, e) =>
            {
                SelectedSchedule = listBox.SelectedItem as ScheduleItem ?? null!;
                if (SelectedSchedule != null)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn một schedule!", "Thông báo");
                }
            };

            var cancelButton = new Button
            {
                Content = "Hủy",
                Padding = new Thickness(20, 8, 20, 8),
                IsCancel = true
            };
            cancelButton.Click += (s, e) => { DialogResult = false; Close(); };

            buttonPanel.Children.Add(selectButton);
            buttonPanel.Children.Add(cancelButton);

            Grid.SetRow(buttonPanel, 2);
            grid.Children.Add(buttonPanel);

            Content = grid;
        }
    }
}