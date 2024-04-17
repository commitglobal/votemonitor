import { useEffect, useState } from "react";
import { AuthContext } from "./auth-context";
import API from "../../services/api";
import * as SecureStore from "expo-secure-store";

const AuthContextProvider = ({ children }: React.PropsWithChildren) => {
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);

  useEffect(() => {
    const token = SecureStore.getItem("access_token");
    setIsAuthenticated(!!token);
    setIsLoading(false);
  }, []);

  const signIn = async (email: string, password: string) => {
    console.log("Signing in...");
    console.log("Email: ", email);
    console.log("Password: ", password);

    try {
      setIsLoading(true);
      // const { token } = await dummyLogin();
      const {
        data: { token },
      } = await API.post("auth/login", {
        email: "alice@example.com",
        password: "string",
      });
      SecureStore.setItem("access_token", token);
      setIsAuthenticated(true);
    } catch (err: unknown) {
      console.log("Error while trying to sign in", err);
    } finally {
      setIsLoading(false);
    }
  };

  const signOut = async () => {
    setIsAuthenticated(false);
    // remove token
    await SecureStore.deleteItemAsync("access_token");
  };

  return (
    <AuthContext.Provider
      value={{
        signIn,
        signOut,
        isAuthenticated,
        isLoading,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export default AuthContextProvider;
