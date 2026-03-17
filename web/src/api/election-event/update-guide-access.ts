import { authApi } from '@/common/auth-api';

export function updateGuideAccess(
  electionRoundId: string,
  coalitionId: string,
  guideId: string,
  ngoMembersIds: string[]
) {
  return authApi.put<void>(
    `/election-rounds/${electionRoundId}/coalitions/${coalitionId}/guides/${guideId}:access`,
    {
      ngoMembersIds,
    }
  );
}

