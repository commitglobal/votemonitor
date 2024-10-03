import { createContext, useState, useContext } from "react";

type AppModeContextType = {
  appMode?: "citizen" | "observer";
  setAppMode: (appMode: "citizen" | "observer") => void;

  onboardingComplete: boolean;
  setOnboardingComplete: (onboardingComplete: boolean) => void;
};

export const AppModeContext = createContext<AppModeContextType | null>(null);

const AppModeContextProvider = ({ children }: React.PropsWithChildren) => {
  const [appMode, setAppMode] = useState<"citizen" | "observer">();
  const [onboardingComplete, setOnboardingComplete] = useState<boolean>(false);

  return (
    <AppModeContext.Provider
      value={{ appMode, setAppMode, onboardingComplete, setOnboardingComplete }}
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
