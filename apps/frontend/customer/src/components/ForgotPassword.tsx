import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { KeyRound, Loader2 } from 'lucide-react';
import { authApi } from '../services/api';
import { useLanguage } from '../contexts/LanguageContext';

export default function ForgotPassword() {
  const { t } = useLanguage();
  const navigate = useNavigate();

  const [phase, setPhase] = useState<'request' | 'reset'>('request');
  const [email, setEmail] = useState('');
  const [resetCode, setResetCode] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const [notice, setNotice] = useState('');
  const [loading, setLoading] = useState(false);

  const inputClass =
    'w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-sudan-green-500 focus:border-sudan-green-500';

  const sendCode = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      await authApi.sendResetCode(email);
      setNotice(t('reset.code.sent'));
      setPhase('reset');
    } catch (err) {
      setError(err instanceof Error ? err.message : t('error'));
    } finally {
      setLoading(false);
    }
  };

  const reset = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    if (newPassword !== confirmPassword) {
      setError(t('confirm.password'));
      return;
    }
    setLoading(true);
    try {
      await authApi.resetPassword({ email, resetCode, newPassword, confirmPassword });
      navigate('/login', { state: { notice: t('success') } });
    } catch (err) {
      setError(err instanceof Error ? err.message : t('error'));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-[80vh] flex items-center justify-center px-4 py-12 bg-gray-50">
      <div className="w-full max-w-md bg-white rounded-2xl shadow-lg p-8">
        <div className="flex flex-col items-center mb-6">
          <div className="bg-sudan-green-600 text-white p-3 rounded-xl mb-3">
            <KeyRound className="h-7 w-7" />
          </div>
          <h1 className="text-2xl font-bold text-gray-900">{t('reset.password.title')}</h1>
        </div>

        {notice && <p className="bg-green-50 text-green-700 text-sm rounded-lg p-3 mb-4">{notice}</p>}
        {error && <p className="bg-red-50 text-red-600 text-sm rounded-lg p-3 mb-4">{error}</p>}

        {phase === 'request' ? (
          <form onSubmit={sendCode} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">{t('email.address')}</label>
              <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} required className={inputClass} />
            </div>
            <button
              type="submit"
              disabled={loading}
              className="w-full bg-sudan-green-600 text-white py-2.5 rounded-lg font-medium hover:bg-sudan-green-700 disabled:opacity-60 flex items-center justify-center gap-2"
            >
              {loading && <Loader2 className="h-4 w-4 animate-spin" />}
              {loading ? t('processing') : t('send.reset.code')}
            </button>
          </form>
        ) : (
          <form onSubmit={reset} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">{t('reset.code')}</label>
              <input type="text" value={resetCode} onChange={(e) => setResetCode(e.target.value)} required className={inputClass} />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">{t('new.password')}</label>
              <input type="password" value={newPassword} onChange={(e) => setNewPassword(e.target.value)} required className={inputClass} />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">{t('confirm.password')}</label>
              <input type="password" value={confirmPassword} onChange={(e) => setConfirmPassword(e.target.value)} required className={inputClass} />
            </div>
            <button
              type="submit"
              disabled={loading}
              className="w-full bg-sudan-green-600 text-white py-2.5 rounded-lg font-medium hover:bg-sudan-green-700 disabled:opacity-60 flex items-center justify-center gap-2"
            >
              {loading && <Loader2 className="h-4 w-4 animate-spin" />}
              {loading ? t('processing') : t('reset.password.title')}
            </button>
          </form>
        )}

        <p className="text-center text-sm text-gray-600 mt-6">
          <Link to="/login" className="text-sudan-green-600 font-medium hover:underline">
            {t('sign.in')}
          </Link>
        </p>
      </div>
    </div>
  );
}
