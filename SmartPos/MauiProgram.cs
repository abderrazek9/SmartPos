using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SmartPos.Data;
using SmartPos.Models;
using System.Globalization;
using SmartPos.Resources.Strings;
using Microsoft.Maui.Storage;
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


		var savedCulture = Preferences.Default.Get("culture", CultureInfo.CurrentCulture.Name);

		var ci = new CultureInfo(savedCulture);

		CultureInfo.DefaultThreadCurrentCulture = ci;

		CultureInfo.DefaultThreadCurrentUICulture = ci;

		AppResources.Culture = ci;


		builder.Services.AddSingleton<DataBaseService>()
			.AddSingleton<HomeViewModel>()
			.AddSingleton<MainPage>()
			.AddSingleton<OrdersViewModel>()
			.AddSingleton<OrdersPage>()
			.AddTransient<ManageMenuItemsViewModel>()
			.AddTransient<ManageMenuItemPage>()
			.AddSingleton<SettingsViewModel>()
			.AddSingleton<SettingsPage>()
			.AddSingleton<IBluetoothPrinterService, DummyBluetoothPrinterService>()
		    .AddSingleton<NotificationsService>()
			.AddSingleton<NotificationsViewModel>()
			.AddSingleton<NotificationsPage>()
			.AddTransient<ManageMenuCategoriesViewModel>()
			.AddTransient<ManageMenuCategoriesPage>();
        //.AddSingleton<IBluetoothPrinterService, BluetoothPrinterService>();

        return builder.Build();
	}
}
