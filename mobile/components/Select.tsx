import React, { useMemo, useState } from "react";
import {
  Adapt,
  Select as TamaguiSelect,
  Sheet,
  SelectProps,
  styled,
  Input,
  XStack,
  YStack,
} from "tamagui";
import { Icon } from "./Icon";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { Typography } from "./Typography";

interface StyledSelectProps extends SelectProps {
  placeholder?: string;
  options: { id: string | number; value: string; label: string }[];
}

const Select = ({ placeholder = "Select", options, ...props }: StyledSelectProps) => {
  const insets = useSafeAreaInsets();
  const [searchTerm, setSearchTerm] = useState<string>("");

  // Filter options based on search term
  const filteredOptions = useMemo(() => {
    if (!searchTerm) return options;
    return options.filter((option) =>
      option.label.toLowerCase().includes(searchTerm.toLowerCase()),
    );
  }, [options, searchTerm]);

  return (
    <TamaguiSelect disablePreventBodyScroll native {...props}>
      <TamaguiSelect.Trigger
        backgroundColor="white"
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
        <Sheet native modal snapPoints={[50]} moveOnKeyboardChange={true}>
          <Sheet.Frame>
            <YStack
              borderBottomWidth={1}
              borderBottomColor="$gray3"
              padding="$md"
              backgroundColor="white"
            >
              <XStack backgroundColor="$purple1" borderRadius={8} alignItems="center">
                <Icon icon="search" color="transparent" size={20} marginLeft="$sm" />
                <SearchInput flex={1} value={searchTerm} onChangeText={setSearchTerm} />
              </XStack>
            </YStack>

            <Sheet.ScrollView
              marginBottom={insets.bottom}
              paddingHorizontal="$sm"
              keyboardShouldPersistTaps="handled"
            >
              <Adapt.Contents />
            </Sheet.ScrollView>
          </Sheet.Frame>
          <Sheet.Overlay />
        </Sheet>
      </Adapt>

      <TamaguiSelect.Content>
        <TamaguiSelect.Viewport>
          <TamaguiSelect.Group>
            {filteredOptions.length === 0 ? (
              <Typography padding="$md">No data found for current search.</Typography>
            ) : (
              filteredOptions.map((entry, i) => {
                return (
                  <TamaguiSelect.Item
                    index={i}
                    key={`${entry.id}_${i}`}
                    value={entry.value}
                    gap="$3"
                    paddingBottom="$sm"
                  >
                    <TamaguiSelect.ItemText width={"90%"} numberOfLines={1}>
                      {entry.label}
                    </TamaguiSelect.ItemText>
                    <TamaguiSelect.ItemIndicator>
                      <Icon icon="chevronLeft" />
                    </TamaguiSelect.ItemIndicator>
                  </TamaguiSelect.Item>
                );
              })
            )}
          </TamaguiSelect.Group>
        </TamaguiSelect.Viewport>
      </TamaguiSelect.Content>
    </TamaguiSelect>
  );
};

export default Select;

const SearchInput = styled(Input, {
  backgroundColor: "$purple1",
  placeholder: "Search",
  color: "$purple5",
  placeholderTextColor: "$purple5",
  focusStyle: {
    borderColor: "transparent",
  },
});
