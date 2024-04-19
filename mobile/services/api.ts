import * as SecureStore from "expo-secure-store";
import axios, { AxiosRequestHeaders } from "axios";
import { reloadAsync } from "expo-updates";

// https://vitejs.dev/guide/env-and-mode.html
const API = axios.create({
  baseURL: `https://votemonitor.staging.heroesof.tech/api/`,
  timeout: 100000,
  headers: {
    "Content-Type": "application/json",
  },
});

API.interceptors.request.use(async (request) => {
  // add auth header with jwt if account is logged in and request is to the api url
  try {
    const token = SecureStore.getItem("access_token");

    if (!request.headers) {
      request.headers = {} as AxiosRequestHeaders;
    }

    if (token) {
      request.headers.Authorization = `Bearer ${token}`;
    }
  } catch (err) {
    // User not authenticated. May be a public API.
    // Catches "The user is not authenticated".
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
        await SecureStore.deleteItemAsync("access_token");
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
