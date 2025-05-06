using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using SmartPos.Models;
using SmartPos.Resources.Strings;
using System.Threading.Tasks;

namespace SmartPos.Controls;

public partial class HelpPopup : Popup
{

    public const string Email = "abderrazek1993@gmail.com";
    public const string Phone = "0540704131";

    public string titleText => AppResources.HelpPopup_Title;
    public string connectText => AppResources.HelpPopup_YouCanConnectWithUs;
    public string copyText => AppResources.HelpPopup_CopyToClipboard;
    public string developerText => AppResources.HelpPopup_DesignedAndDevelopedBy;
    public string emailText => AppResources.HelpPopup_EmailUsAt;
    public string phoneText => AppResources.HelpPopup_CallUsAt;
    public HelpPopup()
	{
		InitializeComponent();

        BindingContext = this;

        WeakReferenceMessenger.Default.Register<CultureChangedMessage>(this, (r, m) =>
        {
            OnPropertyChanged(nameof(emailText));
            OnPropertyChanged(nameof(phoneText));
            OnPropertyChanged(nameof(titleText));
            OnPropertyChanged(nameof(connectText));
            OnPropertyChanged(nameof(copyText));
            OnPropertyChanged(nameof(developerText));
        });
	}

    private async void CloseLabel_Tapped(object sender, TappedEventArgs e)
    {
		await this.CloseAsync();
    }

    private async void Footer_Tapped(object sender, TappedEventArgs e)
    {
        await Launcher.Default.OpenAsync("https://www.facebook.com/abdou.aim");
    }

    private async void CopyEmail_Tapped(object sender, TappedEventArgs e)
    {
        await Clipboard.Default.SetTextAsync(Email);
        copyEmailLabel.Text = AppResources.HelpPopup_Copied;
        await Task.Delay(2200);
        copyEmailLabel.Text = AppResources.HelpPopup_CopyToClipboard;
    }
    private async void CopyPhone_Tapped(object sender, TappedEventArgs e)
    {
        await Clipboard.Default.SetTextAsync(Phone);
        copyPhoneLabel.Text = AppResources.HelpPopup_Copied;
        await Task.Delay(2200);
        copyPhoneLabel.Text = AppResources.HelpPopup_CopyToClipboard;
    }
}