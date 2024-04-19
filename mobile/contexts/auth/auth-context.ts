import { QueryClient } from "@tanstack/react-query";
import React from "react";

type AuthContextType = {
  signIn: (email: string, password: string) => void;
  signOut: (queryClient: QueryClient) => void;
  isLoading: boolean;
  isAuthenticated: boolean;
};

export const AuthContext = React.createContext<AuthContextType>({
  signIn: (_email: string, _password: string) => null,
  signOut: (_queryClient: QueryClient) => null,
  isAuthenticated: false,
  isLoading: false,
});
