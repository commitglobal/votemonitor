import React, { useContext } from "react";
import { Adapt, Select, Sheet } from "tamagui";
import { Language, LanguageContext } from "../contexts/language/LanguageContext.provider";
import { Keyboard } from "react-native";
import { Icon } from "./Icon";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { useTranslation } from "react-i18next";
import * as SecureStore from "expo-secure-store";
import { I18N_LANGUAGE } from "../common/constants";

interface SelectLanguageProps {
  open: boolean;
  setOpen: (isOpen: boolean) => void;
}

const SelectAppLanguage = ({ open, setOpen }: SelectLanguageProps) => {
  const insets = useSafeAreaInsets();
  const { t, i18n } = useTranslation("languages");
  const { changeLanguage } = useContext(LanguageContext);

  const onChangeLanguage = (language: Language) => {
    Keyboard.dismiss();
    changeLanguage(language);
    SecureStore.setItem(I18N_LANGUAGE, language);
  };

  return (
    <Select
      disablePreventBodyScroll
      open={open}
      onOpenChange={setOpen}
      onValueChange={onChangeLanguage}
    >
      <Adapt platform="touch">
        <Sheet
          modal
          snapPointsMode="fit"
          open={open}
          moveOnKeyboardChange={open || Keyboard.isVisible()}
        >
          <Sheet.Overlay />
          <Sheet.Frame>
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
            {i18n.languages?.map((lang, i) => (
              <Select.Item index={i} key={lang} value={lang} gap="$3" paddingBottom="$sm">
                <Select.ItemText
                  color={lang === i18n.language ? "$purple5" : "$gray9"}
                  maxFontSizeMultiplier={1.2}
                >
                  {t(lang)}
                </Select.ItemText>
                <Select.ItemIndicator>
                  <Icon icon="chevronLeft" color="$purple5" />
                </Select.ItemIndicator>
              </Select.Item>
            ))}
          </Select.Group>
        </Select.Viewport>
      </Select.Content>
    </Select>
  );
};

export default SelectAppLanguage;
