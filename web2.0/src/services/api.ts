import axios, { type AxiosRequestHeaders } from 'axios'
import { router } from '@/main'
import { setAuthTokens } from '@/lib/utils'
import { STORAGE_KEYS } from '@/constants/storage-keys'

/*
 * Token Refresh Mechanism:
 *
 * 1. When an API call fails with a 401 or token-expired header:
 *    - If not already refreshing, initiates a token refresh
 *    - If already refreshing, adds request to queue of subscribers
 *
 * 2. Token refresh process:
 *    - Gets current tokens from AsyncStorage
 *    - Calls refresh endpoint with existing tokens
 *    - Stores new tokens in AsyncStorage
 *    - Updates original request with new token
 *    - Retries original request
 *
 * 3. For concurrent requests during refresh:
 *    - Queues them as subscribers
 *    - Once refresh completes, replays all queued requests with new token
 *    - Prevents multiple simultaneous refresh calls
 *
 * 4. On refresh failure:
 *    - Clears tokens from storage
 *    - Redirects to login
 */

const API_BASE_URL = import.meta.env.VITE_API_URL

const TIMEOUT = 60 * 1000 // 60 seconds

class TokenRefreshManager {
  private isRefreshing = false
  private refreshSubscribers: ((token: string) => void)[] = []

  onRefreshed(token: string) {
    this.refreshSubscribers.forEach((callback) => callback(token))
    this.refreshSubscribers = []
  }

  addRefreshSubscriber(callback: (token: string) => void) {
    this.refreshSubscribers.push(callback)
  }

  setRefreshing(value: boolean) {
    this.isRefreshing = value
  }

  isCurrentlyRefreshing() {
    return this.isRefreshing
  }

  clearSubscribers() {
    this.refreshSubscribers = []
  }
}

const tokenManager = new TokenRefreshManager()

const API = axios.create({
  baseURL: API_BASE_URL,
  timeout: TIMEOUT,
  headers: {
    'Content-Type': 'application/json',
  },
})

API.interceptors.request.use(async (request) => {
  try {
    const token = localStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN)

    if (!request.headers) {
      request.headers = {} as AxiosRequestHeaders
    }

    if (token) {
      request.headers.Authorization = `Bearer ${token}`
    }
  } catch (err) {
    console.error(err)
    // Sentry.captureException(err);
  }

  return request
})

const handleTokenRefresh = async (originalRequest: any) => {
  try {
    const [token, refreshToken] = [
      localStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN),
      localStorage.getItem(STORAGE_KEYS.REFRESH_TOKEN),
    ]

    if (!token || !refreshToken) {
      console.log('No tokens available')
      throw new Error('No tokens available')
    }

    const { data } = await axios.post(`${API_BASE_URL}auth/refresh`, {
      token,
      refreshToken,
    })

    setAuthTokens(data.token, data.refreshToken, data.refreshTokenExpiryTime)
    tokenManager.onRefreshed(data.token)

    originalRequest.headers.Authorization = `Bearer ${data.token}`
    return API(originalRequest)
  } catch (error) {
    localStorage.removeItem(STORAGE_KEYS.ACCESS_TOKEN)
    localStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN)
    console.log(error)
    console.log('redirect')
    router.navigate({ to: '/login' })
    throw error
  } finally {
    tokenManager.setRefreshing(false)
  }
}

API.interceptors.response.use(
  (response) => response,
  async (error: any) => {
    const originalRequest = error.config
    const isTokenExpiredError =
      error.response?.headers['token-expired'] === 'true' ||
      error.response?.status === 401

    if (
      error.response &&
      isTokenExpiredError &&
      !originalRequest?.url?.includes('auth') &&
      !originalRequest._retry
    ) {
      if (tokenManager.isCurrentlyRefreshing()) {
        return new Promise((resolve) => {
          tokenManager.addRefreshSubscriber(async (token: string) => {
            originalRequest.headers.Authorization = `Bearer ${token}`
            resolve(API(originalRequest))
          })
        })
      }

      console.log('❌ [API FAILED] URL', originalRequest.url)
      console.log('❌ [API FAILED] Status', error.response?.status)
      console.log('❌ [API FAILED] isTokenExpiredError', isTokenExpiredError)

      originalRequest._retry = true
      tokenManager.setRefreshing(true)
      return handleTokenRefresh(originalRequest)
    }

    if (error.request) {
      console.log('❌ [API FAILED] Request', error.request)
      // Sentry.captureException(new Error("Network request failed"), {
      //   extra: { request: error.request },
      // });
    } else {
      console.log('❌ [API FAILED] Error', error)
      // Sentry.captureException(error);
    }

    throw error
  }
)

export default API
