import { createContext, useContext, useEffect, useState, ReactNode } from 'react';
import { useAuth } from './AuthContext';
import { authApi } from '../services/api';
import { MeInfo, Role, ROLES } from '../types/infrastructure';

// MeContext loads the full identity payload once after login — roles +
// assigned stations are needed by the sidebar + route guards + filters.
// AuthContext stays focused on the login flow itself.

interface MeContextType {
  me: MeInfo | null;
  isLoading: boolean;
  hasRole: (...roles: Role[]) => boolean;
  isAdmin: boolean;
  reload: () => Promise<void>;
}

const MeContext = createContext<MeContextType | undefined>(undefined);

export const MeProvider = ({ children }: { children: ReactNode }) => {
  const { isAuthenticated } = useAuth();
  const [me, setMe] = useState<MeInfo | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  const load = async () => {
    setIsLoading(true);
    try {
      const data = await authApi.me();
      setMe(data);
    } catch {
      setMe(null);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    if (isAuthenticated) load();
    else {
      setMe(null);
      setIsLoading(false);
    }
  }, [isAuthenticated]);

  const hasRole = (...roles: Role[]) =>
    !!me && roles.some((r) => me.roles.includes(r));

  const isAdmin = hasRole(ROLES.SuperAdmin, ROLES.Admin);

  return (
    <MeContext.Provider value={{ me, isLoading, hasRole, isAdmin, reload: load }}>
      {children}
    </MeContext.Provider>
  );
};

export const useMe = () => {
  const ctx = useContext(MeContext);
  if (!ctx) throw new Error('useMe must be used within a MeProvider');
  return ctx;
};
