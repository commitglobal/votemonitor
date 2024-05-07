import { RatingScaleType, UserPayload } from '@/common/types';
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

export function redirectIfNotAuth(): void {
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

export function ratingScaleToNumber(scale: RatingScaleType): number {
  switch (scale) {
    case RatingScaleType.OneTo3: {
      return 3;
    }
    case RatingScaleType.OneTo4: {
      return 4;
    }
    case RatingScaleType.OneTo5: {
      return 5;
    }
    case RatingScaleType.OneTo6: {
      return 6;
    }
    case RatingScaleType.OneTo7: {
      return 7;
    }
    case RatingScaleType.OneTo8: {
      return 8;
    }
    case RatingScaleType.OneTo9: {
      return 9;
    }
    case RatingScaleType.OneTo10: {
      return 10;
    }
    default: {
      return 5;
    }
  }
}
