import { authApi } from '@/common/auth-api';

export function uploadObserverGuide(electionRoundId: string, formData: FormData) {
  return authApi.post(`/election-rounds/${electionRoundId}/observer-guide`, formData, {
    headers: {
      'Content-Type': 'multipart/form-data',
    },
  });
}

