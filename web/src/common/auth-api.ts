/* eslint-disable @typescript-eslint/no-unsafe-member-access */
/* eslint-disable @typescript-eslint/no-unsafe-assignment */

import { redirect } from '@tanstack/react-router';
import axios from 'axios';

export interface ILoginResponse {
  token: string;
  role: string;
}

export interface LoginDTO {
  email: string;
  password: string;
}

export const authApi = axios.create({
  baseURL: import.meta.env.VITE_API_URL
});

authApi.defaults.headers.common['Content-Type'] = 'application/json';
authApi.defaults.headers.common['Access-Control-Allow-Credentials'] = 'true';

authApi.interceptors.request.use(
  config => {
    const accessToken = localStorage.getItem('token');
    if (!!accessToken) {
      config.headers['Authorization'] = `Bearer ${accessToken}`;
    }

    return config;
  },
);

authApi.interceptors.response.use(
  (response) => {
    return response;
  },
  async (error) => {
    const originalRequest = error.config;

    if (error.response.status === 401) {
      if (!originalRequest._retry) {
        originalRequest._retry = true;

        const accessToken = localStorage.getItem('token');
        authApi.defaults.headers.common['Authorization'] = `Bearer ${accessToken}`;

        // eslint-disable-next-line @typescript-eslint/no-unsafe-argument
        return authApi(originalRequest);
      }

      // token is expired we need to relogin
      localStorage.removeItem('token');
      throw redirect({
        to: '/login',
      });

    }
    throw error;
  }
);
