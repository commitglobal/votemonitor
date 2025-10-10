import axios from 'axios'
import { clsx, type ClassValue } from 'clsx'
import { toast } from 'sonner'
import { twMerge } from 'tailwind-merge'
import { STORAGE_KEYS } from '@/constants/storage-keys'

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}

export const isProduction = import.meta.env.MODE === 'production'

export const setAuthTokens = (
  token: string,
  refreshToken: string,
  refreshTokenExpiryTime: string
) => {
  localStorage.setItem(STORAGE_KEYS.ACCESS_TOKEN, token)
  localStorage.setItem(STORAGE_KEYS.REFRESH_TOKEN, refreshToken)
  localStorage.setItem(
    STORAGE_KEYS.REFRESH_TOKEN_EXPIRY_TIME,
    refreshTokenExpiryTime
  )
}

export function buildURLSearchParams(data: any) {
  const params = new URLSearchParams()

  Object.entries(data)
    .filter(([_, value]) => !!value)
    .forEach(([key, value]) => {
      if (Array.isArray(value)) {
        // @ts-ignore
        value.forEach((value) => params.append(key, value.toString()))
      } else {
        // @ts-ignore
        params.append(key, value.toString())
      }
    })

  return params
}

export const downloadFile = async (presignedUrl: string, fileName: string) => {
  try {
    const response = await axios.get(presignedUrl, {
      responseType: 'blob',
      headers: {},
    })

    // Create download link
    const url = window.URL.createObjectURL(new Blob([response.data]))
    const link = document.createElement('a')
    link.href = url
    link.download = fileName
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)
  } catch (error) {
    toast.error('Download failed')
  }
}

// Convert array to object with specified key
// CAUTION: will remove duplicate objects from array if the key matches
type StringKeys<T> = {
  [K in keyof T]: T[K] extends string | number | symbol ? K : never
}[keyof T]
export const arrayToKeyObject = <
  T extends Record<StringKeys<T>, string | number | symbol>,
  TKeyName extends keyof Record<StringKeys<T>, string | number | symbol>,
>(
  array: T[],
  key: TKeyName
): Record<T[TKeyName], T> =>
  Object.fromEntries(array.map((a) => [a[key], a])) as Record<T[TKeyName], T>

export const groupArrayByKey = <
  T extends Record<StringKeys<T>, string | number | symbol>,
  TKeyName extends keyof T,
>(
  array: T[],
  key: TKeyName
): Record<T[TKeyName], T[]> => {
  return array.reduce(
    (acc, item) => {
      const k = item[key]
      if (!acc[k]) {
        acc[k] = []
      }
      acc[k].push(item)
      return acc
    },
    {} as Record<T[TKeyName], T[]>
  )
}
