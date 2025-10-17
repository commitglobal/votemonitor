import * as React from 'react'
import publicAPI from '@/services/api'
import { decodeToken } from '@/lib/jwt'
import { setAuthTokens } from '@/lib/utils'
import { STORAGE_KEYS } from '@/constants/storage-keys'

export type UserRole = 'PlatformAdmin' | 'NgoAdmin'

export interface ForgotPasswordApiResponse {
  success: boolean
  errors?: {
    name: string
    reason: string
  }[]
}

export interface AuthContext {
  isAuthenticated: boolean
  userRole: UserRole | null
  email: string | null
  forgotPasswordApiResponse?: ForgotPasswordApiResponse | null
  isLoading: boolean
  login: (email: string, password: string) => Promise<void>
  logout: () => Promise<void>
  forgotPassword: (email: string) => Promise<void>
  setForgotPasswordApiResponse: React.Dispatch<
    React.SetStateAction<ForgotPasswordApiResponse | null>
  >
}

const AuthContext = React.createContext<AuthContext>({
  isAuthenticated: false,
  isLoading: true,
  email: '',
  login: (email: string, password: string) => Promise.resolve(),
  forgotPassword: (email: string) => Promise.resolve(),
  setForgotPasswordApiResponse: () => {},
  logout: () => Promise.resolve(),
  userRole: 'NgoAdmin',
})

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [isAuthenticated, setIsAuthenticated] = React.useState<boolean>(false)
  const [userRole, setUserRole] = React.useState<UserRole | null>(null)
  const [email, setEmail] = React.useState<string | null>(null)
  const [isLoading, setIsLoading] = React.useState(true)
  const [forgotPasswordApiResponse, setForgotPasswordApiResponse] =
    React.useState<ForgotPasswordApiResponse | null>(null)

  React.useEffect(() => {
    try {
      init()
    } finally {
      setIsLoading(false)
    }
  }, [])

  const init = () => {
    try {
      const token = localStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN)
      if (token) {
        const decodedToken = decodeToken(token)
        setUserRole(decodedToken.role)
        setEmail(decodedToken.email)
      }
      setIsAuthenticated(!!token)
    } catch (err) {
      localStorage.removeItem(STORAGE_KEYS.ACCESS_TOKEN)
      localStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN)
      localStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN_EXPIRY_TIME)
    }
  }

  const logout = React.useCallback(async () => {
    setIsAuthenticated(false)
    localStorage.removeItem(STORAGE_KEYS.ACCESS_TOKEN)
    localStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN)
    localStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN_EXPIRY_TIME)
  }, [])

  const login = async (email: string, password: string) => {
    setIsLoading(true)
    try {
      const {
        data: { token, refreshToken, refreshTokenExpiryTime },
      } = await publicAPI.post('auth/login', {
        email,
        password,
      })

      setAuthTokens(token, refreshToken, refreshTokenExpiryTime)
      const decodedToken = decodeToken(token)
      setUserRole(decodedToken.role)
      setEmail(decodedToken.email)

      setIsAuthenticated(true)
    } catch (err: unknown) {
      console.log('Error while trying to sign in', err)
      setIsAuthenticated(false)
      throw new Error('Error while trying to sign in')
    } finally {
      setIsLoading(false)
    }
  }

  const forgotPassword = async (email: string) => {
    setIsLoading(true)
    let response
    try {
      response = await publicAPI.post('auth/forgot-password', {
        email,
      })
      setForgotPasswordApiResponse({ success: true })
    } catch (err: unknown) {
      console.log('Error while trying to send forgot password email', err)
      setForgotPasswordApiResponse({
        success: false,
        errors: response?.data.errors,
      })
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <AuthContext.Provider
      value={{
        isAuthenticated,
        userRole,
        email,
        forgotPasswordApiResponse,
        setForgotPasswordApiResponse,
        isLoading,
        login,
        forgotPassword,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const context = React.useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return context
}
