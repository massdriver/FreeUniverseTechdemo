using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Core
{
    public interface INotificationCenterDelegate
    {
        void OnNotificationCenterEvent(NotificationID id, object data);
    }
}
