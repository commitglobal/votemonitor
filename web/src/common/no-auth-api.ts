/* eslint-disable @typescript-eslint/no-unsafe-member-access */
/* eslint-disable @typescript-eslint/no-unsafe-assignment */

import axios from 'axios';

export const noAuthApi = axios.create({
  baseURL: import.meta.env.VITE_API_URL
  // withCredentials: true, // TODO Enable this when using a real login and authentication system
});

noAuthApi.defaults.headers.common['Content-Type'] = 'application/json';

