import { STORAGE_KEYS } from "@/constants/storage-keys";
import { decodeToken } from "@/lib/jwt";
import { setAuthTokens } from "@/lib/utils";
import API from "@/services/api";
import * as React from "react";

export type UserRole = "PlatformAdmin" | "NgoAdmin";

export interface AuthContext {
  isAuthenticated: boolean;
  login: (email: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
  userRole: UserRole | null;
  email: string | null;
  isLoading: boolean;
}

const AuthContext = React.createContext<AuthContext>({
  isAuthenticated: false,
  isLoading: true,
  email: "",
  login: (email: string, password: string) => Promise.resolve(),
  logout: () => Promise.resolve(),
  userRole: "NgoAdmin",
});

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [isAuthenticated, setIsAuthenticated] = React.useState<boolean>(false);
  const [userRole, setUserRole] = React.useState<UserRole | null>(null);
  const [email, setEmail] = React.useState<string | null>(null);
  const [isLoading, setIsLoading] = React.useState(true);

  React.useEffect(() => {
    try {
      init();
    } finally {
      setIsLoading(false);
    }
  }, []);

  const init = () => {
    try {
      const token = localStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN);
      if (token) {
        const decodedToken = decodeToken(token);
        setUserRole(decodedToken.role);
        setEmail(decodedToken.email);
      }
      setIsAuthenticated(!!token);
    } catch (err) {
      localStorage.removeItem(STORAGE_KEYS.ACCESS_TOKEN);
      localStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN);
      localStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN_EXPIRY_TIME);
    }
  };

  const logout = React.useCallback(async () => {
    setIsAuthenticated(false);
    localStorage.removeItem(STORAGE_KEYS.ACCESS_TOKEN);
    localStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN);
    localStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN_EXPIRY_TIME);
  }, []);

  const login = async (email: string, password: string) => {
    setIsLoading(true);
    try {
      const {
        data: { token, refreshToken, refreshTokenExpiryTime },
      } = await API.post("auth/login", {
        email,
        password,
      });

      setAuthTokens(token, refreshToken, refreshTokenExpiryTime);
      const decodedToken = decodeToken(token);
      setUserRole(decodedToken.role);
      setEmail(decodedToken.email);

      setIsAuthenticated(true);
    } catch (err: unknown) {
      console.log("Error while trying to sign in", err);
      setIsAuthenticated(false);
      throw new Error("Error while trying to sign in");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <AuthContext.Provider
      value={{ isAuthenticated, userRole, email, login, logout, isLoading }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = React.useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
}
