import { useIsRestoring } from "@tanstack/react-query";

export function PersistGate({ children }: React.PropsWithChildren) {
  const isRestoring = useIsRestoring();
  console.log("🔂 IS RESTORING ", isRestoring);

  //   return isRestoring ? <LoadingScreen /> : children;
  return isRestoring ? null : children;
}
