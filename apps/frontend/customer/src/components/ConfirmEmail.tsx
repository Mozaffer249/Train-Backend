import { useState } from 'react';
import { useNavigate, useLocation, Link } from 'react-router-dom';
import { MailCheck, Loader2 } from 'lucide-react';
import { authApi } from '../services/api';
import { useLanguage } from '../contexts/LanguageContext';

interface LocationState {
  notice?: string;
  userId?: number;
  email?: string;
}

export default function ConfirmEmail() {
  const { t } = useLanguage();
  const navigate = useNavigate();
  const location = useLocation();
  const state = (location.state as LocationState) || {};
  const notice = state.notice || '';
  const prefilledEmail = state.email || '';
  const userId = state.userId;

  const [code, setCode] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!userId) return;
    setError('');
    setLoading(true);
    try {
      await authApi.confirmEmail(userId, code);
      navigate('/login', {
        state: { notice: t('email.confirmed.login'), email: prefilledEmail },
      });
    } catch (err) {
      setError(err instanceof Error ? err.message : t('error'));
    } finally {
      setLoading(false);
    }
  };

  const inputClass =
    'w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-sudan-green-500 focus:border-sudan-green-500';

  // No registration context — the user opened this page directly. Send them to /register.
  if (!userId) {
    return (
      <div className="min-h-[80vh] flex items-center justify-center px-4 py-12 bg-gray-50">
        <div className="w-full max-w-md bg-white rounded-2xl shadow-lg p-8 text-center">
          <div className="bg-sudan-green-600 text-white p-3 rounded-xl mb-3 inline-flex">
            <MailCheck className="h-7 w-7" />
          </div>
          <h1 className="text-2xl font-bold text-gray-900 mb-2">{t('confirm.email.title')}</h1>
          <p className="text-sm text-gray-500 mb-6">{t('confirm.email.no.session')}</p>
          <Link
            to="/register"
            className="inline-block bg-sudan-green-600 text-white px-4 py-2.5 rounded-lg font-medium hover:bg-sudan-green-700"
          >
            {t('sign.up')}
          </Link>
          <p className="text-center text-sm text-gray-600 mt-6">
            <Link to="/login" className="text-sudan-green-600 font-medium hover:underline">
              {t('sign.in')}
            </Link>
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-[80vh] flex items-center justify-center px-4 py-12 bg-gray-50">
      <div className="w-full max-w-md bg-white rounded-2xl shadow-lg p-8">
        <div className="flex flex-col items-center mb-6">
          <div className="bg-sudan-green-600 text-white p-3 rounded-xl mb-3">
            <MailCheck className="h-7 w-7" />
          </div>
          <h1 className="text-2xl font-bold text-gray-900">{t('confirm.email.title')}</h1>
          <p className="text-sm text-gray-500 text-center mt-2">{t('confirm.email.desc')}</p>
          {prefilledEmail && (
            <p className="text-sm text-gray-700 mt-2 font-medium" dir="ltr">{prefilledEmail}</p>
          )}
        </div>

        {notice && <p className="bg-green-50 text-green-700 text-sm rounded-lg p-3 mb-4">{notice}</p>}
        {error && <p className="bg-red-50 text-red-600 text-sm rounded-lg p-3 mb-4">{error}</p>}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">{t('verification.code')}</label>
            <input
              type="text"
              inputMode="numeric"
              maxLength={4}
              value={code}
              onChange={(e) => setCode(e.target.value.replace(/\D/g, ''))}
              required
              autoFocus
              className={`${inputClass} tracking-[0.5em] text-center text-lg`}
            />
          </div>

          <button
            type="submit"
            disabled={loading || code.length < 4}
            className="w-full bg-sudan-green-600 text-white py-2.5 rounded-lg font-medium hover:bg-sudan-green-700 disabled:opacity-60 flex items-center justify-center gap-2"
          >
            {loading && <Loader2 className="h-4 w-4 animate-spin" />}
            {loading ? t('processing') : t('verify')}
          </button>
        </form>

        <p className="text-center text-sm text-gray-600 mt-6">
          <Link to="/login" className="text-sudan-green-600 font-medium hover:underline">
            {t('sign.in')}
          </Link>
        </p>
      </div>
    </div>
  );
}
