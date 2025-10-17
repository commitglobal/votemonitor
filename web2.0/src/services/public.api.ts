import axios from 'axios'

const API_BASE_URL = import.meta.env.VITE_API_URL

const TIMEOUT = 60 * 1000 // 60 seconds

const publicAPI = axios.create({
  baseURL: API_BASE_URL,
  timeout: TIMEOUT,
  headers: {
    'Content-Type': 'application/json',
  },
})

export default publicAPI
