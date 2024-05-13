import React, { useEffect, useMemo, useState } from "react";
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
import { Keyboard } from "react-native";
import { ListView } from "./ListView";
import { number } from "zod";

interface StyledSelectProps extends SelectProps {
  placeholder?: string;
  options: { id: string | number; value: string; label: string }[];
}

const Select = ({ placeholder = "Select", options, ...props }: StyledSelectProps) => {
  const insets = useSafeAreaInsets();
  const [searchTerm, setSearchTerm] = useState<string>("");
  const [isOpen, setIsOpen] = useState(false);
  const [numberOfItems, setNumberOfItems] = useState(10);

  // Filter options based on search term
  const filteredOptions = useMemo(() => {
    if (!searchTerm) return options.slice(0, numberOfItems);
    return options.filter((option) =>
      option.label.toLowerCase().includes(searchTerm.toLowerCase()),
    ).slice(0, numberOfItems);;
  }, [options, searchTerm, numberOfItems]);

  const loadMore = () => {
    console.log('🙈 🙈 🙈 🙈 🙈 🙈 ......loading more......')
    setNumberOfItems((num) => num + 10)
  }

  // if (!onValueChange) {
  //   return;
  // }

  // return (
  //   <YStack
  //     height={45 * filteredOptions.length}>
  //     <ListView<{ id: string | number; value: string; label: string }>
  //       data={filteredOptions}
  //       showsVerticalScrollIndicator={false}
  //       bounces={false}
  //       estimatedItemSize={50}
  //       renderItem={({ item, index }) => <Typography onPress={() => { onValueChange(item.value) }}>{item.label}</Typography>}
  //     />
  //   </YStack>
  // );

  return (
    <TamaguiSelect
      disablePreventBodyScroll
      native
      onOpenChange={(open) => {
        Keyboard.dismiss();
        return setIsOpen(open);
      }}
      {...props}
    >
      <TamaguiSelect.Trigger
        backgroundColor="white"
        paddingHorizontal="$md"
        borderColor="$gray3"
        borderWidth={1}
        iconAfter={<Icon icon="chevronRight" size={20} transform="rotate(90deg)" color="$gray7" />}
      >
        <TamaguiSelect.Value
          width={"90%"}
          color="$gray5"
          placeholder={placeholder}
          fontSize={16}
        // fontWeight="500"
        ></TamaguiSelect.Value>
      </TamaguiSelect.Trigger>

      <Adapt platform="touch">
        <Sheet
          native
          modal
          snapPoints={[50]}
          open={isOpen}
          moveOnKeyboardChange={isOpen || Keyboard.isVisible()}
        >
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
        <TamaguiSelect.Viewport >
          <TamaguiSelect.Group
            height={
              50 * filteredOptions.length
            }
          >

            {filteredOptions.length === 0 ? (
              <Typography padding="$md">No data found for current search.</Typography>
            ) : (
              <ListView<{ id: string | number; value: string; label: string }>
                data={filteredOptions}
                onEndReached={loadMore}
                onEndReachedThreshold={0.1}
                showsVerticalScrollIndicator={false}
                bounces={false}
                estimatedItemSize={50}
                renderItem={({ item, index }) => <SelectItem item={item} index={index} />}
              />
            )}
          </TamaguiSelect.Group>
        </TamaguiSelect.Viewport>
      </TamaguiSelect.Content>

      {/* <TamaguiSelect.Content>
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
      </TamaguiSelect.Content> */}
    </TamaguiSelect >
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


const SelectItem = ({ item, index }: { item: any, index: number }) => {
  return (
    <TamaguiSelect.Item
      index={index}
      value={item?.value}
      gap="$3"
      paddingBottom="$sm"
    >
      <TamaguiSelect.ItemText width={"90%"} numberOfLines={1}>
        {item?.label}
      </TamaguiSelect.ItemText>
      <TamaguiSelect.ItemIndicator>
        <Icon icon="chevronLeft" />
      </TamaguiSelect.ItemIndicator>
    </TamaguiSelect.Item>
  )
};

// const SelectItem2 = ({ item, index }: { item: any, index: number }) => {
//   return (
//     <Typography>{item.label{</Typography>
//   )
// };