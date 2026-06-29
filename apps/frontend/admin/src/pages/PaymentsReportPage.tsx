import { useCallback, useEffect, useState } from 'react';
import { ChevronLeft, ChevronRight, RefreshCw } from 'lucide-react';
import { paymentsApi } from '../services/api';
import { AR } from '../i18n/ar';
import { PAYMENT_METHODS, PAYMENT_STATUSES, PaymentReportItem } from '../types/infrastructure';
import { extractErrorMessage, showError } from '../utils/alerts';

const PAGE_SIZE = 20;

function formatDateTime(iso: string): string {
  const d = new Date(iso);
  if (isNaN(d.getTime())) return '--';
  return d.toLocaleString('ar', { dateStyle: 'short', timeStyle: 'short' });
}

function statusBadgeClass(status: string): string {
  switch (status) {
    case 'Completed': return 'bg-green-100 text-green-800';
    case 'Pending': return 'bg-yellow-100 text-yellow-800';
    case 'Failed': return 'bg-red-100 text-red-800';
    case 'Refunded': return 'bg-blue-100 text-blue-800';
    default: return 'bg-gray-100 text-gray-800';
  }
}

function methodLabel(method: string): string {
  return AR.payments.methods[method as keyof typeof AR.payments.methods] || method;
}

const PaymentsReportPage = () => {
  const [items, setItems] = useState<PaymentReportItem[]>([]);
  const [totalCollected, setTotalCollected] = useState(0);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(false);
  const [fromDate, setFromDate] = useState('');
  const [toDate, setToDate] = useState('');
  const [methodFilter, setMethodFilter] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [page, setPage] = useState(1);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const data = await paymentsApi.getReport({
        fromDate: fromDate || undefined,
        toDate: toDate || undefined,
        method: methodFilter || undefined,
        status: statusFilter || undefined,
        pageNumber: page,
        pageSize: PAGE_SIZE,
      });
      setItems(data.items);
      setTotalCollected(data.summary.totalCollected);
      setTotalCount(data.totalCount);
    } catch (err) {
      showError(AR.common.errorTitle, extractErrorMessage(err));
      setItems([]);
      setTotalCollected(0);
      setTotalCount(0);
    } finally {
      setLoading(false);
    }
  }, [fromDate, toDate, methodFilter, statusFilter, page]);

  useEffect(() => {
    load();
  }, [load]);

  const totalPages = Math.max(1, Math.ceil(totalCount / PAGE_SIZE));

  return (
    <div>
      <div className="mb-6 flex flex-wrap items-start justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">{AR.payments.title}</h1>
          <p className="text-gray-600">{AR.payments.subtitle}</p>
        </div>
        <button type="button" onClick={load} className="admin-button-secondary inline-flex items-center gap-2 px-4 py-2">
          <RefreshCw className="h-4 w-4" />
          {AR.common.filter}
        </button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
        <div className="admin-card">
          <p className="text-sm text-gray-600 mb-1">{AR.payments.totalCollected}</p>
          <p className="text-2xl font-bold text-gray-900">{loading ? '…' : `${Math.round(totalCollected).toLocaleString('ar')} SDG`}</p>
        </div>
        <div className="admin-card">
          <p className="text-sm text-gray-600 mb-1">{AR.payments.transactionCount}</p>
          <p className="text-2xl font-bold text-gray-900">{loading ? '…' : totalCount.toLocaleString('ar')}</p>
        </div>
      </div>

      <div className="admin-card mb-4 p-4 flex flex-wrap items-end gap-3">
        <div>
          <label className="block text-xs text-gray-600 mb-1">{AR.payments.dateFrom}</label>
          <input type="date" value={fromDate} onChange={(e) => { setFromDate(e.target.value); setPage(1); }} className="border rounded-md px-3 py-1.5 text-sm" />
        </div>
        <div>
          <label className="block text-xs text-gray-600 mb-1">{AR.payments.dateTo}</label>
          <input type="date" value={toDate} onChange={(e) => { setToDate(e.target.value); setPage(1); }} className="border rounded-md px-3 py-1.5 text-sm" />
        </div>
        <div>
          <label className="block text-xs text-gray-600 mb-1">{AR.payments.method}</label>
          <select value={methodFilter} onChange={(e) => { setMethodFilter(e.target.value); setPage(1); }} className="border rounded-md px-3 py-1.5 text-sm">
            <option value="">{AR.common.none}</option>
            {PAYMENT_METHODS.map((m) => (
              <option key={m} value={m}>{methodLabel(m)}</option>
            ))}
          </select>
        </div>
        <div>
          <label className="block text-xs text-gray-600 mb-1">{AR.payments.status}</label>
          <select value={statusFilter} onChange={(e) => { setStatusFilter(e.target.value); setPage(1); }} className="border rounded-md px-3 py-1.5 text-sm">
            <option value="">{AR.common.none}</option>
            {PAYMENT_STATUSES.map((s) => (
              <option key={s} value={s}>{AR.payments.statuses[s] || s}</option>
            ))}
          </select>
        </div>
      </div>

      <div className="admin-card overflow-hidden">
        <table className="min-w-full text-sm">
          <thead className="bg-gray-50 text-gray-700">
            <tr>
              <th className="px-4 py-3 text-start">{AR.payments.reference}</th>
              <th className="px-4 py-3 text-start">{AR.payments.booking}</th>
              <th className="px-4 py-3 text-start">{AR.payments.customer}</th>
              <th className="px-4 py-3 text-start">{AR.payments.method}</th>
              <th className="px-4 py-3 text-start">{AR.payments.status}</th>
              <th className="px-4 py-3 text-start">{AR.payments.amount}</th>
              <th className="px-4 py-3 text-start">{AR.payments.card}</th>
              <th className="px-4 py-3 text-start">{AR.payments.date}</th>
            </tr>
          </thead>
          <tbody>
            {loading && (
              <tr><td colSpan={8} className="px-4 py-6 text-center text-gray-500">{AR.common.loading}</td></tr>
            )}
            {!loading && items.length === 0 && (
              <tr><td colSpan={8} className="px-4 py-6 text-center text-gray-500">{AR.common.none}</td></tr>
            )}
            {items.map((p) => (
              <tr key={p.id} className="border-t">
                <td className="px-4 py-3 font-mono text-xs">{p.reference || '—'}</td>
                <td className="px-4 py-3 font-mono">{p.bookingRef}</td>
                <td className="px-4 py-3">{p.customerName || '—'}</td>
                <td className="px-4 py-3">{methodLabel(p.method)}</td>
                <td className="px-4 py-3">
                  <span className={`px-2 py-0.5 rounded text-xs ${statusBadgeClass(p.status)}`}>
                    {AR.payments.statuses[p.status] || p.status}
                  </span>
                </td>
                <td className="px-4 py-3">{Math.round(p.amount).toLocaleString('ar')} {p.currency}</td>
                <td className="px-4 py-3">
                  {p.cardBrand || p.cardLast4
                    ? `${p.cardBrand || ''}${p.cardBrand && p.cardLast4 ? ' · ' : ''}${p.cardLast4 ? `**** ${p.cardLast4}` : ''}`
                    : '—'}
                </td>
                <td className="px-4 py-3 whitespace-nowrap">{formatDateTime(p.createdAt)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {totalPages > 1 && (
        <div className="flex items-center justify-center gap-4 mt-4">
          <button
            type="button"
            disabled={page <= 1}
            onClick={() => setPage((p) => Math.max(1, p - 1))}
            className="admin-button-secondary p-2 disabled:opacity-40"
          >
            <ChevronRight className="h-4 w-4" />
          </button>
          <span className="text-sm text-gray-600">
            {page} / {totalPages}
          </span>
          <button
            type="button"
            disabled={page >= totalPages}
            onClick={() => setPage((p) => p + 1)}
            className="admin-button-secondary p-2 disabled:opacity-40"
          >
            <ChevronLeft className="h-4 w-4" />
          </button>
        </div>
      )}
    </div>
  );
};

export default PaymentsReportPage;
