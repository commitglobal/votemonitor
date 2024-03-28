import React, { useMemo, useState } from "react";
import { Select, Adapt, Sheet, YStack } from "tamagui";
import { Check, ChevronDown, ChevronDownCircle } from "@tamagui/lucide-icons";

const PollingStationAddressSelect = () => {
  const [val, setVal] = useState("");

  return (
    <YStack padding="$2.5" backgroundColor="white">
      <Select value={val} onValueChange={setVal} disablePreventBodyScroll>
        <Select.Trigger
          backgroundColor="#F0E5FF"
          borderRadius="$10"
          iconAfter={<ChevronDown color="#7833B3" />}
        >
          <Select.Value
            placeholder="Polling Station"
            color="#7833B3"
            // TODO: add font weight here
            width={"90%"}
          />
        </Select.Trigger>

        <Adapt platform="touch">
          <Sheet native modal snapPoints={[80, 50]}>
            <Sheet.Frame>
              <Sheet.ScrollView>
                <Adapt.Contents />
              </Sheet.ScrollView>
            </Sheet.Frame>
            <Sheet.Overlay />
          </Sheet>
        </Adapt>

        <Select.Content>
          <Select.Viewport>
            <Select.Group>
              <Select.Label>Choose polling station</Select.Label>
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
                          <Check size={16} />
                        </Select.ItemIndicator>
                      </Select.Item>
                    );
                  }),
                [pollingStationAdresses]
              )}
            </Select.Group>
          </Select.Viewport>
          <Select.ScrollDownButton />
        </Select.Content>
      </Select>
    </YStack>
  );
};

export default PollingStationAddressSelect;

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
