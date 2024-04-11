import React, { useMemo, useState } from "react";
import { Adapt, Select, SelectProps, Sheet, View, YStack } from "tamagui";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { PollingStationVisitVM } from "../services/definitions.api";
import Button from "../components/Button";

interface SelectPollingStationProps extends SelectProps {
  placeholder?: string;
  options: PollingStationVisitVM[];
}

const SelectPollingStation: React.FC<SelectPollingStationProps> = ({
  options,
  placeholder = "Select polling station",
}) => {
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
            placeholder={placeholder}
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
                <Typography preset="body2" color="$gray5">
                  My polling stations
                </Typography>
                {/* //TODO: not sure how many nroflines we should leave here */}
                <Typography numberOfLines={7} color="$gray5" marginTop="$xxs">
                  {/* //TODO: translation here */}
                  You can switch between polling stations if you want to revisit form answers or
                  polling station information.
                </Typography>
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
                <Button preset="outlined">Add new polling station</Button>
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
                  options.map((entry, i) => {
                    return (
                      <Select.Item
                        index={i}
                        key={entry.pollingStationId}
                        value={entry.pollingStationId}
                        gap="$3"
                      >
                        {/* //TODO: change number of lines to 2 if that's what we want */}
                        <Select.ItemText width={"90%"} numberOfLines={1}>
                          {entry.pollingStationId}
                        </Select.ItemText>
                        <Select.ItemIndicator>
                          <Icon icon="chevronLeft" />
                        </Select.ItemIndicator>
                      </Select.Item>
                    );
                  }),
                [options],
              )}
            </Select.Group>
          </Select.Viewport>
        </Select.Content>
      </Select>
    </YStack>
  );
};

export default SelectPollingStation;
