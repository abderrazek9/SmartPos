using CommunityToolkit.Maui.Views;
using System.Threading.Tasks;

namespace SmartPos.Controls;

public partial class HelpPopup : Popup
{

    public const string Email = "abderrazek1993@gmail.com";
    public const string Phone = "0540704131";


    public HelpPopup()
	{
		InitializeComponent();
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
        copyEmailLabel.Text = "Copied";
        await Task.Delay(2200);
        copyEmailLabel.Text = "Copy to Clipbaord";
    }
    private async void CopyPhone_Tapped(object sender, TappedEventArgs e)
    {
        await Clipboard.Default.SetTextAsync(Phone);
        copyPhoneLabel.Text = "Copied";
        await Task.Delay(2200);
        copyPhoneLabel.Text = "Copy to Clipbaord";
    }
}