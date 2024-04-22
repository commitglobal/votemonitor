/* eslint-disable @typescript-eslint/no-unsafe-member-access */
/* eslint-disable @typescript-eslint/no-unsafe-assignment */

import axios from 'axios';


const BASE_URL = 'https://votemonitor.staging.heroesof.tech/api/';

export const noAuthApi = axios.create({
  baseURL: BASE_URL,
  // withCredentials: true, // TODO Enable this when using a real login and authentication system
});

noAuthApi.defaults.headers.common['Content-Type'] = 'application/json';

