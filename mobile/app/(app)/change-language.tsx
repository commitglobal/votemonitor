import React, { useContext } from "react";
import { View } from "react-native";
import { LanguageContext } from "../../contexts/language/LanguageContext.provider";
import Button from "../../components/Button";

const ChangeLanguage = () => {
  const { changeLanguage } = useContext(LanguageContext);

  const switchToEnglish = () => {
    changeLanguage("en");
  };

  const switchToRomanian = () => {
    changeLanguage("ro");
  };

  return (
    <View
      style={{
        flex: 1,
        justifyContent: "center",
        alignItems: "center",
        gap: 20,
      }}
    >
      <Button onPress={switchToEnglish}>English</Button>
      <Button onPress={switchToRomanian}>Romanian</Button>
    </View>
  );
};

export default ChangeLanguage;
