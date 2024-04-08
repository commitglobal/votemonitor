import React, { useMemo } from "react";
import { Adapt, Select as TamaguiSelect, Sheet, SelectProps } from "tamagui";
import { Icon } from "./Icon";
import { useSafeAreaInsets } from "react-native-safe-area-context";

interface StyledSelectProps extends SelectProps {
  placeholder?: string;
  options: { id: string | number; value: string }[];
}

const Select = ({ placeholder = "Select", options, ...props }: StyledSelectProps) => {
  const insets = useSafeAreaInsets();
  return (
    <TamaguiSelect disablePreventBodyScroll native {...props}>
      <TamaguiSelect.Trigger
        backgroundColor="transparent"
        paddingHorizontal="$md"
        borderColor="$gray1"
        borderWidth={1}
        iconAfter={<Icon icon="chevronRight" size={20} transform="rotate(90deg)" color="$gray7" />}
      >
        <TamaguiSelect.Value
          width={"90%"}
          color="$gray5"
          placeholder={placeholder}
          fontWeight="500"
        ></TamaguiSelect.Value>
      </TamaguiSelect.Trigger>

      <Adapt platform="touch">
        <Sheet native modal snapPoints={[80, 50]}>
          <Sheet.Frame>
            <Sheet.ScrollView padding="$sm" marginBottom={20 + insets.bottom}>
              <Adapt.Contents />
            </Sheet.ScrollView>
          </Sheet.Frame>
          <Sheet.Overlay />
        </Sheet>
      </Adapt>

      <TamaguiSelect.Content>
        <TamaguiSelect.Viewport>
          <TamaguiSelect.Group>
            {useMemo(
              () =>
                options.map((entry, i) => {
                  return (
                    <TamaguiSelect.Item index={i} key={entry.id} value={entry.value} gap="$3">
                      <TamaguiSelect.ItemText width={"90%"} numberOfLines={1}>
                        {entry.value}
                      </TamaguiSelect.ItemText>
                      <TamaguiSelect.ItemIndicator>
                        <Icon icon="chevronLeft" />
                      </TamaguiSelect.ItemIndicator>
                    </TamaguiSelect.Item>
                  );
                }),
              [options],
            )}
          </TamaguiSelect.Group>
        </TamaguiSelect.Viewport>
      </TamaguiSelect.Content>
    </TamaguiSelect>
  );
};

export default Select;
