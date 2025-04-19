using CommunityToolkit.Maui.Views;
using SmartPos.Controls;
using SmartPos.Pages;

namespace SmartPos
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(NotificationsPage), typeof(NotificationsPage));
        }




        private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            var helpPopup = new HelpPopup();
            await this.ShowPopupAsync(helpPopup);
        }
    }
}
