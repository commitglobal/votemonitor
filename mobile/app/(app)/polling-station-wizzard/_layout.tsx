import { Slot } from "expo-router";
import Header from "../../../components/Header";
import { useTheme } from "tamagui";
import { Icon } from "../../../components/Icon";
import { router } from "expo-router";

import { useContext, useState } from "react";
import React from "react";

// TODO: Move the context
type OpenContextType = {
  open: boolean;
  setOpen: (value: boolean) => void;
};
export const OpenContext = React.createContext<OpenContextType>({
  open: false,
  setOpen: () => null,
});

export const OpenContextProvider = ({ children }: React.PropsWithChildren) => {
  const [open, setOpen] = useState(false);

  return (
    <OpenContext.Provider
      value={{
        open,
        setOpen,
      }}
    >
      {children}
    </OpenContext.Provider>
  );
};

const PollingStationWizzardLayout = () => {
  return (
    <>
      <OpenContextProvider>
        <HeaderWithContext />
        <Slot />
      </OpenContextProvider>
    </>
  );
};

const HeaderWithContext = () => {
  const theme = useTheme();
  const { open, setOpen } = useContext(OpenContext);

  console.log(open);

  return (
    <Header
      title="Header"
      titleColor="white"
      backgroundColor={theme.purple5?.val}
      barStyle="light-content"
      leftIcon={<Icon icon="chevronLeft" color="white" />}
      rightIcon={<Icon icon="dotsVertical" color="white" />}
      onLeftPress={() => router.back()}
      onRightPress={() => setOpen(true)}
    />
  );
};

export default PollingStationWizzardLayout;
