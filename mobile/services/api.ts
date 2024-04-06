import axios from "axios";

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
    // const user = await Auth.currentAuthenticatedUser({ bypassCache: true });

    // if (!request.headers) {
    //   request.headers = {} as AxiosRequestHeaders;
    // }

    // if (user?.getSignInUserSession()) {
    //   request.headers.Authorization = `Bearer ${user
    //     .getSignInUserSession()
    //     .getAccessToken()
    //     .getJwtToken()}`;
    // }
    const hardcodedToken =
      "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI2MDVlZTIxZC00YjRjLTQ2ZDMtOTYzMS1hNTc3YThhMzQ3NjIiLCJyb2xlIjoiT2JzZXJ2ZXIiLCJleHAiOjE3MTIzODk0MjAsImlhdCI6MTcxMjMwMzAyMCwibmJmIjoxNzEyMzAzMDIwfQ.PFzTfPX33ijCa6lPcWAs85s88kB3IZuxLQhesGXT4O8";
    request.headers.Authorization = `Bearer ${hardcodedToken}`;
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
        "❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️ API ERROR CAUGHT BY INTERCEPTOR ❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️"
      );
      console.log("Response data", error.response.data);
      console.log("Response status", error.response.status);
      console.log(error.response.headers);
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
    console.log(
      "❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️❗️"
    );
    throw error;
  }
);

export default API;
