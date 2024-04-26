import React, { useState, useMemo, useContext } from "react";
import { Adapt, Input, Select, Sheet, XStack, YStack, styled } from "tamagui";
import { LanguageContext } from "../contexts/language/LanguageContext.provider";
import { Keyboard } from "react-native";
import { Icon } from "./Icon";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import * as SecureStore from "expo-secure-store";

// TODO: Maybe this should be provided via LanguageContext provider
type LanguageOption = {
  label: string;
  value: string;
};

const languages: LanguageOption[] = [
  {
    label: "Romanian",
    value: "ro",
  },
  {
    label: "English",
    value: "en",
  },
];

interface SelectLanguageProps {
  /**
   * Determines whether the language selection is open or closed.
   * This is direclty controlled by the parent component.
   */
  open: boolean;
  setOpen: (state: boolean) => void;
}

const SelectAppLanguage = (props: SelectLanguageProps) => {
  const insets = useSafeAreaInsets();
  const { open, setOpen } = props;
  const appLanguage = SecureStore.getItem("app_language");

  // TODO: generalize this for all languages
  const { changeLanguage } = useContext(LanguageContext);
  const changeLanguageCallback = (value: string) => {
    if (value === "Romanian") {
      changeLanguage("ro");
    }
    if (value === "English") {
      changeLanguage("en");
    }
  };

  // Filter languages based on search term
  const [searchTerm, setSearchTerm] = useState("");
  const filteredLanguages = useMemo(() => {
    if (!searchTerm) return languages;
    return languages.filter((language) =>
      language.label.toLowerCase().includes(searchTerm.toLowerCase()),
    );
  }, [languages, searchTerm]);

  return (
    <Select
      disablePreventBodyScroll
      open={open}
      onOpenChange={setOpen}
      onValueChange={(value) => {
        Keyboard.dismiss();
        changeLanguageCallback(value);
        SecureStore.setItem("app_language", value);
      }}
    >
      <Adapt platform="touch">
        <Sheet
          native
          modal
          snapPoints={[40]}
          open={open}
          moveOnKeyboardChange={open || Keyboard.isVisible()}
        >
          <Sheet.Overlay />
          <Sheet.Frame>
            <YStack
              borderBottomWidth={1}
              borderBottomColor="$gray3"
              paddingHorizontal="$md"
              paddingVertical="$lg"
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
              paddingTop="$sm"
              keyboardShouldPersistTaps="handled"
            >
              <Adapt.Contents />
            </Sheet.ScrollView>
          </Sheet.Frame>
        </Sheet>
      </Adapt>

      <Select.Content>
        <Select.Viewport>
          <Select.Group>
            {useMemo(
              () =>
                filteredLanguages?.map((entry, i) => {
                  return (
                    <Select.Item
                      index={i}
                      key={`${entry}_${i}`}
                      value={entry.label}
                      gap="$3"
                      paddingBottom="$sm"
                    >
                      <Select.ItemText color={entry.label === appLanguage ? "$purple5" : "$gray9"}>
                        {entry.label}
                      </Select.ItemText>
                      <Select.ItemIndicator>
                        <Icon icon="chevronLeft" color="$purple5" />
                      </Select.ItemIndicator>
                    </Select.Item>
                  );
                }),
              [filteredLanguages, appLanguage],
            )}
          </Select.Group>
        </Select.Viewport>
      </Select.Content>
    </Select>
  );
};

const SearchInput = styled(Input, {
  backgroundColor: "$purple1",
  placeholder: "Search",
  color: "$purple5",
  placeholderTextColor: "$purple5",
  focusStyle: {
    borderColor: "transparent",
  },
});

export default SelectAppLanguage;
