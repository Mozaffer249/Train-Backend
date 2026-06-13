import { useEffect, useState } from 'react';
import { refundsApi } from '../services/api';
import { Refund, REFUND_STATUSES } from '../types/infrastructure';
import { AR } from '../i18n/ar';
import { showConfirm, showSuccess, showError, extractErrorMessage } from '../utils/alerts';

const RefundsPage = () => {
  const [refunds, setRefunds] = useState<Refund[]>([]);
  const [loading, setLoading] = useState(false);
  const [statusFilter, setStatusFilter] = useState<string>('');

  const load = async () => {
    setLoading(true);
    try {
      const data = await refundsApi.getAll(statusFilter || undefined);
      setRefunds(data);
    } catch (err) {
      showError(AR.common.errorTitle, extractErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, [statusFilter]);

  const handleApprove = async (r: Refund) => {
    const ok = await showConfirm(AR.refunds.confirmApprove, `${r.refundNumber} — ${r.amount} ${r.currency}`);
    if (!ok) return;
    try {
      await refundsApi.approve(r.id);
      await showSuccess(AR.refunds.approved);
      load();
    } catch (err) {
      showError(AR.common.errorTitle, extractErrorMessage(err));
    }
  };

  const handleReject = async (r: Refund) => {
    const ok = await showConfirm(AR.refunds.confirmReject, `${r.refundNumber} — ${r.amount} ${r.currency}`);
    if (!ok) return;
    try {
      await refundsApi.reject(r.id);
      await showSuccess(AR.refunds.rejected);
      load();
    } catch (err) {
      showError(AR.common.errorTitle, extractErrorMessage(err));
    }
  };

  return (
    <div>
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-900">{AR.refunds.title}</h1>
        <p className="text-gray-600">{AR.refunds.subtitle}</p>
      </div>

      <div className="admin-card mb-4 p-4 flex items-center gap-3">
        <label className="text-sm text-gray-700">{AR.common.filter}</label>
        <select
          value={statusFilter}
          onChange={(e) => setStatusFilter(e.target.value)}
          className="border rounded-md px-3 py-1.5 text-sm"
        >
          <option value="">{AR.common.none}</option>
          {REFUND_STATUSES.map((s) => (
            <option key={s} value={s}>{AR.status[s] || s}</option>
          ))}
        </select>
      </div>

      <div className="admin-card overflow-hidden">
        <table className="min-w-full text-sm">
          <thead className="bg-gray-50 text-gray-700">
            <tr>
              <th className="px-4 py-3 text-start">{AR.refunds.refundNumber}</th>
              <th className="px-4 py-3 text-start">{AR.refunds.booking}</th>
              <th className="px-4 py-3 text-start">{AR.refunds.customer}</th>
              <th className="px-4 py-3 text-start">{AR.refunds.amount}</th>
              <th className="px-4 py-3 text-start">{AR.refunds.method}</th>
              <th className="px-4 py-3 text-start">{AR.refunds.reason}</th>
              <th className="px-4 py-3 text-start">{AR.common.actions}</th>
            </tr>
          </thead>
          <tbody>
            {loading && (
              <tr><td colSpan={7} className="px-4 py-6 text-center text-gray-500">{AR.common.loading}</td></tr>
            )}
            {!loading && refunds.length === 0 && (
              <tr><td colSpan={7} className="px-4 py-6 text-center text-gray-500">{AR.common.none}</td></tr>
            )}
            {refunds.map((r) => (
              <tr key={r.id} className="border-t">
                <td className="px-4 py-3 font-mono">{r.refundNumber}</td>
                <td className="px-4 py-3">{r.bookingReference}</td>
                <td className="px-4 py-3">{r.userFullName || '—'}</td>
                <td className="px-4 py-3">{r.amount} {r.currency}</td>
                <td className="px-4 py-3">{r.method}</td>
                <td className="px-4 py-3 max-w-xs truncate" title={r.reason || ''}>{r.reason || '—'}</td>
                <td className="px-4 py-3">
                  <span className="me-2 px-2 py-0.5 rounded bg-gray-100 text-xs">{AR.status[r.status] || r.status}</span>
                  {r.status === 'Pending' && (
                    <span className="space-x-1">
                      <button className="admin-button text-xs px-3 py-1" onClick={() => handleApprove(r)}>{AR.refunds.approve}</button>
                      <button className="admin-button-secondary text-xs px-3 py-1" onClick={() => handleReject(r)}>{AR.refunds.reject}</button>
                    </span>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default RefundsPage;
