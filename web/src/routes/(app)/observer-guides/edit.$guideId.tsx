import EditTextGuide from '@/features/election-event/components/Guides/EditTextGuide';
import { observerGuideDetailsQueryOptions } from '@/features/election-event/hooks/observer-guides-hooks';
import { GuidePageType } from '@/features/election-event/models/guide';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/(app)/observer-guides/edit/$guideId')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: EditObserverGuide,
  loader: async ({ context: { queryClient, currentElectionRoundContext }, params: { guideId } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    await queryClient.ensureQueryData(observerGuideDetailsQueryOptions(electionRoundId, guideId));
  },
});

function EditObserverGuide() {
  const { guideId } = Route.useParams();

  return <EditTextGuide guidePageType={GuidePageType.Observer} guideId={guideId} />;
}
