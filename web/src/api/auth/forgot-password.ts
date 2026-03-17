import { noAuthApi } from '@/common/no-auth-api';

export interface ForgotPasswordRequest {
  email: string;
}

export async function requestPasswordReset(payload: ForgotPasswordRequest): Promise<void> {
  await noAuthApi.post<ForgotPasswordRequest>('auth/forgot-password', payload);
}

