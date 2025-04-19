import { clsx, type ClassValue } from "clsx";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export const isProduction = import.meta.env.MODE === "production";

export async function sleep(ms: number) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}
