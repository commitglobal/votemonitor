import { ILoginResponse, LoginDTO, authApi } from '@/common/auth-api';
import { useToast } from '@/components/ui/use-toast';
import { parseJwt } from '@/lib/utils';
import { useNavigate } from '@tanstack/react-router';
import { createContext, useEffect, useState } from 'react';

export type AuthContextType = {
  signIn: (user: LoginDTO) => Promise<boolean>;
  signOut: () => void;
  isAuthenticated: boolean;
  isLoading: boolean;
  token: string | undefined;
  userRole: string | undefined;
};

export const AuthContext = createContext<AuthContextType>({
  signIn: () => new Promise(() => false),
  signOut: () => { },
  isAuthenticated: true,
  isLoading: false,
  token: undefined,
  userRole: undefined
});

const AuthContextProvider = ({ children }: React.PropsWithChildren) => {
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
  const [token, setToken] = useState<string>('');
  const [userRole, setUserRole] = useState<string>('Unknown');
  const { toast } = useToast();

  useEffect(() => {
    const token = localStorage.getItem('token');
    setIsAuthenticated(!!token);
    setIsLoading(false);
    if (token) {
      setToken(token);
      setUserRole(parseJwt(token)[`user-role`]);
    }
  }, []);

  const signIn = async (user: LoginDTO): Promise<boolean> => {
    try {
      const response = await authApi.post<ILoginResponse>('auth/login', user);
      localStorage.setItem('token', response.data.token);
      setIsAuthenticated(true);
      setToken(response.data.token);
      setUserRole(response.data.role);
      return true;
    } catch (error: any) {
      if (error.response.status === 400) {
        toast({
          title: 'Error',
          description: 'You have tered an invalid email or password',
          variant: 'destructive',
        });
      }
      return false;
    }
  };

  const signOut = (): void => {
    localStorage.removeItem('token');
    setIsAuthenticated(false);
    setToken('');
    setUserRole('');
  };

  return (
    <AuthContext.Provider
      value={{
        signIn,
        signOut,
        isAuthenticated,
        isLoading,
        token,
        userRole,
      }}>
      {children}
    </AuthContext.Provider>
  );
};

export default AuthContextProvider;
