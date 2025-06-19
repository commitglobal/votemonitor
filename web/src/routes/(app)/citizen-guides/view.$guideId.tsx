import ViewTextGuide from '@/features/election-event/components/Guides/ViewTextGuide';
import { citizenGuideDetailsQueryOptions } from '@/features/election-event/hooks/citizen-guides-hooks';
import { GuidePageType } from '@/features/election-event/models/guide';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/(app)/citizen-guides/view/$guideId')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: ViewCitizenGuide,
  loader: async ({ context: { queryClient, currentElectionRoundContext }, params: { guideId } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    await queryClient.ensureQueryData(citizenGuideDetailsQueryOptions(electionRoundId, guideId));
  },
});

function ViewCitizenGuide() {
  const { guideId } = Route.useParams();

  return (
      <ViewTextGuide guidePageType={GuidePageType.Citizen} guideId={guideId} />
  );
}
