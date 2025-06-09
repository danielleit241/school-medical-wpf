using System;
using System.Globalization;
using System.Windows.Data;

namespace SchoolMedicalWpf.App.Admin
{
    public class StatusToStringConverter : IValueConverter
    {
        // Chuyển đổi từ bool sang chuỗi
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool status)
            {
                return status ? "Đang hoạt động" : "Không hoạt động";
            }

            return "Không xác định";
        }

        // Không cần triển khai lại ConvertBack vì chúng ta chỉ cần chuyển đổi một chiều
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
