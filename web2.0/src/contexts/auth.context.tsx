import { sleep } from "@/lib/utils";
import * as React from "react";

export type UserRole = "PlatformAdmin" | "NgoAdmin";

export interface AuthContext {
  isAuthenticated: boolean;
  login: (userRole: UserRole) => Promise<void>;
  logout: () => Promise<void>;
  userRole: UserRole | null;
}

const AuthContext = React.createContext<AuthContext | null>(null);

const key = "tanstack.auth.user";

function getStoredUserRole() {
  return localStorage.getItem(key) as UserRole;
}

function setStoredUser(userRole: string | null) {
  if (userRole) {
    localStorage.setItem(key, userRole);
  } else {
    localStorage.removeItem(key);
  }
}

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [userRole, setUserRole] = React.useState<UserRole | null>(
    getStoredUserRole()
  );
  const isAuthenticated = !!userRole;

  const logout = React.useCallback(async () => {
    await sleep(250);

    setStoredUser(null);
    setUserRole(null);
  }, []);

  const login = React.useCallback(async (userRole: UserRole) => {
    await sleep(500);

    setStoredUser(userRole);
    setUserRole(userRole);
  }, []);

  React.useEffect(() => {
    setUserRole(getStoredUserRole());
  }, []);

  return (
    <AuthContext.Provider value={{ isAuthenticated, userRole, login, logout }}>
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
