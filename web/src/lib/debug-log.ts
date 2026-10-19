import { isAxiosError } from 'axios';
import { toast } from 'sonner';

const pad = (value: number) => value.toString().padStart(2, '0');

function encodeBase64(text: string): string {
  const bytes = new TextEncoder().encode(text);
  let binary = '';
  bytes.forEach((byte) => {
    binary += String.fromCharCode(byte);
  });

  return btoa(binary);
}

function getRequestBody(error: unknown): string {
  if (isAxiosError(error) && error.config?.data !== undefined) {
    const data = error.config.data;
    return typeof data === 'string' ? data : JSON.stringify(data);
  }

  return '';
}

/**
 * Downloads a file containing the base64 encoded body of the request that caused the error.
 * File is named `debug log {yyyy-mm-dd}-{hh-mm-ss}.txt` (":" is not allowed in file names on Windows).
 */
export function downloadDebugLog(error: unknown): void {
  const now = new Date();
  const date = `${now.getFullYear()}-${pad(now.getMonth() + 1)}-${pad(now.getDate())}`;
  const time = `${pad(now.getHours())}-${pad(now.getMinutes())}-${pad(now.getSeconds())}`;

  const blob = new Blob([encodeBase64(getRequestBody(error))], { type: 'text/plain' });
  const url = URL.createObjectURL(blob);

  const link = document.createElement('a');
  link.href = url;
  link.download = `debug log ${date}-${time}.txt`;
  document.body.appendChild(link);
  link.click();
  link.remove();
  URL.revokeObjectURL(url);
}

/**
 * Shows an error toast with a "Copy debug info" action that downloads the failed request body.
 */
export function toastErrorWithDebugInfo(title: string, error: unknown): void {
  toast.error(title, {
    description: 'Please contact tech support',
    duration: Infinity,
    action: {
      label: 'Copy debug info',
      onClick: () => downloadDebugLog(error),
    },
  });
}
