import axios from "axios";

// https://vitejs.dev/guide/env-and-mode.html
const API = axios.create({
  baseURL: `https://ce19-79-115-230-202.ngrok-free.app/`,
  timeout: 100000,
  headers: {
    "Content-Type": "application/json",
  },
});

// API.interceptors.request.use(async (request) => {
//   // add auth header with jwt if account is logged in and request is to the api url
//   try {
//     const user = await Auth.currentAuthenticatedUser({ bypassCache: true });

//     if (!request.headers) {
//       request.headers = {} as AxiosRequestHeaders;
//     }

//     if (user?.getSignInUserSession()) {
//       request.headers.Authorization = `Bearer ${user
//         .getSignInUserSession()
//         .getAccessToken()
//         .getJwtToken()}`;
//     }
//   } catch (err) {
//     // User not authenticated. May be a public API.
//     // Catches "The user is not authenticated".
//     return request;
//   }

//   return request;
// });

API.interceptors.response.use(
  async (response) => {
    return response;
  },
  async (error: any) => {
    // if not any of the auth error codes throw the error
    console.log(error);
    throw error;
  }
);

export default API;
