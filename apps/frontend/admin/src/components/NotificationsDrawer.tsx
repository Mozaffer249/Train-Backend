import { useEffect, useState } from 'react';
import { Bell, X } from 'lucide-react';
import { notificationsApi } from '../services/api';
import { Notification } from '../types/infrastructure';
import { AR } from '../i18n/ar';

const typeLabel = (type: string) => {
  const key = type as keyof typeof AR.notificationTypes;
  return AR.notificationTypes[key] ?? type;
};

export default function NotificationsDrawer() {
  const [open, setOpen] = useState(false);
  const [items, setItems] = useState<Notification[]>([]);
  const [loading, setLoading] = useState(false);
  const [unread, setUnread] = useState(0);

  const refreshCount = () => {
    notificationsApi.mine(true).then((r) => setUnread(r.length)).catch(() => {});
  };

  useEffect(() => {
    refreshCount();
  }, []);

  const openDrawer = async () => {
    setOpen(true);
    setLoading(true);
    try {
      const data = await notificationsApi.mine();
      setItems(data);
      await Promise.all(
        data.filter((n) => !n.isRead).map((n) =>
          notificationsApi.markRead(n.id).catch(() => null),
        ),
      );
      setUnread(0);
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <button
        type="button"
        onClick={openDrawer}
        className="relative p-2 text-gray-600 hover:bg-gray-100 rounded-lg transition-colors"
        aria-label={AR.notifications.title}
      >
        <Bell size={20} />
        {unread > 0 && (
          <span className="absolute top-0 end-0 inline-flex items-center justify-center min-w-[18px] h-[18px] text-[10px] font-bold text-white bg-red-600 rounded-full px-1">
            {unread}
          </span>
        )}
      </button>

      {open && (
        <div className="fixed inset-0 z-50 flex" onClick={() => setOpen(false)}>
          <div className="flex-1 bg-black/40" />
          <div
            className="w-full sm:w-96 bg-white shadow-xl h-full overflow-y-auto"
            onClick={(e) => e.stopPropagation()}
          >
            <div className="flex items-center justify-between p-4 border-b">
              <h2 className="font-bold text-gray-900">{AR.notifications.title}</h2>
              <button onClick={() => setOpen(false)} className="text-gray-400 hover:text-gray-600">
                <X size={20} />
              </button>
            </div>

            {loading && (
              <div className="p-6 text-center text-sm text-gray-500">{AR.common.loading}</div>
            )}
            {!loading && items.length === 0 && (
              <div className="p-6 text-center text-sm text-gray-500">{AR.notifications.empty}</div>
            )}

            <ul className="divide-y">
              {items.map((n) => (
                <li key={n.id} className={`p-4 ${n.isRead ? '' : 'bg-admin-primary-50'}`}>
                  <div className="flex items-baseline justify-between mb-1">
                    <span className="text-xs text-gray-500">{typeLabel(n.type)}</span>
                    <span className="text-[11px] text-gray-400">
                      {new Date(n.createdAt).toLocaleString('ar')}
                    </span>
                  </div>
                  <div className="font-semibold text-sm text-gray-900">{n.subject}</div>
                  <div className="text-sm text-gray-700 mt-1">{n.message}</div>
                  {n.bookingReference && (
                    <div className="text-xs text-gray-500 mt-1 font-mono">{n.bookingReference}</div>
                  )}
                </li>
              ))}
            </ul>
          </div>
        </div>
      )}
    </>
  );
}
