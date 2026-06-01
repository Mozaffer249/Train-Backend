import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { UserPlus, Loader2 } from 'lucide-react';
import { authApi } from '../services/api';
import { useLanguage } from '../contexts/LanguageContext';

export default function Register() {
  const { t } = useLanguage();
  const navigate = useNavigate();

  const [form, setForm] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: '',
    password: '',
    confirmPassword: '',
  });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const update = (key: keyof typeof form) => (e: React.ChangeEvent<HTMLInputElement>) =>
    setForm({ ...form, [key]: e.target.value });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    if (form.password !== form.confirmPassword) {
      setError(t('confirm.password'));
      return;
    }
    setLoading(true);
    try {
      const result = await authApi.register(form);
      navigate('/confirm-email', {
        state: {
          notice: result?.resumeConfirmation
            ? t('registration.resume')
            : t('registration.success'),
          userId: result?.userId,
          email: result?.email ?? form.email,
        },
      });
    } catch (err) {
      setError(err instanceof Error ? err.message : t('error'));
    } finally {
      setLoading(false);
    }
  };

  const inputClass =
    'w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-sudan-green-500 focus:border-sudan-green-500';

  return (
    <div className="min-h-[80vh] flex items-center justify-center px-4 py-12 bg-gray-50">
      <div className="w-full max-w-md bg-white rounded-2xl shadow-lg p-8">
        <div className="flex flex-col items-center mb-6">
          <div className="bg-sudan-green-600 text-white p-3 rounded-xl mb-3">
            <UserPlus className="h-7 w-7" />
          </div>
          <h1 className="text-2xl font-bold text-gray-900">{t('create.account')}</h1>
        </div>

        {error && <p className="bg-red-50 text-red-600 text-sm rounded-lg p-3 mb-4">{error}</p>}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">{t('first.name')}</label>
              <input type="text" value={form.firstName} onChange={update('firstName')} required className={inputClass} />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">{t('last.name')}</label>
              <input type="text" value={form.lastName} onChange={update('lastName')} required className={inputClass} />
            </div>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">{t('email.address')}</label>
            <input type="email" value={form.email} onChange={update('email')} required className={inputClass} />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              {t('phone.number')} <span className="text-gray-400">({t('optional')})</span>
            </label>
            <input type="tel" value={form.phoneNumber} onChange={update('phoneNumber')} className={inputClass} />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">{t('password')}</label>
            <input type="password" value={form.password} onChange={update('password')} required className={inputClass} />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">{t('confirm.password')}</label>
            <input type="password" value={form.confirmPassword} onChange={update('confirmPassword')} required className={inputClass} />
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full bg-sudan-green-600 text-white py-2.5 rounded-lg font-medium hover:bg-sudan-green-700 disabled:opacity-60 flex items-center justify-center gap-2"
          >
            {loading && <Loader2 className="h-4 w-4 animate-spin" />}
            {loading ? t('processing') : t('register')}
          </button>
        </form>

        <p className="text-center text-sm text-gray-600 mt-6">
          {t('already.have.account')}{' '}
          <Link to="/login" className="text-sudan-green-600 font-medium hover:underline">
            {t('sign.in')}
          </Link>
        </p>
      </div>
    </div>
  );
}
