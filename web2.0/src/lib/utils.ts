import { STORAGE_KEYS } from "@/constants/storage-keys";
import { clsx, type ClassValue } from "clsx";
import { twMerge } from "tailwind-merge";
import axios from "axios";
import { toast } from "sonner";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export const isProduction = import.meta.env.MODE === "production";

export async function sleep(ms: number) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

export const setAuthTokens = (
  token: string,
  refreshToken: string,
  refreshTokenExpiryTime: string
) => {
  localStorage.setItem(STORAGE_KEYS.ACCESS_TOKEN, token);
  localStorage.setItem(STORAGE_KEYS.REFRESH_TOKEN, refreshToken);
  localStorage.setItem(
    STORAGE_KEYS.REFRESH_TOKEN_EXPIRY_TIME,
    refreshTokenExpiryTime
  );
};

export function buildURLSearchParams(data: any) {
  const params = new URLSearchParams();

  Object.entries(data)
    .filter(([_, value]) => !!value)
    .forEach(([key, value]) => {
      if (Array.isArray(value)) {
        // @ts-ignore
        value.forEach((value) => params.append(key, value.toString()));
      } else {
        // @ts-ignore
        params.append(key, value.toString());
      }
    });

  return params;
}

export const downloadFile = async (presignedUrl: string, fileName: string) => {
  try {
    const response = await axios.get(presignedUrl, {
      responseType: "blob",
      headers: {},
    });

    // Create download link
    const url = window.URL.createObjectURL(new Blob([response.data]));
    const link = document.createElement("a");
    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  } catch (error) {
    toast.error("Download failed");
  }
};
