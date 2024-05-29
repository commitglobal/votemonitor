import { useEffect, useState } from "react";
import { AuthContext } from "./auth-context";
import API from "../../services/api";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { QueryClient } from "@tanstack/react-query";
import * as DB from "../../database/DAO/PollingStationsNomenclatorDAO";
import * as Sentry from "@sentry/react-native";
import { ASYNC_STORAGE_KEYS } from "../../common/constants";
import { clearAsyncStorage } from "../../common/utils/utils";
import { Typography } from "../../components/Typography";

const AuthContextProvider = ({ children }: React.PropsWithChildren) => {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean | null>(null);

  useEffect(() => {
    init();
  }, []);

  const init = async () => {
    try {
      const token = await AsyncStorage.getItem(ASYNC_STORAGE_KEYS.ACCESS_TOKEN);
      setIsAuthenticated(!!token);
    } catch (err) {
      Sentry.captureException(err);
      await AsyncStorage.removeItem(ASYNC_STORAGE_KEYS.ACCESS_TOKEN);
    }
  };

  const signIn = async (email: string, password: string) => {
    try {
      const {
        data: { token },
      } = await API.post("auth/login", {
        email,
        password,
      });
      try {
        await AsyncStorage.setItem(ASYNC_STORAGE_KEYS.ACCESS_TOKEN, token);
      } catch (err) {
        console.error("Could not set Aceess Token in AsyncStorage");
        throw err;
      }
      setIsAuthenticated(true);
    } catch (err: unknown) {
      Sentry.captureException(err);
      console.log("Error while trying to sign in", err);
      setIsAuthenticated(false);
      throw new Error("Error while trying to sign in");
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

  if (isAuthenticated === null) {
    // Actually the SplashScreen will be displayed but we don't pass a wrong value down the chain
    return <Typography>Loading...</Typography>;
  }

  return (
    <AuthContext.Provider
      value={{
        signIn,
        signOut,
        isAuthenticated,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export default AuthContextProvider;
