import { Slot } from "expo-router";
import AuthContextProvider from "../contexts/auth/AuthContext.provider";

export default function Root() {
  return (
    <AuthContextProvider>
      <Slot />
    </AuthContextProvider>
  );
}
