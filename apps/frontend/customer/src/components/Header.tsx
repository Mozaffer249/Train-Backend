import React, { useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { Train, User, Menu, X, Globe, Sun, Moon } from 'lucide-react';
import { useLanguage } from '../contexts/LanguageContext';
import { useAuth } from '../contexts/AuthContext';

const Header: React.FC = () => {
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const [isDarkMode, setIsDarkMode] = useState(false);
  const { language, setLanguage, t } = useLanguage();
  const { user, logout, isAuthenticated } = useAuth();
  const location = useLocation();

  const toggleLanguage = () => {
    setLanguage(language === 'ar' ? 'en' : 'ar');
  };

  const toggleDarkMode = () => {
    setIsDarkMode(!isDarkMode);
  };

  const navigation = [
    { name: t('home'), path: '/' },
    { name: t('search'), path: '/search' },
    { name: t('dashboard'), path: '/dashboard' },
    ...(user?.role === 'admin' ? [{ name: t('admin'), path: '/admin' }] : []),
  ];

  return (
    <header className="bg-blue-600 text-white p-4">
      <div className="max-w-7xl mx-auto flex justify-between items-center">
        {/* Logo */}
        <Link to="/" className="flex items-center space-x-2 rtl:space-x-reverse">
          <div className="bg-white p-2 rounded-lg">
            <Train className="h-6 w-6 text-blue-600" />
          </div>
          <h1 className="text-lg font-bold">{t('atbara.rail')}</h1>
        </Link>

        {/* Desktop Navigation */}
        <nav className="hidden md:flex space-x-8 rtl:space-x-reverse">
          {navigation.map((item) => (
            <Link
              key={item.path}
              to={item.path}
              className={`px-3 py-2 text-sm font-medium transition-colors ${
                location.pathname === item.path
                  ? 'text-white border-b-2 border-white'
                  : 'text-gray-200 hover:text-white'
              }`}
            >
              {item.name}
            </Link>
          ))}
        </nav>

        {/* Actions */}
        <div className="flex items-center space-x-4 rtl:space-x-reverse">
          {/* Language Toggle */}
          <button
            onClick={toggleLanguage}
            className="p-2 text-gray-200 hover:text-white transition-colors"
            title={language === 'ar' ? 'English' : 'العربية'}
          >
            <Globe className="h-5 w-5" />
          </button>

          {/* Dark Mode Toggle */}
          <button
            onClick={toggleDarkMode}
            className="p-2 text-gray-200 hover:text-white transition-colors"
          >
            {isDarkMode ? <Sun className="h-5 w-5" /> : <Moon className="h-5 w-5" />}
          </button>

          {/* User Menu */}
          {isAuthenticated ? (
            <div className="relative">
              <button className="flex items-center space-x-2 rtl:space-x-reverse text-gray-200 hover:text-white transition-colors">
                <User className="h-5 w-5" />
                <span className="text-sm">{user?.name}</span>
              </button>
              <button
                onClick={logout}
                className="ml-4 rtl:ml-0 rtl:mr-4 text-sm text-gray-200 hover:text-white transition-colors"
              >
                {t('logout')}
              </button>
            </div>
          ) : (
            <Link
              to="/login"
              className="bg-white text-blue-600 px-4 py-2 rounded-lg text-sm font-medium hover:bg-gray-100 transition-colors"
            >
              {t('login')}
            </Link>
          )}

          {/* Mobile Menu Button */}
          <button
            onClick={() => setIsMenuOpen(!isMenuOpen)}
            className="md:hidden p-2 text-gray-200 hover:text-white transition-colors"
          >
            {isMenuOpen ? <X className="h-5 w-5" /> : <Menu className="h-5 w-5" />}
          </button>
        </div>
      </div>

      {/* Mobile Navigation */}
      {isMenuOpen && (
        <div className="md:hidden py-4 border-t border-gray-700">
          <nav className="flex flex-col space-y-2">
            {navigation.map((item) => (
              <Link
                key={item.path}
                to={item.path}
                className={`px-3 py-2 text-sm font-medium transition-colors ${
                  location.pathname === item.path
                    ? 'text-blue-600 bg-blue-50'
                    : 'text-gray-200 hover:text-white hover:bg-gray-700'
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
  );
};

export default Header;