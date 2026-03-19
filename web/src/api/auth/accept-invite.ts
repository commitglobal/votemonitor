import { noAuthApi } from '@/common/no-auth-api';

export interface AcceptInviteRequest {
  password: string;
  confirmPassword: string;
  invitationToken: string;
}

export async function acceptInvite(payload: AcceptInviteRequest): Promise<void> {
  await noAuthApi.post<AcceptInviteRequest>('/auth/accept-invite', payload);
}

