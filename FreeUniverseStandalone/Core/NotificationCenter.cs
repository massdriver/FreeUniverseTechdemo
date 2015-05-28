using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Core
{
    public class NotificationCenter
    {
        class Notification
        {
            public NotificationID id;
            public object data;
        }

        class NotificationQueue
        {
            public List<INotificationCenterDelegate> delegates;
            public ListEx<Notification> events;

            public void Init()
            {
                delegates = new List<INotificationCenterDelegate>();
                events = new ListEx<Notification>();
            }
        }

        static NotificationCenter instance;

        public NotificationCenter()
        {
            instance = this;
        }

        Dictionary<NotificationID, NotificationQueue> _notificationMap = new Dictionary<NotificationID, NotificationQueue>();

        public static void Update()
        {
            if (instance == null)
                return;

            foreach (KeyValuePair<NotificationID,NotificationQueue> pair in instance._notificationMap)
                ProcessQueue(pair.Value);
        }

        public static void PostImmediate(NotificationID eventID, object obj)
        {
            NotificationQueue queue;
            instance._notificationMap.TryGetValue(eventID, out queue);

            if (null == queue)
                return;

            ProcessNotification(eventID, obj, queue.delegates);
        }

        static void ProcessQueue(NotificationQueue queue)
        {
            Notification n = queue.events.Iterate();
            while (n != null)
            {
                ProcessNotification(n.id, n.data, queue.delegates);
                queue.events.Remove();
                n = queue.events.Next();
            }
        }

        static void ProcessNotification(NotificationID eventID, object obj, List<INotificationCenterDelegate> delegates)
        {
            foreach (INotificationCenterDelegate d in delegates)
                d.OnNotificationCenterEvent(eventID, obj);
        }

        public static void Post(NotificationID eventID, object obj)
        {
            Notification n = new Notification();
            n.id = eventID;
            n.data = obj;

            NotificationQueue queue;
            instance._notificationMap.TryGetValue(eventID, out queue);

            if (queue == null)
            {
                queue = new NotificationQueue();
                queue.Init();
                queue.events.Add(n);
                instance._notificationMap[eventID] = queue;
                return;
            }

            queue.events.Add(n);
        }

        public static void AddDelegate(INotificationCenterDelegate delegateObject, NotificationID eventID)
        {
            NotificationQueue queue;
            instance._notificationMap.TryGetValue(eventID, out queue);

            if (queue == null)
            {
                queue = new NotificationQueue();
                queue.Init();
                queue.delegates.Add(delegateObject);
                instance._notificationMap[eventID] = queue;
                return;
            }

            queue.delegates.Add(delegateObject);
        }

        public static void RemoveDelegate(INotificationCenterDelegate delegateObject, NotificationID eventID)
        {
            NotificationQueue queue;
            instance._notificationMap.TryGetValue(eventID, out queue);

            if (queue == null)
                return;

            queue.delegates.Remove(delegateObject);
        }
    }
}
