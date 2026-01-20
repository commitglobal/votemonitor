export function isNullOrEmpty(value?: string | null): boolean {
    return !value || value.trim() === "";
  }
  