import React, { createContext, ReactNode, useContext, useState } from "react";

type NetInfoContextType = {
  isOnline: boolean;
  setIsOnline: React.Dispatch<React.SetStateAction<boolean>>;
  showNetInfoBanner: boolean;
  setShowNetInfoBanner: React.Dispatch<React.SetStateAction<boolean>>;
  isBannerShowing: boolean;
};

const NetInfoContext = createContext<NetInfoContextType>({
  isOnline: false,
  setIsOnline: () => null,
  showNetInfoBanner: true,
  setShowNetInfoBanner: () => null,
  isBannerShowing: true,
});

export const useNetInfoContext = () => useContext(NetInfoContext);

export const NetInfoProvider = ({ children }: { children: ReactNode }) => {
  const [isOnline, setIsOnline] = useState(false);
  const [showNetInfoBanner, setShowNetInfoBanner] = useState(true);

  const isBannerShowing = (isOnline && showNetInfoBanner) || !isOnline;

  return (
    <NetInfoContext.Provider
      value={{ isOnline, setIsOnline, showNetInfoBanner, setShowNetInfoBanner, isBannerShowing }}
    >
      {children}
    </NetInfoContext.Provider>
  );
};
