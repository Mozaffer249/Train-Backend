import { useState } from 'react';
import { useNavigate, useLocation, Link } from 'react-router-dom';
import { Train, Loader2 } from 'lucide-react';
import { useAuth } from '../contexts/AuthContext';
import { useLanguage } from '../contexts/LanguageContext';

interface LocationState {
  from?: string;
  notice?: string;
  email?: string;
}

export default function Login() {
  const { login } = useAuth();
  const { t } = useLanguage();
  const navigate = useNavigate();
  const location = useLocation();
  const state = (location.state as LocationState) || {};

  const [email, setEmail] = useState(state.email || '');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const notice = state.notice || '';
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      await login(email, password);
      navigate(state.from || '/dashboard', { replace: true });
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
            <Train className="h-7 w-7" />
          </div>
          <h1 className="text-2xl font-bold text-gray-900">{t('sign.in')}</h1>
        </div>

        {notice && <p className="bg-green-50 text-green-700 text-sm rounded-lg p-3 mb-4">{notice}</p>}
        {error && <p className="bg-red-50 text-red-600 text-sm rounded-lg p-3 mb-4">{error}</p>}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">{t('email.address')}</label>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-sudan-green-500 focus:border-sudan-green-500"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">{t('password')}</label>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-sudan-green-500 focus:border-sudan-green-500"
            />
          </div>

          <div className="text-right">
            <Link to="/forgot-password" className="text-sm text-sudan-green-600 hover:underline">
              {t('forgot.password')}
            </Link>
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full bg-sudan-green-600 text-white py-2.5 rounded-lg font-medium hover:bg-sudan-green-700 disabled:opacity-60 flex items-center justify-center gap-2"
          >
            {loading && <Loader2 className="h-4 w-4 animate-spin" />}
            {loading ? t('processing') : t('sign.in')}
          </button>
        </form>

        <p className="text-center text-sm text-gray-600 mt-6">
          {t('dont.have.account')}{' '}
          <Link to="/register" className="text-sudan-green-600 font-medium hover:underline">
            {t('sign.up')}
          </Link>
        </p>
      </div>
    </div>
  );
}
