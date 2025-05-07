using CommunityToolkit.Mvvm.Messaging;
using SmartPos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SmartPos.Resources.Strings;

namespace SmartPos.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private const string NameKey = "name";
        private const string PromoPercentageKey = "promo";


        private bool _isInitialized;


        private const string CultureKey = "culture";

        // قائمة اللغات المتاحة
        public ObservableCollection<CultureInfo> AvailableCultures { get; }
            = new ObservableCollection<CultureInfo>
            {
        new CultureInfo("en"),
        new CultureInfo("ar")
            };

        // اللغة المحددة حالياً
        [ObservableProperty]
        private CultureInfo _selectedCulture;


        public string TitleText => AppResources.SettingsPage_Title;
        public string ChangeUserNameText => AppResources.SettingsPage_Button_ChangeUserName;
        public string LanguageLabelText => AppResources.SettingsPage_Label_Language;

        partial void OnSelectedCultureChanged(CultureInfo value)
        {
            // خزّن الاختيار
            Preferences.Default.Set(CultureKey, value.Name);


            //// طبّق الثقافة
            CultureInfo.CurrentCulture = value;
            CultureInfo.CurrentUICulture = value;
            AppResources.Culture = value;


            OnPropertyChanged(nameof(TitleText));
            OnPropertyChanged(nameof(ChangeUserNameText));
            OnPropertyChanged(nameof(LanguageLabelText));


            WeakReferenceMessenger.Default.Send(new CultureChangedMessage(value));
            //// (اختياري) أعد تحميل الصفحة أو أعد تشغيل التطبيق
            // Shell.Current.GoToAsync("//SettingsPage", true);
        }

        public async ValueTask InitializeAsync()
        {
            if (_isInitialized)
                return;

            var saved = Preferences.Default.Get(CultureKey, CultureInfo.CurrentCulture.Name);
            SelectedCulture = AvailableCultures.FirstOrDefault(c => c.Name == saved)
                              ?? AvailableCultures.First();

            _isInitialized = true;

            var name = Preferences.Default.Get<string?>(NameKey, null);

            if(name is null)
            {
                do
                {
                    name = await Shell.Current.DisplayPromptAsync($"{AppResources.Prompt_YourName_Title}", $"{AppResources.Prompt_YourName_Message}");
                }
                while (string.IsNullOrWhiteSpace(name));

                Preferences.Default.Set<string>(NameKey, name!);

            }

            WeakReferenceMessenger.Default.Send(NameChangedMessage.From(name));
        }

        public int GetPromoPercentage() => Preferences.Default.Get<int>(PromoPercentageKey, 0);

        public void SetPromoPercentage(int promoPercentage) => Preferences.Default.Set<int>(PromoPercentageKey, promoPercentage);



        [RelayCommand]
        public async Task PromptForUserNameAsync()
        {
            string? name;
            do
            {
                name = await Shell.Current
                    .DisplayPromptAsync($"{AppResources.Prompt_YourName_Title}", $"{AppResources.Prompt_YourName_Message}");
            }
            while (string.IsNullOrWhiteSpace(name));

            Preferences.Default.Set<string>(NameKey, name);
            WeakReferenceMessenger.Default.Send(NameChangedMessage.From(name));
        }

    }
}
