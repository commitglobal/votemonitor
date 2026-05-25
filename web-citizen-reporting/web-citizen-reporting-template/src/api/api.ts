/* eslint-disable @typescript-eslint/no-unsafe-member-access */
/* eslint-disable @typescript-eslint/no-unsafe-assignment */

import axios from "axios";

export const API = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
});

API.defaults.headers.common["Content-Type"] = "application/json";
