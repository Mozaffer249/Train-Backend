import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { Train } from 'lucide-react';
import { AR } from '../i18n/ar';
import BrandStripe from '../components/BrandStripe';

const Login = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setIsLoading(true);

    try {
      await login(email, password);
      navigate('/dashboard');
    } catch (err) {
      const message = err instanceof Error ? err.message : AR.auth.notAuthorized;
      setError(message);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex flex-col bg-gradient-to-br from-admin-primary-700 to-admin-primary-900">
      <BrandStripe />
      <div className="flex-1 flex items-center justify-center px-4 py-12">
        <div className="bg-white p-8 rounded-lg shadow-2xl w-full max-w-md border-t-4 border-sudan-gold-400">
          <div className="flex items-center justify-center mb-8 gap-3">
            <div className="bg-sudan-gold-400 p-2 rounded-lg">
              <Train className="text-admin-primary-800" size={28} />
            </div>
            <div className="text-center">
              <h1 className="text-2xl font-bold text-gray-900">{AR.brand.name}</h1>
              <p className="text-sm text-gray-600">{AR.auth.portalTitle}</p>
            </div>
          </div>

          <form onSubmit={handleSubmit} className="space-y-6">
            {error && (
              <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">{error}</div>
            )}

            <div>
              <label htmlFor="email" className="block text-sm font-medium text-gray-700 mb-2">
                {AR.auth.emailOrUsername}
              </label>
              <input
                id="email"
                type="text"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                placeholder={AR.auth.emailPlaceholder}
                required
              />
            </div>

            <div>
              <label htmlFor="password" className="block text-sm font-medium text-gray-700 mb-2">
                {AR.auth.password}
              </label>
              <input
                id="password"
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500"
                placeholder={AR.auth.passwordPlaceholder}
                required
              />
            </div>

            <button
              type="submit"
              disabled={isLoading}
              className="w-full bg-admin-primary-700 text-white py-3 rounded-lg hover:bg-admin-primary-800 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {isLoading ? AR.auth.signingIn : AR.auth.signIn}
            </button>
          </form>

          <p className="mt-6 text-center text-sm text-gray-600">{AR.auth.notAuthorized}</p>
        </div>
      </div>
    </div>
  );
};

export default Login;
