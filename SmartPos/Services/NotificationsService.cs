using SmartPos.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using Microsoft.Maui.Storage;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace SmartPos.Services
{
    public class NotificationsService
    {

        private const string PrefKey = "SavedNotification";

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


        public NotificationsService()
        {
            LoadNotifications();
            _notifications.CollectionChanged += OnCollectionChanged;  // حفظ آلي عند أي تغيير 3
        }

        private void LoadNotifications()
        {
            var json = Preferences.Get(PrefKey, "");                 // قراءة JSON من Preferences 4
            if (!string.IsNullOrWhiteSpace(json))
            {
                try
                {
                    var list = JsonSerializer.Deserialize<List<NotificationModel>>(json);
                    if (list != null)
                        foreach (var note in list)
                            _notifications.Add(note);
                }
                catch
                {
                    Preferences.Remove(PrefKey);                     // مسح المفتاح عند تلف البيانات 5
                }
            }
        }


        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => SaveNotifications();

        private void SaveNotifications()
        {
            var list = _notifications.ToList();
            var json = JsonSerializer.Serialize(list);               // تحويل القائمة إلى JSON 6
            Preferences.Set(PrefKey, json);                          // حفظها في Preferences 7
        }

        public void Remove(NotificationModel note)
        {
            _notifications.Remove(note);                             // يحفّز CollectionChanged فيتم الحفظ
        }

    }
}
