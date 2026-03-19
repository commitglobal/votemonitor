import { authApi } from '@/common/auth-api';

export interface SetPasswordRequest {
  aspNetUserId: string;
  newPassword: string;
}

export async function setPassword(payload: SetPasswordRequest): Promise<void> {
  await authApi.post<SetPasswordRequest>('auth/set-password', payload);
}

