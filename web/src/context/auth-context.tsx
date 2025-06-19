import { LOCAL_STORAGE_KEYS } from '@/common/constants';
import { UserRole } from '@/common/types';
import { decodeToken } from '@/lib/jwt';
import { setAuthTokens } from '@/lib/utils';
import { router } from '@/main';
import API from '@/services/api';
import * as React from 'react';
import { toast } from 'sonner';

export interface AuthContextType {
  isAuthenticated: boolean;
  login: (email: string, password: string, redirect: string) => Promise<void>;
  logout: () => Promise<void>;
  userRole: UserRole;
  email: string | null;
  isNgoAdmin: boolean;
  isPlatformAdmin: boolean;
}

export const AuthContext = React.createContext<AuthContextType>({} as AuthContextType);

export function AuthContextProvider({ children }: { children: React.ReactNode }) {
  const [isAuthenticated, setIsAuthenticated] = React.useState<boolean>(false);
  const [userRole, setUserRole] = React.useState<UserRole>('NoOne');
  const [email, setEmail] = React.useState<string | null>(null);
  const [loading, setLoading] = React.useState(true);
  const [isNgoAdmin, setIsNgoAdmin] = React.useState(false);
  const [isPlatformAdmin, setIsPlatformAdmin] = React.useState(false);

  React.useEffect(() => {
    processToken();
  }, []);

  const processToken = () => {
    try {
      const token = localStorage.getItem(LOCAL_STORAGE_KEYS.ACCESS_TOKEN);
      if (token) {
        const decodedToken = decodeToken(token);
        setUserRole(decodedToken.role);
        setEmail(decodedToken.email);
        setIsNgoAdmin(decodedToken.role === 'NgoAdmin');
        setIsPlatformAdmin(decodedToken.role === 'PlatformAdmin');
      }
      setIsAuthenticated(!!token);
    } catch (err) {
      console.error(err);
      localStorage.removeItem(LOCAL_STORAGE_KEYS.ACCESS_TOKEN);
      localStorage.removeItem(LOCAL_STORAGE_KEYS.REFRESH_TOKEN);
      localStorage.removeItem(LOCAL_STORAGE_KEYS.REFRESH_TOKEN_EXPIRY_TIME);
    } finally {
      setLoading(false);
    }
  };

  const logout = React.useCallback(async () => {
    setIsAuthenticated(false);
    localStorage.removeItem(LOCAL_STORAGE_KEYS.ACCESS_TOKEN);
    localStorage.removeItem(LOCAL_STORAGE_KEYS.REFRESH_TOKEN);
    localStorage.removeItem(LOCAL_STORAGE_KEYS.REFRESH_TOKEN_EXPIRY_TIME);
  }, []);

  const login = async (email: string, password: string, redirect: string) => {
    try {
      const {
        data: { token, refreshToken, refreshTokenExpiryTime },
      } = await API.post('auth/login', {
        email,
        password,
      });

      setAuthTokens(token, refreshToken, refreshTokenExpiryTime);
      processToken();
      router.navigate({ to: redirect });
    } catch (err: unknown) {
      toast.error('Invalid credentials provided');
      console.log('Error while trying to sign in', err);
      setIsAuthenticated(false);
      throw new Error('Error while trying to sign in');
    }
  };

  return (
    <AuthContext.Provider value={{ isAuthenticated, userRole, email, isNgoAdmin, isPlatformAdmin, login, logout }}>
      {!loading && children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = React.useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}
