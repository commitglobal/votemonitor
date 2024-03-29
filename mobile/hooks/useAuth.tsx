import { useContext } from "react";
import { AuthContext } from "../contexts/auth/auth-context";

export function useAuth() {
  return useContext(AuthContext);
}
