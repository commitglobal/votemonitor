import React from "react";

type AuthContextType = {
  signIn: () => void;
  signOut: () => void;
  isAuthenticated: boolean;
};

export const AuthContext = React.createContext<AuthContextType>({
  signIn: () => null,
  signOut: () => null,
  isAuthenticated: false,
});
