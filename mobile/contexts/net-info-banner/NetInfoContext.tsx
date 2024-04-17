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
  const [isOnline, setIsOnline] = useState(false);
  const [showNetInfoBanner, setShowNetInfoBanner] = useState(true);

  const isFirstRender = useRef(true);

  useEffect(() => {
    const unsubscribe = NetInfo.addEventListener((state) => {
      const status = !!state.isConnected;
      setIsOnline(!status);
    });
    return unsubscribe();
  }, []);

  useEffect(() => {
    if (isOnline && isFirstRender.current !== true) {
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
