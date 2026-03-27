import { noAuthApi } from '@/common/no-auth-api';

export interface ResetPasswordRequest {
  password: string;
  token: string;
  email: string;
}

export async function resetPassword(payload: ResetPasswordRequest): Promise<void> {
  await noAuthApi.post<ResetPasswordRequest>('auth/reset-password', payload);
}

