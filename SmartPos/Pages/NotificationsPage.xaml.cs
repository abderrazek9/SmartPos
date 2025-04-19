using Microsoft.Maui.Controls;
using SmartPos.ViewModels;

namespace SmartPos.Pages
{
    public partial class NotificationsPage : ContentPage
    {
        public NotificationsPage(NotificationsViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}
