import { createContext, useState, useContext, useEffect } from "react";
import { SECURE_STORAGE_KEYS } from "../../common/constants";
import * as SecureStore from "expo-secure-store";

export enum AppMode {
  CITIZEN = "citizen",
  OBSERVER = "observer",
}

type AppModeContextType = {
  appMode: AppMode | null;
  setAppMode: (appMode: AppMode) => void;
};

export const AppModeContext = createContext<AppModeContextType | null>(null);

const AppModeContextProvider = ({ children }: React.PropsWithChildren) => {
  const [appMode, setAppMode] = useState<AppMode | null>(null);
  console.log("AppModeContextProvider", appMode);

  // SecureStore.deleteItemAsync(SECURE_STORAGE_KEYS.ONBOARDING_NEW_COMPLETE);

  useEffect(() => {
    const appMode = SecureStore.getItem(SECURE_STORAGE_KEYS.ONBOARDING_NEW_COMPLETE);
    if (appMode) {
      setAppMode(appMode as AppMode);
    }
  }, []);

  const handleSetAppMode = (appMode: AppMode) => {
    console.log("handleSetAppMode", appMode);
    setAppMode(appMode);
    SecureStore.setItem(SECURE_STORAGE_KEYS.ONBOARDING_NEW_COMPLETE, appMode);
  };

  return (
    <AppModeContext.Provider value={{ appMode, setAppMode: handleSetAppMode }}>
      {children}
    </AppModeContext.Provider>
  );
};

export const useAppMode = () => {
  const data = useContext(AppModeContext);

  if (!data) {
    throw new Error("No data in AppModeContext found. Was used outside the tree");
  }

  return data;
};

export default AppModeContextProvider;
