import AsyncStorage from "@react-native-async-storage/async-storage";
import { Linking, Platform } from "react-native";

// openURL rejects when no app can handle the URL (e.g. no browser installed)
export const openExternalUrl = (url: string) =>
  Linking.openURL(url).catch((err) => console.log("Could not open URL", url, err));

export const clearAsyncStorage = async () => {
  const asyncStorageKeys = await AsyncStorage.getAllKeys();
  if (asyncStorageKeys.length > 0) {
    if (Platform.OS === "android") {
      await AsyncStorage.clear();
    }
    if (Platform.OS === "ios") {
      await AsyncStorage.multiRemove(asyncStorageKeys);
    }
  }
};
