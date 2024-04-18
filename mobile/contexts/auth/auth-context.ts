import React from "react";

type AuthContextType = {
  signIn: (email: string, password: string) => void;
  signOut: () => void;
  isLoading: boolean;
  isAuthenticated: boolean;
};

export const AuthContext = React.createContext<AuthContextType>({
  signIn: (email: string, password: string) => null,
  signOut: () => null,
  isAuthenticated: false,
  isLoading: false,
});
