using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartPos.Models;
using SmartPos.Services;
using System.Collections.ObjectModel;
using SmartPos.Resources.Strings;
using CommunityToolkit.Mvvm.Messaging;

namespace SmartPos.ViewModels
{



    public partial class NotificationsViewModel : ObservableObject
    {

        public string titleText => AppResources.NotificationsPage_Title;
        public string deleteText => AppResources.NotificationsPage_SwipeItem_Delete;
        public string deleteTextConfirm => AppResources.ManageMenuCategoriesPage_SwipeLeftDelete;


        private readonly NotificationsService _service;

        // تبقى القائمة من الخدمة نفسها
        [ObservableProperty]
        private ObservableCollection<NotificationModel> _notifications;

        // الأمر لحذف إشعار
        public IRelayCommand<NotificationModel> DeleteCommand { get; }

        public NotificationsViewModel(NotificationsService svc)
        {
            _service = svc;

            Notifications = _service.Notifications;
            // إنشاء أمر الحذف
            DeleteCommand = new RelayCommand<NotificationModel>(OnDeleteRequested);


            WeakReferenceMessenger.Default.Register<CultureChangedMessage>(this, (r, m) =>
            {
                OnPropertyChanged(nameof(titleText));
                OnPropertyChanged(nameof(deleteText));
                OnPropertyChanged(nameof(deleteTextConfirm));
            });

        }

        // معالج حذف الإشعار مع تأكيد المستخدم
        private async void OnDeleteRequested(NotificationModel note)
        {
            bool confirm = await App.Current.MainPage
                .DisplayAlert(titleText, AppResources.NotificationsPage_ConfirmDelete, AppResources.Prompt_ClearCart_Accept, AppResources.Prompt_ClearCart_Cancel);
            if (!confirm)
                return;

            _service.Remove(note);                              // يستدعي Removal ثم تحفظ البيانات دائماً
        }
    }


    //public partial class NotificationsViewModel : ObservableObject
    //{



    //    // ← حافظ على ObservableCollection وليس نسخة منه
    //    public ObservableCollection<NotificationModel> Notifications { get; }

    //    public NotificationsViewModel(NotificationsService svc)
    //    {
    //        Notifications = svc.Notifications;
    //    }

    //    //public ObservableCollection<NotificationModel> Notifications { get; }

    //    //public NotificationsViewModel(NotificationsService svc)
    //    //{
    //    //    Notifications = new ObservableCollection<NotificationModel>(svc.Notifications);
    //    //}
    //}
}
