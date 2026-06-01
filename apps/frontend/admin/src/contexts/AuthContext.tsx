import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';

interface User {
  id: string;
  name: string;
  email: string;
  role: 'Admin' | 'Staff';
}

interface LoginResponse {
  statusCode: number;
  succeeded: boolean;
  message: string | null;
  data: {
    accessToken: string;
    refreshToken: {
      userName: string;
      tokenString: string;
      expireAt: string;
    };
    requiresTwoFactor: boolean;
    isNewDevice: boolean;
    deviceId: string | null;
    userId: number;
    userName: string;
    email: string;
    fullName: string | null;
    roles: string[];
  };
  errors: string[] | null;
  meta: any;
}

interface AuthContextType {
  user: User | null;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
  isAuthenticated: boolean;
  isLoading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    // Check for stored auth token on mount
    const checkAuth = () => {
      const token = localStorage.getItem('admin_token');
      const storedUser = localStorage.getItem('admin_user');
      
      if (token && storedUser) {
        try {
          const parsedUser = JSON.parse(storedUser);
          // Validate that user has admin/staff role
          if (parsedUser.role === 'Admin' || parsedUser.role === 'Staff') {
            setUser(parsedUser);
          } else {
            // Invalid role, clear storage
            localStorage.removeItem('admin_token');
            localStorage.removeItem('admin_user');
          }
        } catch (error) {
          console.error('Failed to parse stored user', error);
          localStorage.removeItem('admin_token');
          localStorage.removeItem('admin_user');
        }
      }
      setIsLoading(false);
    };

    checkAuth();
  }, []);

  const login = async (email: string, password: string) => {
    try {
      const apiUrl = import.meta.env.VITE_API_URL || 'http://localhost:8081';
      const response = await fetch(`${apiUrl}/Api/V1/Authentication/Login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ userNameOrEmail: email, password }),
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Authentication failed');
      }

      const result: LoginResponse = await response.json();
      
      // Check if response succeeded
      if (!result.succeeded) {
        throw new Error(result.message || 'Authentication failed');
      }
      
      // Extract data from Response<JwtAuthResult>
      const authData = result.data;
      
      // Validate that user has admin or staff role
      const hasAdminAccess = authData.roles?.some(
        (role: string) => role === 'SuperAdmin' || role === 'Admin' || role === 'Staff'
      );
      
      if (!hasAdminAccess) {
        throw new Error('Insufficient permissions. Admin or Staff access required.');
      }

      const adminUser: User = {
        id: authData.userId.toString(),
        name: authData.fullName || authData.userName,
        email: authData.email,
        role: authData.roles.includes('SuperAdmin') ? 'Admin' : 'Staff',
      };

      // Store auth data
      localStorage.setItem('admin_token', authData.accessToken);
      localStorage.setItem('admin_user', JSON.stringify(adminUser));
      setUser(adminUser);
    } catch (error) {
      console.error('Login error:', error);
      throw error;
    }
  };

  const logout = () => {
    localStorage.removeItem('admin_token');
    localStorage.removeItem('admin_user');
    setUser(null);
  };

  const isAuthenticated = !!user;

  return (
    <AuthContext.Provider value={{ user, login, logout, isAuthenticated, isLoading }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
