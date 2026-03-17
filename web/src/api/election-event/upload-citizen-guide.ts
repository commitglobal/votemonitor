import { authApi } from '@/common/auth-api';

export function uploadCitizenGuide(electionRoundId: string, formData: FormData) {
  return authApi.post(`/election-rounds/${electionRoundId}/citizen-guides`, formData, {
    headers: {
      'Content-Type': 'multipart/form-data',
    },
  });
}

