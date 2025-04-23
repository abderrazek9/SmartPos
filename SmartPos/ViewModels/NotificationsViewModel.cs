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

namespace SmartPos.ViewModels
{



    public partial class NotificationsViewModel : ObservableObject
    {
        private readonly NotificationsService _service;

        // تبقى القائمة من الخدمة نفسها
        [ObservableProperty]
        private ObservableCollection<NotificationModel> _notifications;

        // الأمر لحذف إشعار
        public IRelayCommand<NotificationModel> DeleteCommand { get; }

        public NotificationsViewModel(NotificationsService svc)
        {
            _service = svc;
            // ربط القائمة بالـ Service
            Notifications = _service.Notifications;
            // إنشاء أمر الحذف
            DeleteCommand = new RelayCommand<NotificationModel>(OnDeleteRequested);
        }

        // معالج حذف الإشعار مع تأكيد المستخدم
        private async void OnDeleteRequested(NotificationModel note)
        {
            bool confirm = await App.Current.MainPage
                .DisplayAlert("تأكيد الحذف",                       // عرض Alert من صفحة الـ MAUI 14
                              "هل تريد حقًا حذف هذا الإشعار؟",
                              "نعم",
                              "لا");
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
