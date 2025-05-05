using CommunityToolkit.Mvvm.Messaging;
using SmartPos.Models;
using SmartPos.Resources.Strings;

namespace SmartPos.Controls;

public partial class CurrentDateTimeControl : ContentView , IDisposable , IRecipient<CultureChangedMessage>
{
	private readonly PeriodicTimer _timer;
	public CurrentDateTimeControl()
	{
		InitializeComponent();

		//dayTimeLabel.Text = DateTime.Now.ToString("dddd, hh:mm:ss tt ");
		//dateLabel.Text = DateTime.Now.ToString("MMM dd,yyyy");
		WeakReferenceMessenger.Default.Register(this);

		SetDateTimeText();

        _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

		UpdateTimer();

    }

    private void SetDateTimeText()
	{
        var ci = AppResources.Culture;
		if ( ci.TwoLetterISOLanguageName == "ar")
		{
            dateLabel.Text = DateTime.Now.ToString("HH:mm:ss", ci);
            dayTimeLabel.Text = DateTime.Now.ToString("dddd dd MMMM yyyy", ci);
		}
		else
		{
			dateLabel.Text = DateTime.Now.ToString("MMMM dd, yyyy", ci);
			dayTimeLabel.Text = DateTime.Now.ToString("dddd, hh:mm:ss tt", ci);
		}

    }

    private async void UpdateTimer()
	{
		while (await _timer.WaitForNextTickAsync())
		{
			SetDateTimeText();
        }
	}

	public void Receive(CultureChangedMessage message)
	{
		SetDateTimeText();
	}

	public void Dispose() => _timer?.Dispose();

}