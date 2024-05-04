import { useUpdates, checkForUpdateAsync, fetchUpdateAsync, reloadAsync } from "expo-updates";
import React, { createContext, useContext, useEffect } from "react";

import { useAppState } from "../../hooks/useAppState";
import { useInterval } from "../../hooks/useInterval";
import { isAvailableUpdateCritical } from "../../helpers/updateUtils";

// Wrap async expo-updates functions (errors are surfaced in useUpdates() hook so can be ignored)
const checkForUpdate: () => Promise<string | undefined> = async () => {
  try {
    const result = await checkForUpdateAsync();
    return result.reason;
  } catch (_error) {}
};

const downloadUpdate = () => fetchUpdateAsync().catch((_error) => {});
const runUpdate = () => reloadAsync().catch((_error) => {});

const defaultUpdateCheckInterval = 30 * 60 * 1000; // 30 minutes
const defaultCheckOnForeground = true;
const defaultAutoLaunchCritical = true;

interface EasUpdateMonitorContextProps {
  isUpdateReadyToInstall: boolean;
  isUpdateCritical: boolean;
}

const EasUpdateMonitorContext = createContext<EasUpdateMonitorContextProps>({
  isUpdateReadyToInstall: false,
  isUpdateCritical: false,
});

export function useEasUpdateMonitor() {
  return useContext(EasUpdateMonitorContext);
}

export const EasUpdateMonitorContextProvider = ({ children }: { children: React.ReactNode }) => {
  useEffect(() => {
    // console.log("useUpdateMonitor initialized");
    checkForUpdate();
  }, []);

  const updatesSystem = useUpdates();

  const { isUpdateAvailable, isUpdatePending } = updatesSystem;

  const isUpdateCritical = isAvailableUpdateCritical(updatesSystem);

  // Check if needed when app becomes active
  const appStateHandler = (activating: boolean) => {
    if (activating) {
      // console.log("Check for update activating");
      checkForUpdate();
    }
  };
  const appState = useAppState(defaultCheckOnForeground ? appStateHandler : undefined);

  // This effect runs periodically to see if an update check is needed
  // The effect interval should be smaller than monitorInterval
  useInterval(() => {
    if (appState === "active") {
      // console.log("Check for update");
      checkForUpdate();
    }
  }, defaultUpdateCheckInterval);

  // If update is critical, download it
  useEffect(() => {
    if (isUpdateCritical && !isUpdatePending && defaultAutoLaunchCritical) {
      downloadUpdate();
    }
  }, [isUpdateCritical, isUpdatePending]);

  // Run the downloaded update (after delay) if download completes successfully and it is critical
  useEffect(() => {
    if (isUpdatePending && isUpdateCritical && defaultAutoLaunchCritical) {
      setTimeout(() => runUpdate(), 2000);
    }
  }, [isUpdateCritical, isUpdatePending]);

  // Starts the download when there is an available update
  useEffect(() => {
    if (isUpdateAvailable && !isUpdatePending) {
      downloadUpdate();
    }
  }, [isUpdateAvailable, isUpdatePending]);

  return (
    <EasUpdateMonitorContext.Provider
      value={{
        isUpdateReadyToInstall: isUpdatePending,
        isUpdateCritical,
      }}
    >
      {children}
    </EasUpdateMonitorContext.Provider>
  );
};
