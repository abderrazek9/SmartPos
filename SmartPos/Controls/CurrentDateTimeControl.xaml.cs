using SmartPos.Resources.Strings;

namespace SmartPos.Controls;

public partial class CurrentDateTimeControl : ContentView , IDisposable
{
	private readonly PeriodicTimer _timer;
	public CurrentDateTimeControl()
	{
		InitializeComponent();

		dayTimeLabel.Text = DateTime.Now.ToString("dddd, hh:mm:ss tt ");
		dateLabel.Text = DateTime.Now.ToString("MMM dd,yyyy");

		var ci = AppResources.Culture;
		dayTimeLabel.Text = DateTime.Now.ToString("T", ci);
		dateLabel.Text = DateTime.Now.ToString("d", ci);

		_timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

		UpdateTimer();

    }    

    private async void UpdateTimer()
	{
		while (await _timer.WaitForNextTickAsync())
		{
            dayTimeLabel.Text = DateTime.Now.ToString("dddd, hh:mm:ss tt ");
            dateLabel.Text = DateTime.Now.ToString("MMM dd,yyyy");
			var ci = AppResources.Culture;
        }
	}

	public void Dispose() => _timer?.Dispose();

}