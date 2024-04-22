import { useEffect, useState } from "react";
import { AuthContext } from "./auth-context";
import API from "../../services/api";
import * as SecureStore from "expo-secure-store";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { QueryClient } from "@tanstack/react-query";
import * as DB from "../../database/DAO/PollingStationsNomenclatorDAO";

const AuthContextProvider = ({ children }: React.PropsWithChildren) => {
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
  const [authError, setAuthError] = useState<boolean>(false);

  useEffect(() => {
    const token = SecureStore.getItem("access_token");
    setIsAuthenticated(!!token);
    setIsLoading(false);
    setAuthError(false);
  }, []);

  const signIn = async (email: string, password: string) => {
    try {
      setIsLoading(true);
      // const { token } = await dummyLogin();
      const {
        data: { token },
      } = await API.post("auth/login", {
        email,
        password,
      });
      SecureStore.setItem("access_token", token);
      setIsAuthenticated(true);
      setAuthError(false);
    } catch (err: unknown) {
      console.log("Error while trying to sign in", err);
      setAuthError(true);
    } finally {
      setIsLoading(false);
    }
  };

  const signOut = async (queryClient: QueryClient) => {
    setIsAuthenticated(false);

    queryClient.clear();

    await SecureStore.deleteItemAsync("access_token");
    await AsyncStorage.clear();
    await DB.deleteEverything();
  };

  return (
    <AuthContext.Provider
      value={{
        signIn,
        signOut,
        isAuthenticated,
        isLoading,
        authError,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export default AuthContextProvider;
