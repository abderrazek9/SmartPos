using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SmartPos.Data;
using SmartPos.Models;
using SmartPos.Pages;
using SmartPos.Services;
using SmartPos.ViewModels;

namespace SmartPos;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{

                fonts.AddFont("Poppins-Regular.ttf", "PoppinsRegular");
                fonts.AddFont("Poppins-Bold.ttf", "PoppinsBold");

            });

#if DEBUG
		builder.Logging.AddDebug();
#endif


		builder.Services.AddSingleton<DataBaseService>()
			.AddSingleton<HomeViewModel>()
			.AddSingleton<MainPage>()
			.AddSingleton<OrdersViewModel>()
			.AddSingleton<OrdersPage>()
			.AddTransient<ManageMenuItemsViewModel>()
			.AddTransient<ManageMenuItemPage>()
			.AddSingleton<SettingsViewModel>()
			.AddSingleton<IBluetoothPrinterService, DummyBluetoothPrinterService>();
			//.AddSingleton<IBluetoothPrinterService, BluetoothPrinterService>();

		return builder.Build();
	}
}
