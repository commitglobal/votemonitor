import axios, { AxiosError, AxiosRequestHeaders } from "axios";
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

const API_BASE_URL = "https://api.votemonitor.org/api/";

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

    console.log("❌ [API FAILED] Error", error);
    reportApiError(error);

    throw error;
  },
);

const UUID_REGEX = /[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}/gi;

// Replace ids so the same endpoint is grouped into a single Sentry issue
const normalizeUrl = (url?: string) =>
  (url ?? "").split("?")[0].replace(UUID_REGEX, "{id}").replace(/^\//, "");

const reportApiError = (error: AxiosError) => {
  if (axios.isCancel(error)) {
    return;
  }

  const method = error.config?.method?.toUpperCase() ?? "";
  const url = normalizeUrl(error.config?.url);
  const status = error.response?.status;

  // Wrong credentials are a user error, not an app error
  if (url === "auth/login" && (status === 400 || status === 401)) {
    return;
  }

  let title: string;
  let level: Sentry.SeverityLevel;

  if (status) {
    // The server responded with an error status
    title = `API ${status} ${method} ${url}`;
    level = status >= 500 ? "error" : "warning";
  } else if (error.request) {
    // No response: timeout or no connectivity
    const isTimeout = error.code === "ECONNABORTED" || error.code === "ETIMEDOUT";
    title = `API ${isTimeout ? "timeout" : "network error"} ${method} ${url}`;
    level = "warning";
  } else {
    // The request could not be created
    Sentry.captureException(error);
    return;
  }

  const apiError = new Error(title);
  apiError.name = "ApiError";

  Sentry.captureException(apiError, {
    level,
    fingerprint: ["api-error", method, url, String(status ?? error.code)],
    contexts: {
      api: {
        method,
        url: error.config?.url,
        status,
        code: error.code,
        message: error.message,
        responseData: JSON.stringify(error.response?.data)?.slice(0, 1000),
      },
    },
  });
};

export default API;
