using System;
using System.Collections;
using System.Globalization;
using Microsoft.Maui.Controls;    // أو Xamarin.Forms حسب المشروع
using SmartPos.Resources;
using SmartPos.Resources.Strings;

namespace SmartPos.Converters
{
    public class NameKeyToDisplayNameConverter : IValueConverter
    {
        // تحويل من NameKey إلى DisplayName للعرض
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var key = value as string;
            if (string.IsNullOrWhiteSpace(key))
                return string.Empty;

            var disp = AppResources.ResourceManager
                             .GetString(key, AppResources.Culture);
            return disp ?? key;
        }

        // تحويل من النص المدخل في Entry إلى NameKey عند الحفظ
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var display = value as string;
            if (string.IsNullOrWhiteSpace(display))
                return string.Empty;

            var rs = AppResources.ResourceManager
                          .GetResourceSet(AppResources.Culture, true, true);
            foreach (DictionaryEntry entry in rs)
            {
                if (entry.Value is string val && val == display)
                    return entry.Key as string;
            }

            // لم نعثر على ترجمة، نعيد النص نفسه كمفتاح جديد
            return display;
        }
    }
}