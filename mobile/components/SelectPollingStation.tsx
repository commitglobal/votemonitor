import React, { useMemo, useState } from "react";
import { Adapt, Button, Select, Sheet, View, YStack } from "tamagui";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import { useSafeAreaInsets } from "react-native-safe-area-context";

const SelectPollingStation = () => {
  const [val, setVal] = useState("");
  const insets = useSafeAreaInsets();

  return (
    <YStack paddingVertical="$xs" paddingHorizontal="$md" backgroundColor="white">
      <Select value={val} onValueChange={setVal} disablePreventBodyScroll>
        <Select.Trigger
          justifyContent="center"
          alignItems="center"
          backgroundColor="$purple1"
          borderRadius="$10"
          iconAfter={
            <Icon icon="chevronRight" size={24} transform="rotate(90deg)" color="$purple5" />
          }
        >
          <Select.Value
            width={"90%"}
            color="$purple5"
            placeholder="Select polling station"
            fontWeight="500"
          ></Select.Value>
        </Select.Trigger>

        <Adapt platform="touch">
          <Sheet native modal snapPoints={[80, 50]}>
            <Sheet.Frame>
              <YStack
                paddingVertical="$xl"
                paddingLeft="$lg"
                paddingRight="$xxxl"
                borderBottomWidth={1}
                borderBottomColor="$gray3"
              >
                <Typography preset="body1" color="$gray5">
                  My polling stations
                </Typography>
                <View marginTop="$xxs">
                  <Typography numberOfLines={3} color="$gray5">
                    You can switch between polling stations if you want to revisit form answers or
                    polling station information.
                  </Typography>
                </View>
              </YStack>

              <Sheet.ScrollView padding="$sm">
                <Adapt.Contents />
              </Sheet.ScrollView>
              <View
                paddingVertical="$xl"
                paddingHorizontal={40}
                borderTopWidth={1}
                borderTopColor="$gray3"
                marginBottom={insets.bottom}
              >
                {/* //TODO: change button here with our custom one */}
                <Button>Add new polling station</Button>
              </View>
            </Sheet.Frame>
            <Sheet.Overlay />
          </Sheet>
        </Adapt>

        <Select.Content>
          <Select.Viewport>
            <Select.Group>
              {/* //TODO: texts from translation */}

              {useMemo(
                () =>
                  pollingStationAdresses.map((pollingStationAddress, i) => {
                    return (
                      <Select.Item
                        index={i}
                        key={pollingStationAddress.id}
                        value={pollingStationAddress.address}
                        gap="$3"
                      >
                        {/* //TODO: change number of lines to 2 if that's what we want */}
                        <Select.ItemText width={"90%"} numberOfLines={1}>
                          {pollingStationAddress.address}
                        </Select.ItemText>
                        <Select.ItemIndicator>
                          <Icon icon="chevronLeft" />
                        </Select.ItemIndicator>
                      </Select.Item>
                    );
                  }),
                [pollingStationAdresses],
              )}
            </Select.Group>
          </Select.Viewport>
        </Select.Content>
      </Select>
    </YStack>
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
  { id: 10, address: "Secția 222, Str. Olteniei, nr. 18, Craiova, Romania" },
  { id: 11, address: "Secția 333, Str. Banatului, nr. 22, Arad, Romania" },
  { id: 12, address: "Secția 444, Str. Mureșului, nr. 11, Deva, Romania" },
  { id: 13, address: "Secția 555, Str. Dobrogei, nr. 7, Tulcea, Romania" },
  { id: 14, address: "Secția 667, Str. Moldovei, nr. 9, Bacău, Romania" },
  { id: 15, address: "Secția 777, Str. Crișului, nr. 13, Satu Mare, Romania" },
  { id: 16, address: "Secția 888, Str. Olteniei, nr. 4, Pitești, Romania" },
  { id: 17, address: "Secția 999, Str. Bucovinei, nr. 16, Suceava, Romania" },
  {
    id: 18,
    address: "Secția 1010, Str. Transilvaniei, nr. 32, Alba Iulia, Romania",
  },
  { id: 19, address: "Secția 1111, Str. Banatului, nr. 3, Reșița, Romania" },
  { id: 20, address: "Secția 1212, Str. București, nr. 7, Galați, Romania" },
];
