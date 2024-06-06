import axios, { AxiosRequestHeaders } from "axios";
import * as Sentry from "@sentry/react-native";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { ASYNC_STORAGE_KEYS } from "../common/constants";
import { reloadAsync } from "expo-updates";

const API = axios.create({
  // baseURL: `https://votemonitor.staging.heroesof.tech/api/`,
  baseURL: `https://api.votemonitor.org/api/`,
  timeout: 100000,
  headers: {
    "Content-Type": "application/json",
  },
});

API.interceptors.request.use(async (request) => {
  // add auth header with jwt if account is logged in and request is to the api url
  try {
    const token = await AsyncStorage.getItem(ASYNC_STORAGE_KEYS.ACCESS_TOKEN);

    if (!request.headers) {
      request.headers = {} as AxiosRequestHeaders;
    }

    if (token) {
      request.headers.Authorization = `Bearer ${token}`;
    }
  } catch (err) {
    // User not authenticated. May be a public API.
    // Catches "The user is not authenticated".
    Sentry.captureException(err);
    return request;
  }

  return request;
});

API.interceptors.response.use(
  async (response) => {
    return response;
  },
  async (error: any) => {
    if (error.response) {
      // The request was made and the server responded with a status code
      // that falls out of the range of 2xx
      console.log(
        "❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️ API ERROR CAUGHT BY INTERCEPTOR ❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️",
      );
      console.log("Response data", error.response.data);
      console.log("Response status", error.response.status);
      console.log(error.response.headers);

      if (error.response.status === 401) {
        await AsyncStorage.removeItem(ASYNC_STORAGE_KEYS.ACCESS_TOKEN);
        reloadAsync();
      }
    } else if (error.request) {
      // The request was made but no response was received
      // `error.request` is an instance of XMLHttpRequest in the browser and an instance of
      // http.ClientRequest in node.js
      console.log(error.request);
    } else {
      // Something happened in setting up the request that triggered an Error
      console.log("Error", error.message);
    }
    console.log(error.config);
    console.log("❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️");
    throw error;
  },
);

export default API;
