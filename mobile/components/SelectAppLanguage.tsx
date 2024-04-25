import React, { useState, useMemo, useContext } from "react";
import { Adapt, Input, Select, Sheet, XStack, YStack, styled } from "tamagui";
import { LanguageContext } from "../contexts/language/LanguageContext.provider";
import { Keyboard } from "react-native";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import { useSafeAreaInsets } from "react-native-safe-area-context";

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

  /**
   * The currently selected language.
   */
  language: string;
  setLanguage: (state: string) => void;
}

const SelectAppLanguage = (props: SelectLanguageProps) => {
  const insets = useSafeAreaInsets();
  const { open, setOpen, language, setLanguage } = props;

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
        setLanguage(value);
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
              padding="$md"
              backgroundColor="white"
              gap="$sm"
            >
              <XStack backgroundColor="$purple1" borderRadius={8} alignItems="center">
                <Icon icon="search" color="transparent" size={20} marginLeft="$sm" />
                <SearchInput flex={1} value={searchTerm} onChangeText={setSearchTerm} />
              </XStack>
              <Typography preset="body2" color="$gray5">
                Language options:{" "}
              </Typography>
            </YStack>
            <Sheet.ScrollView
              marginBottom={insets.bottom}
              paddingHorizontal="$sm"
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
                      <Select.ItemText>{entry.label}</Select.ItemText>
                      <Select.ItemIndicator>
                        <Icon icon="chevronLeft" color="$purple5" />
                      </Select.ItemIndicator>
                    </Select.Item>
                  );
                }),
              [filteredLanguages, language],
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
