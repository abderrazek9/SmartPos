using CommunityToolkit.Mvvm.Messaging;
using SmartPos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SmartPos.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private const string NameKey = "name";
        private const string PromoPercentageKey = "promo";


        private bool _isInitialized;

        public async ValueTask InitializeAsync()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;

            var name = Preferences.Default.Get<string?>(NameKey, null);

            if(name is null)
            {
                do
                {
                    name = await Shell.Current.DisplayPromptAsync("Your name", "Enter your name");
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
                    .DisplayPromptAsync("Your name", "Enter your name");
            }
            while (string.IsNullOrWhiteSpace(name));

            Preferences.Default.Set<string>(NameKey, name);
            WeakReferenceMessenger.Default.Send(NameChangedMessage.From(name));
        }

    }
}
