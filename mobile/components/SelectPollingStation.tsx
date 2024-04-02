import React, { useMemo, useState } from "react";
import { Platform } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import {
  Adapt,
  Button,
  PortalProvider,
  Select,
  Sheet,
  useTheme,
  XStack,
} from "tamagui";
import { Typography } from "./Typography";
import { Icon } from "./Icon";

const SelectPollingStation = () => {
  const [open, setOpen] = useState(false);
  const insets = useSafeAreaInsets();
  const theme = useTheme();
  return (
    <>
      <XStack
        backgroundColor="white"
        paddingVertical="$xs"
        paddingHorizontal="$md"
      >
        <XStack
          backgroundColor="$purple1"
          flex={1}
          borderRadius={50}
          paddingVertical={7}
          paddingHorizontal="$xs"
          alignItems="center"
          justifyContent="space-between"
          onPress={() => setOpen(true)}
        >
          <Typography
            preset="body2"
            style={{ color: theme.purple5?.val, flex: 1 }}
          >
            Secția 123, Str. Moldovei, nr. 30, Targu Muree3eeeeeeeeeez
          </Typography>
          <Icon icon="chevronRight" />
        </XStack>
      </XStack>

      <Sheet modal={true} open={open} onOpenChange={setOpen} zIndex={100_000}>
        <Sheet.Overlay />
        <Sheet.Frame justifyContent="center" alignItems="center">
          <XStack
            padding="$md"
            gap="$sm"
            marginBottom={Platform.OS === "ios" && insets.bottom}
          >
            <Button
              backgroundColor="transparent"
              fontWeight="500"
              fontSize={16}
              onPress={() => setOpen(false)}
            >
              Cancel
            </Button>
            <Button backgroundColor="$red12" color="white" flex={1}>
              Clear form
            </Button>
          </XStack>
        </Sheet.Frame>
      </Sheet>
    </>
  );
};

export default SelectPollingStation;

const pollingStationAdresses = [
  { id: 1, address: "Secția 123, Str. Moldovei, nr. 30, Târgu Mureș, Romania" },
  {
    id: 2,
    address: "Secția 456, Str. Transilvaniei, nr. 45, Cluj-Napoca, Romania",
  },
  { id: 3, address: "Secția 789, Str. București, nr. 12, București, Romania" },
  { id: 4, address: "Secția 101, Str. Timișoarei, nr. 20, Timișoara, Romania" },
  { id: 5, address: "Secția 234, Str. Iași, nr. 15, Iași, Romania" },
  { id: 6, address: "Secția 345, Str. Crișana, nr. 10, Oradea, Romania" },
  {
    id: 7,
    address: "Secția 567, Str. Maramureșului, nr. 25, Baia Mare, Romania",
  },
  { id: 8, address: "Secția 890, Str. Dobrogei, nr. 8, Constanța, Romania" },
  { id: 9, address: "Secția 111, Str. Ardealului, nr. 5, Sibiu, Romania" },
  { id: 10, address: "Secția 222, Str. Banatului, nr. 18, Arad, Romania" },
];
