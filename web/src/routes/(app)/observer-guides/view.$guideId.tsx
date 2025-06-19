import ViewTextGuide from '@/features/election-event/components/Guides/ViewTextGuide';
import { observerGuideDetailsQueryOptions } from '@/features/election-event/hooks/observer-guides-hooks';
import { GuidePageType } from '@/features/election-event/models/guide';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/(app)/observer-guides/view/$guideId')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: ViewObserverGuide,
  loader: async ({ context: { queryClient, currentElectionRoundContext }, params: { guideId } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    await queryClient.ensureQueryData(observerGuideDetailsQueryOptions(electionRoundId, guideId));
  },
});

function ViewObserverGuide() {
  const { guideId } = Route.useParams();

  return <ViewTextGuide guidePageType={GuidePageType.Observer} guideId={guideId} />;
}
