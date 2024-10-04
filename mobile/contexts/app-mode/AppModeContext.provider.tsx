import { createContext, useState, useContext, useEffect } from "react";
import { Typography } from "../../components/Typography";
import { SECURE_STORAGE_KEYS } from "../../common/constants";
import * as SecureStore from "expo-secure-store";

type AppModeContextType = {
  appMode: "citizen" | "observer" | "onboarding";
  setAppMode: (appMode: "citizen" | "observer" | "onboarding") => void;

  onboardingComplete: boolean;
  setOnboardingComplete: (onboardingComplete: boolean) => void;
};

export const AppModeContext = createContext<AppModeContextType | null>(null);

const AppModeContextProvider = ({ children }: React.PropsWithChildren) => {
  const [appMode, setAppMode] = useState<"citizen" | "observer" | "onboarding">();
  const [onboardingComplete, setOnboardingComplete] = useState<boolean>(false);
  console.log("AppModeContextProvider", appMode);

  useEffect(() => {
    const storedAppMode = SecureStore.getItem(SECURE_STORAGE_KEYS.ONBOARDING_NEW_COMPLETE);
    // if no appMode has been set it should be onboarding to avoid flickering
    const appMode = storedAppMode ? (storedAppMode as "citizen" | "observer") : "onboarding";
    setAppMode(appMode);
  }, []);

  const handleSetAppMode = (appMode: "citizen" | "observer" | "onboarding") => {
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
