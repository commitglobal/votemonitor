import EditTextGuide from '@/features/election-event/components/Guides/EditTextGuide';
import { citizenGuideDetailsQueryOptions } from '@/features/election-event/hooks/citizen-guides-hooks';
import { GuidePageType } from '@/features/election-event/models/guide';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/citizen-guides/edit/$guideId')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: EditCitizenGuide,
  loader: async ({ context: { queryClient, currentElectionRoundContext }, params: { guideId } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    await queryClient.ensureQueryData(citizenGuideDetailsQueryOptions(electionRoundId, guideId));
  },
});

function EditCitizenGuide() {
  const { guideId } = Route.useParams();

  return <EditTextGuide guidePageType={GuidePageType.Citizen} guideId={guideId} />;
}
