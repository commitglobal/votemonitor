import { createContext, useState, useContext, useEffect } from "react";
import { Typography } from "../../components/Typography";
import { SECURE_STORAGE_KEYS } from "../../common/constants";
import * as SecureStore from "expo-secure-store";

export enum AppMode {
  CITIZEN = "citizen",
  OBSERVER = "observer",
  ONBOARDING = "onboarding",
}

type AppModeContextType = {
  appMode: AppMode;
  setAppMode: (appMode: AppMode) => void;

  onboardingComplete: boolean;
  setOnboardingComplete: (onboardingComplete: boolean) => void;
};

export const AppModeContext = createContext<AppModeContextType | null>(null);

const AppModeContextProvider = ({ children }: React.PropsWithChildren) => {
  const [appMode, setAppMode] = useState<AppMode>();
  const [onboardingComplete, setOnboardingComplete] = useState<boolean>(false);
  console.log("AppModeContextProvider", appMode);

  useEffect(() => {
    const storedAppMode = SecureStore.getItem(SECURE_STORAGE_KEYS.ONBOARDING_NEW_COMPLETE);
    // if no appMode has been set it should be onboarding to avoid flickering
    const appMode = storedAppMode ? (storedAppMode as "citizen" | "observer") : "onboarding";
    setAppMode(appMode as AppMode);
  }, []);

  const handleSetAppMode = (appMode: AppMode) => {
    console.log("handleSetAppMode", appMode);
    setAppMode(appMode);
    SecureStore.setItem(SECURE_STORAGE_KEYS.ONBOARDING_NEW_COMPLETE, appMode);
  };

  if (!appMode) {
    return <Typography>Loading...</Typography>;
  }

  return (
    <AppModeContext.Provider
      value={{ appMode, setAppMode: handleSetAppMode, onboardingComplete, setOnboardingComplete }}
    >
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
