import { useIsRestoring } from "@tanstack/react-query";
import { SplashScreen } from "expo-router";
import { useEffect } from "react";

export function PersistGate({ children }: React.PropsWithChildren) {
  const isRestoring = useIsRestoring();
  console.log("🔂 IS RESTORING ", isRestoring);

  useEffect(() => {
    if (!isRestoring) {
      SplashScreen.hideAsync();
    }
  }, [isRestoring]);

  return isRestoring ? null : children;
}
