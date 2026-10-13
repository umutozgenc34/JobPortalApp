import { useEffect } from "react";
import { useNotifications } from "../notifications/NotificationsContext";
import { timeAgo } from "../lib/format";

export function Notifications() {
  const { items, unread, refresh, markRead, markAllRead } = useNotifications();

  useEffect(() => {
    refresh();
  }, [refresh]);

  return (
    <div className="container page">
      <header className="page-header page-header--row">
        <div>
          <h1>Bildirimler</h1>
          <p className="muted">{unread > 0 ? `${unread} okunmamış bildirimin var.` : "Tüm bildirimlerini okudun."}</p>
        </div>
        {unread > 0 && (
          <button type="button" className="button" onClick={markAllRead}>Tümünü okundu işaretle</button>
        )}
      </header>

      {items === null && <div className="skeleton" style={{ height: 200 }} />}

      {items?.length === 0 && (
        <div className="panel empty-card">
          <h2>Bildirimin yok</h2>
          <p className="muted">Yeni bir ilan yayınlandığında burada haber vereceğiz.</p>
        </div>
      )}

      {items && items.length > 0 && (
        <ul className="panel list">
          {items.map((notification) => (
            <li key={notification.id} className={`list__item notification${notification.isRead ? "" : " notification--unread"}`}>
              <span className="notification__dot" aria-hidden="true" />
              <div className="notification__main">
                <p className="notification__title">{notification.title}</p>
                <p className="muted">{notification.message}</p>
              </div>
              <div className="notification__side">
                <span className="muted">{timeAgo(notification.createdAt)}</span>
                {!notification.isRead && (
                  <button type="button" className="button button--ghost button--small" onClick={() => markRead(notification.id)}>
                    Okundu işaretle
                  </button>
                )}
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
