import * as SecureStore from "expo-secure-store";
import * as Sentry from "@sentry/react-native";

export const setSecureStoreItem = (key: string, value: string): void => {
  try {
    SecureStore.setItem(key, value);
  } catch (error) {
    Sentry.captureException(error);
    console.error("secure store set item error: ", error);
  }
};

export const deleteSecureStoreItemAsync = async (key: string): Promise<void> => {
  try {
    await SecureStore.deleteItemAsync(key);
  } catch (error) {
    Sentry.captureException(error);
    console.error("secure store delete item error: ", error);
  }
};

export const getSecureStoreItem = (key: string): string | null => {
  try {
    const item = SecureStore.getItem(key);
    if (item) {
      console.log(`${key} was used 🔐 \n`);
    } else {
      console.log("No values stored under key: " + key);
    }
    return item;
  } catch (error) {
    Sentry.captureException(error);
    console.error("secure store get item error: ", error);
    deleteSecureStoreItemAsync(key); // HERE!
    return null;
  }
};
