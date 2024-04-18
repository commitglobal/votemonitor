import { UserPayload } from '@/common/types';
import { redirect } from '@tanstack/react-router';
import { type ClassValue, clsx } from 'clsx';
import { twMerge } from 'tailwind-merge';

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export function valueOrDefault(value: number | null | undefined, fallbackValue: number): number {
  if (value === null || value === undefined || isNaN(value)) {
    return fallbackValue;
  }

  return value;
}

export function stringToText(str: string) {
  let hash = 0;
  for (let i = 0; i < str.length; i++) {
    hash = str.charCodeAt(i) + ((hash << 5) - hash);
  }
  let colour = '#';
  for (let i = 0; i < 3; i++) {
    let value = (hash >> (i * 8)) & 0xff;
    colour += ('00' + value.toString(16)).substr(-2);
  }
  return colour;
}

export function redirectIfNotAuth(isAuthenticated: boolean) {
  const token = localStorage.getItem('token');
  if (!token) {
    throw redirect({
      to: '/login',
    });
  }
}

export function parseJwt(token: string | undefined): UserPayload {
  let base64Url: string | undefined = token!.split('.')[1];
  let base64 = base64Url!.replace(/-/g, '+').replace(/_/g, '/');
  let jsonPayload = decodeURIComponent(
    window
      .atob(base64)
      .split('')
      .map(function (c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
      })
      .join('')
  );

  return JSON.parse(jsonPayload) as UserPayload;
}
