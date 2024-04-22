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


export const authApi = axios.create({
  baseURL: import.meta.env.VITE_API_URL
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
