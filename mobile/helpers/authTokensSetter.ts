import AsyncStorage from "@react-native-async-storage/async-storage";
import { ASYNC_STORAGE_KEYS } from "../common/constants";

export const setAuthTokens = async (
  token: string,
  refreshToken: string,
  refreshTokenExpiryTime: string,
) => {
  await AsyncStorage.setItem(ASYNC_STORAGE_KEYS.ACCESS_TOKEN, token);
  await AsyncStorage.setItem(ASYNC_STORAGE_KEYS.REFRESH_TOKEN, refreshToken);
  await AsyncStorage.setItem(ASYNC_STORAGE_KEYS.REFRESH_TOKEN_EXPIRY_TIME, refreshTokenExpiryTime);
};
