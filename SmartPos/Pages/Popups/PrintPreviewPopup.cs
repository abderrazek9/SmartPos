using CommunityToolkit.Maui.Views;
using SmartPos.Resources.Strings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPos.Pages.Popups
{
    public class PrintPreviewPopup : Popup
    {
        public PrintPreviewPopup(string receiptText, Func<Task> onPrint)
        {
            Size = new Size(300, 400);

            // 1) نعرِّف شبكة بثلاث صفوف: الصف 0 = ScrollView (يشغل المساحة)
            var grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star }
                }
            };

            // 2) ScrollView في الصف الأول
            var scroll = new ScrollView
            {
                Content = new Label
                {
                    Text = receiptText,
                    LineBreakMode = LineBreakMode.WordWrap
                }
            };
            Grid.SetRow(scroll, 0);
            grid.Children.Add(scroll);

            // 3) زرّ “طباعة” في الصف الثاني
            var printBtn = new Button
            {
                Text = $"{AppResources.OrdersPage_Button_Printing}",
                HeightRequest = 40,
                Margin = new Thickness(0, 0, 0, 5),
                BackgroundColor = Colors.DarkOliveGreen,
                TextColor = Colors.White,
                Command = new Command(async () =>
                {
                    await onPrint();
                    Close();
                }),
                VerticalOptions = LayoutOptions.End
            };
            Grid.SetRow(printBtn, 1);
            grid.Children.Add(printBtn);

            // 4) زرّ “إلغاء” في الصف الثالث
            var cancelBtn = new Button
            {
                Text = $"{AppResources.Button_Cancel}",
                HeightRequest = 40,
                Margin = new Thickness(0, 0, 0, 0),
                BackgroundColor = Colors.Red,
                TextColor = Colors.White,
                Command = new Command(Close),
                VerticalOptions = LayoutOptions.End
            };
            Grid.SetRow(cancelBtn, 2);
            grid.Children.Add(cancelBtn);

            // 5) نضع الـ Grid داخل الـ Frame كالمعتاد
            Content = new Frame
            {
                Padding = 16,
                CornerRadius = 8,
                HasShadow = true,
                BackgroundColor = Colors.White,
                Content = grid
            };
        }
    }

}
