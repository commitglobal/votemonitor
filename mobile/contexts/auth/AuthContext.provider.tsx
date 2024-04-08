import { useEffect, useState } from "react";
import { AuthContext } from "./auth-context";
// import API from "../../services/api";
import * as SecureStore from "expo-secure-store";

const AuthContextProvider = ({ children }: React.PropsWithChildren) => {
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);

  useEffect(() => {
    const token = SecureStore.getItem("access_token");
    setIsAuthenticated(!!token);
    setIsLoading(false);
  }, []);

  const signIn = async () => {
    try {
      setIsLoading(true);
      const { token } = await dummyLogin();
      // API.post("auth", {
      //   username: "alice@example.com",
      //   password: "string",
      // });
      SecureStore.setItem("access_token", token);
      setIsAuthenticated(true);
    } catch (err: unknown) {
      console.log("Error while trying to sign in", err);
    } finally {
      setIsLoading(false);
    }
  };

  const dummyLogin = async (): Promise<{ token: string }> => {
    return new Promise((resolve) => {
      setTimeout(() => {
        resolve({ token: "token_1231231231" });
      }, 3000);
    });
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
