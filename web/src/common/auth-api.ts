/* eslint-disable @typescript-eslint/no-unsafe-member-access */
/* eslint-disable @typescript-eslint/no-unsafe-assignment */

import axios from 'axios';

export interface ILoginResponse {
  token: string;
}

export interface LoginDTO {
  email: string;
  password: string;
}

const BASE_URL = 'https://votemonitor.staging.heroesof.tech/api/';

export const authApi = axios.create({
  baseURL: BASE_URL,
  // withCredentials: true, // TODO Enable this when using a real login and authentication system
});

authApi.defaults.headers.common['Content-Type'] = 'application/json';
authApi.defaults.headers.common['Access-Control-Allow-Credentials'] = 'true';

authApi.interceptors.response.use(
  (response) => {
    return response;
  },
  async (error) => {
    const originalRequest = error.config;
    if (error.response.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      const accessToken = localStorage.getItem('token');
      authApi.defaults.headers.common['Authorization'] = `Bearer ${accessToken}`;

      // eslint-disable-next-line @typescript-eslint/no-unsafe-argument
      return authApi(originalRequest);
    }
    throw error;
  }
);
