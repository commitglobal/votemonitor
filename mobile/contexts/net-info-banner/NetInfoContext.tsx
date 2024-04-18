import React, {
  createContext,
  ReactNode,
  useContext,
  useEffect,
  useMemo,
  useRef,
  useState,
} from "react";
import NetInfo from "@react-native-community/netinfo";
import { onlineManager } from "@tanstack/react-query";

type NetInfoContextType = {
  isOnline: boolean;
  shouldDisplayBanner: boolean;
};

const NetInfoContext = createContext<NetInfoContextType>({
  isOnline: false,
  shouldDisplayBanner: true,
});

export const useNetInfoContext = () => useContext(NetInfoContext);

export const NetInfoProvider = ({ children }: { children: ReactNode }) => {
  const [isOnline, setIsOnline] = useState(true);
  const [showNetInfoBanner, setShowNetInfoBanner] = useState(false);

  const isFirstRender = useRef(true);

  useEffect(() => {
    const unsubscribe = NetInfo.addEventListener((state) => {
      const status = !!state.isConnected;
      onlineManager.setOnline(status);
      setIsOnline(status);
    });
    return () => unsubscribe();
  }, []);

  useEffect(() => {
    if (!isFirstRender.current && isOnline) {
      setShowNetInfoBanner(true);
    }

    isFirstRender.current = false;
  }, [isOnline]);

  useEffect(() => {
    if (showNetInfoBanner) {
      const timer = setTimeout(() => {
        setShowNetInfoBanner(false);
      }, 3000);
      return () => clearTimeout(timer);
    }
  }, [showNetInfoBanner]);

  const shouldDisplayBanner = useMemo(
    () => (isOnline && showNetInfoBanner) || !isOnline,
    [isOnline, showNetInfoBanner],
  );

  return (
    <NetInfoContext.Provider value={{ isOnline, shouldDisplayBanner }}>
      {children}
    </NetInfoContext.Provider>
  );
};
