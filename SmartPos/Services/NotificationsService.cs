using SmartPos.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPos.Services
{
    public class NotificationsService
    {

        // ← غيّر List إلى ObservableCollection
        private readonly ObservableCollection<NotificationModel> _notifications
            = new ObservableCollection<NotificationModel>();

        // ← expose الـ ObservableCollection مباشرة
        public ObservableCollection<NotificationModel> Notifications => _notifications;

        public void Add(NotificationModel note)
            => _notifications.Add(note);

        //private readonly List<NotificationModel> _notifications = new();
        //public IReadOnlyList<NotificationModel> Notifications => _notifications;
        //public void Add(NotificationModel note) => _notifications.Add(note);
    }
}
