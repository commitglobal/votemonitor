import { ILoginResponse, LoginDTO, authApi } from '@/common/auth-api';
import { createContext, useEffect, useState } from 'react';

export type AuthContextType = {
  signIn: (user: LoginDTO) => void;
  isAuthenticated: boolean;
  isLoading: boolean;
};

export const AuthContext = createContext<AuthContextType>({
  signIn: () => null,
  isAuthenticated: true,
  isLoading: false,
});

const AuthContextProvider = ({ children }: React.PropsWithChildren) => {
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);

  useEffect(() => {
    const token = localStorage.getItem('token');
    setIsAuthenticated(!!token);
    setIsLoading(false);
  }, []);

  const signIn = async (user: LoginDTO) => {
    const response = await authApi.post<ILoginResponse>('auth/login', user);
    localStorage.setItem('token', response.data.token);
    setIsAuthenticated(true);
    console.log('user autheticated');
  };

  return (
    <AuthContext.Provider
      value={{
        signIn,
        isAuthenticated,
        isLoading,
      }}>
      {children}
    </AuthContext.Provider>
  );
};

export default AuthContextProvider;
