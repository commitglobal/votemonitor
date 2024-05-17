import { useEffect, useState } from "react";
import { AuthContext } from "./auth-context";
import API from "../../services/api";
import * as SecureStore from "expo-secure-store";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { QueryClient } from "@tanstack/react-query";
import * as DB from "../../database/DAO/PollingStationsNomenclatorDAO";
import * as Sentry from "@sentry/react-native";

const AuthContextProvider = ({ children }: React.PropsWithChildren) => {
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);

  useEffect(() => {
    try {
      const token = SecureStore.getItem("access_token");
      setIsAuthenticated(!!token);
      setIsLoading(false);
    } catch (err) {
      Sentry.captureException(err);
      SecureStore.deleteItemAsync("access_token");
    }
  }, []);

  const signIn = async (email: string, password: string) => {
    try {
      setIsLoading(true);
      const {
        data: { token },
      } = await API.post("auth/login", {
        email,
        password,
      });
      try {
        SecureStore.setItem("access_token", token);
      } catch (err) {
        console.error("Could not set Aceess Token in secure storage");
        throw err;
      }
      setIsAuthenticated(true);
    } catch (err: unknown) {
      Sentry.captureException(err);
      console.log("Error while trying to sign in", err);
      throw new Error("Error while trying to sign in");
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
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export default AuthContextProvider;
