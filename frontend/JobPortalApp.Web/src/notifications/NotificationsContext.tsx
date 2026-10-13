import { createContext, useCallback, useContext, useEffect, useMemo, useState, type ReactNode } from "react";
import { notificationsApi } from "../api/endpoints";
import type { AppNotification } from "../api/types";
import { useAuth } from "../auth/AuthContext";

interface NotificationsState {
  items: AppNotification[] | null;
  unread: number;
  refresh: () => Promise<void>;
  markRead: (id: string) => Promise<void>;
  markAllRead: () => Promise<void>;
}

const NotificationsContext = createContext<NotificationsState | null>(null);

const POLL_INTERVAL = 60_000;

export function NotificationsProvider({ children }: { children: ReactNode }) {
  const { user } = useAuth();
  const [items, setItems] = useState<AppNotification[] | null>(null);

  const refresh = useCallback(async () => {
    if (!user) {
      setItems(null);
      return;
    }
    try {
      setItems(await notificationsApi.mine());
    } catch {
      setItems((current) => current ?? []);
    }
  }, [user]);

  useEffect(() => {
    refresh();
    if (!user) return;
    const timer = window.setInterval(refresh, POLL_INTERVAL);
    return () => window.clearInterval(timer);
  }, [user, refresh]);

  const markRead = useCallback(async (id: string) => {
    setItems((current) => current?.map((n) => (n.id === id ? { ...n, isRead: true } : n)) ?? null);
    await notificationsApi.markRead(id);
  }, []);

  const markAllRead = useCallback(async () => {
    setItems((current) => current?.map((n) => ({ ...n, isRead: true })) ?? null);
    await notificationsApi.markAllRead();
  }, []);

  const value = useMemo(
    () => ({ items, unread: items?.filter((n) => !n.isRead).length ?? 0, refresh, markRead, markAllRead }),
    [items, refresh, markRead, markAllRead]
  );

  return <NotificationsContext.Provider value={value}>{children}</NotificationsContext.Provider>;
}

export function useNotifications() {
  const context = useContext(NotificationsContext);
  if (!context) throw new Error("useNotifications must be used inside NotificationsProvider");
  return context;
}
