import { useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { Train, User, Menu, X, LogOut } from 'lucide-react';
import { useLanguage } from '../contexts/LanguageContext';
import { useAuth } from '../contexts/AuthContext';
import BrandStripe from './BrandStripe';

export default function Header() {
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const { t } = useLanguage();
  const { user, logout, isAuthenticated } = useAuth();
  const location = useLocation();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  const navigation = [
    { name: t('home'), path: '/' },
    { name: t('search'), path: '/search' },
    { name: t('dashboard'), path: '/dashboard' },
  ];

  return (
    <>
      <BrandStripe />
      <header className="bg-sudan-green-700 text-white shadow-md">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 flex justify-between items-center h-16">
          <Link to="/" className="flex items-center gap-2">
            <div className="bg-sudan-gold-400 p-2 rounded-lg">
              <Train className="h-6 w-6 text-sudan-green-800" />
            </div>
            <div className="flex flex-col leading-tight">
              <h1 className="text-lg font-bold">{t('brand.name')}</h1>
              <span className="text-[10px] text-sudan-gold-200 hidden sm:inline">{t('brand.tagline')}</span>
            </div>
          </Link>

          <nav className="hidden md:flex gap-8">
            {navigation.map((item) => (
              <Link
                key={item.path}
                to={item.path}
                className={`px-1 py-2 text-sm font-medium transition-colors ${
                  location.pathname === item.path
                    ? 'text-white border-b-2 border-sudan-gold-400'
                    
                    : 'text-sudan-green-50 hover:text-white'
                }`}
              >
                {item.name}
              </Link>
            ))}
          </nav>

          <div className="flex items-center gap-3">
            {isAuthenticated ? (
              <div className="flex items-center gap-3">
                <span className="hidden sm:flex items-center gap-1 text-sm text-sudan-green-50">
                  <User className="h-4 w-4" /> {user?.name}
                </span>
                <button
                  onClick={handleLogout}
                  className="flex items-center gap-1 text-sm text-sudan-green-50 hover:text-white transition-colors"
                >
                  <LogOut className="h-4 w-4" /> {t('logout')}
                </button>
              </div>
            ) : (
              <Link
                to="/login"
                className="bg-sudan-gold-400 text-sudan-green-900 px-4 py-2 rounded-lg text-sm font-medium hover:bg-sudan-gold-300 transition-colors"
              >
                {t('login')}
              </Link>
            )}

            <button
              onClick={() => setIsMenuOpen(!isMenuOpen)}
              className="md:hidden p-2 text-sudan-green-50 hover:text-white transition-colors"
            >
              {isMenuOpen ? <X className="h-5 w-5" /> : <Menu className="h-5 w-5" />}
            </button>
          </div>
        </div>

        {isMenuOpen && (
          <div className="md:hidden py-2 border-t border-sudan-green-800">
            <nav className="flex flex-col px-4">
              {navigation.map((item) => (
                <Link
                  key={item.path}
                  to={item.path}
                  className={`px-3 py-2 text-sm font-medium rounded-md transition-colors ${
                    location.pathname === item.path
                      ? 'bg-sudan-green-800 text-white'
                      : 'text-sudan-green-50 hover:bg-sudan-green-800'
                  }`}
                  onClick={() => setIsMenuOpen(false)}
                >
                  {item.name}
                </Link>
              ))}
            </nav>
          </div>
        )}
      </header>
    </>
  );
}
