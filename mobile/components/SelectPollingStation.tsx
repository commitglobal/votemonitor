import React, { useMemo, useState } from "react";
import { Adapt, Button, Select, Sheet, View, YStack, SelectProps } from "tamagui";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import { useSafeAreaInsets } from "react-native-safe-area-context";

interface SelectPollingStationProps extends SelectProps {
  placeholder?: string;
  options: { id: string | number; value: string }[];
}

const SelectPollingStation: React.FC<SelectPollingStationProps> = ({
  options,
  placeholder = "Select",
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
                <View marginTop="$xxs">
                  {/* //TODO: not sure how many nroflines we should leave here */}
                  <Typography numberOfLines={7} color="$gray5">
                    {/* //TODO: translation here */}
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
                  options.map((entry, i) => {
                    return (
                      <Select.Item index={i} key={entry.id} value={entry.value} gap="$3">
                        {/* //TODO: change number of lines to 2 if that's what we want */}
                        <Select.ItemText width={"90%"} numberOfLines={1}>
                          {entry.value}
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
