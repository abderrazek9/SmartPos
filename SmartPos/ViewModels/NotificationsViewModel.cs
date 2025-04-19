using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SmartPos.Models;
using SmartPos.Services;
using System.Collections.ObjectModel;

namespace SmartPos.ViewModels
{
    public class NotificationsViewModel : ObservableObject
    {

        // ← حافظ على ObservableCollection وليس نسخة منه
        public ObservableCollection<NotificationModel> Notifications { get; }

        public NotificationsViewModel(NotificationsService svc)
        {
            Notifications = svc.Notifications;
        }

        //public ObservableCollection<NotificationModel> Notifications { get; }

        //public NotificationsViewModel(NotificationsService svc)
        //{
        //    Notifications = new ObservableCollection<NotificationModel>(svc.Notifications);
        //}
    }
}
