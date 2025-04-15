import axios, { AxiosRequestHeaders } from "axios";
import * as Sentry from "@sentry/react-native";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { ASYNC_STORAGE_KEYS } from "../common/constants";
import { router } from "expo-router";
import { setAuthTokens } from "../helpers/authTokensSetter";

/*
 * Token Refresh Mechanism:
 *
 * 1. When an API call fails with a 401 or token-expired header:
 *    - If not already refreshing, initiates a token refresh
 *    - If already refreshing, adds request to queue of subscribers
 *
 * 2. Token refresh process:
 *    - Gets current tokens from AsyncStorage
 *    - Calls refresh endpoint with existing tokens
 *    - Stores new tokens in AsyncStorage
 *    - Updates original request with new token
 *    - Retries original request
 *
 * 3. For concurrent requests during refresh:
 *    - Queues them as subscribers
 *    - Once refresh completes, replays all queued requests with new token
 *    - Prevents multiple simultaneous refresh calls
 *
 * 4. On refresh failure:
 *    - Clears tokens from storage
 *    - Redirects to login
 */

const API_BASE_URL = process.env.EXPO_PUBLIC_API_BASE_URL;

const TIMEOUT = 60 * 1000; // 60 seconds

class TokenRefreshManager {
  private isRefreshing = false;
  private refreshSubscribers: ((token: string) => void)[] = [];

  onRefreshed(token: string) {
    this.refreshSubscribers.forEach((callback) => callback(token));
    this.refreshSubscribers = [];
  }

  addRefreshSubscriber(callback: (token: string) => void) {
    this.refreshSubscribers.push(callback);
  }

  setRefreshing(value: boolean) {
    this.isRefreshing = value;
  }

  isCurrentlyRefreshing() {
    return this.isRefreshing;
  }

  clearSubscribers() {
    this.refreshSubscribers = [];
  }
}

const tokenManager = new TokenRefreshManager();

const API = axios.create({
  baseURL: API_BASE_URL,
  timeout: TIMEOUT,
  headers: {
    "Content-Type": "application/json",
  },
});

API.interceptors.request.use(async (request) => {
  try {
    const token = await AsyncStorage.getItem(ASYNC_STORAGE_KEYS.ACCESS_TOKEN);

    if (!request.headers) {
      request.headers = {} as AxiosRequestHeaders;
    }

    if (token) {
      request.headers.Authorization = `Bearer ${token}`;
    }
  } catch (err) {
    Sentry.captureException(err);
  }

  return request;
});

const handleTokenRefresh = async (originalRequest: any) => {
  try {
    const [token, refreshToken] = await Promise.all([
      AsyncStorage.getItem(ASYNC_STORAGE_KEYS.ACCESS_TOKEN),
      AsyncStorage.getItem(ASYNC_STORAGE_KEYS.REFRESH_TOKEN),
    ]);

    if (!token || !refreshToken) {
      throw new Error("No tokens available");
    }

    const { data } = await axios.post(`${API_BASE_URL}auth/refresh`, {
      token,
      refreshToken,
    });

    await setAuthTokens(data.token, data.refreshToken, data.refreshTokenExpiryTime);
    tokenManager.onRefreshed(data.token);

    originalRequest.headers.Authorization = `Bearer ${data.token}`;
    return API(originalRequest);
  } catch (error) {
    await AsyncStorage.multiRemove([
      ASYNC_STORAGE_KEYS.ACCESS_TOKEN,
      ASYNC_STORAGE_KEYS.REFRESH_TOKEN,
    ]);
    router.replace("/login");
    throw error;
  } finally {
    tokenManager.setRefreshing(false);
  }
};

API.interceptors.response.use(
  (response) => response,
  async (error: any) => {
    const originalRequest = error.config;
    const isTokenExpiredError =
      error.response?.headers["token-expired"] === "true" || error.response?.status === 401;

    if (
      error.response &&
      isTokenExpiredError &&
      !originalRequest?.url?.includes("auth") &&
      !originalRequest._retry
    ) {
      if (tokenManager.isCurrentlyRefreshing()) {
        return new Promise((resolve) => {
          tokenManager.addRefreshSubscriber(async (token: string) => {
            originalRequest.headers.Authorization = `Bearer ${token}`;
            resolve(API(originalRequest));
          });
        });
      }

      console.log("❌ [API FAILED] URL", originalRequest.url);
      console.log("❌ [API FAILED] Status", error.response?.status);
      console.log("❌ [API FAILED] isTokenExpiredError", isTokenExpiredError);

      originalRequest._retry = true;
      tokenManager.setRefreshing(true);
      return handleTokenRefresh(originalRequest);
    }

    if (error.request) {
      console.log("❌ [API FAILED] Request", error.request);
      Sentry.captureException(new Error("Network request failed"), {
        extra: { request: error.request },
      });
    } else {
      console.log("❌ [API FAILED] Error", error);
      Sentry.captureException(error);
    }

    throw error;
  },
);

export default API;
