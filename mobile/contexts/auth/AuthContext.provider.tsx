import { useEffect, useState } from "react";
import { AuthContext } from "./auth-context";
import API from "../../services/api";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { QueryClient } from "@tanstack/react-query";
import * as DB from "../../database/DAO/PollingStationsNomenclatorDAO";
import * as Sentry from "@sentry/react-native";
import { ASYNC_STORAGE_KEYS } from "../../common/constants";
import { clearAsyncStorage } from "../../common/utils/utils";

const AuthContextProvider = ({ children }: React.PropsWithChildren) => {
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);

  useEffect(() => {
    try {
      const token = AsyncStorage.getItem(ASYNC_STORAGE_KEYS.ACCESS_TOKEN);
      setIsAuthenticated(!!token);
      setIsLoading(false);
    } catch (err) {
      Sentry.captureException(err);
      AsyncStorage.removeItem(ASYNC_STORAGE_KEYS.ACCESS_TOKEN);
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
        AsyncStorage.setItem(ASYNC_STORAGE_KEYS.ACCESS_TOKEN, token);
      } catch (err) {
        console.error("Could not set Aceess Token in AsyncStorage");
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
    queryClient.clear();
    setIsAuthenticated(false);
    try {
      await clearAsyncStorage();
      await DB.deleteEverything();
    } catch (err) {
      Sentry.captureMessage(`Logout error`);
      Sentry.captureException(err);
    }
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
