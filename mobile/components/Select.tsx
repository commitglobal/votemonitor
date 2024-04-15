import React, { useMemo, useState } from "react";
import { Adapt, Select as TamaguiSelect, Sheet, SelectProps, styled } from "tamagui";
import { Icon } from "./Icon";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { Input } from "tamagui";

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
        <Sheet native modal snapPoints={[50, 40]} moveOnKeyboardChange={true}>
          <Sheet.Frame padding="$sm">
            <SearchInput value={searchTerm} onChangeText={setSearchTerm} />
            <Sheet.ScrollView marginBottom={insets.bottom}>
              <Adapt.Contents />
            </Sheet.ScrollView>
          </Sheet.Frame>
          <Sheet.Overlay />
        </Sheet>
      </Adapt>

      <TamaguiSelect.Content>
        <TamaguiSelect.Viewport>
          <TamaguiSelect.Group>
            {filteredOptions.map((entry, i) => {
              return (
                <TamaguiSelect.Item index={i} key={`${entry.id}_${i}`} value={entry.value} gap="$3">
                  <TamaguiSelect.ItemText width={"90%"} numberOfLines={1}>
                    {entry.label}
                  </TamaguiSelect.ItemText>
                  <TamaguiSelect.ItemIndicator>
                    <Icon icon="chevronLeft" />
                  </TamaguiSelect.ItemIndicator>
                </TamaguiSelect.Item>
              );
            })}
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
});
