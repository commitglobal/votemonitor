import React from "react";

type AuthContextType = {
  signIn: () => void;
  signOut: () => void;
  isLoading: boolean;
  isAuthenticated: boolean;
};

export const AuthContext = React.createContext<AuthContextType>({
  signIn: () => null,
  signOut: () => null,
  isAuthenticated: false,
  isLoading: false,
});
