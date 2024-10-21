import { Platform } from "react-native";

const scrollToPadding = 16;

export const scrollToTextarea = (scrollViewRef: any, textareaRef: any) => {
  if (Platform.OS === "ios") {
    if (scrollViewRef.current && textareaRef.current) {
      (textareaRef.current as any).measureLayout(
        scrollViewRef.current,
        (x: number, y: number, _width: number, _height: number) => {
          setTimeout(() => {
            (scrollViewRef.current as any).scrollTo({ y: y - scrollToPadding, animated: true });
          }, 100);
        },
        () => {},
      );
    }
  }
};
