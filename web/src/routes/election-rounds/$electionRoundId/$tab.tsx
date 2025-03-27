import ElectionRoundDetails from '@/features/election-rounds/components/ElectionRoundDetails/ElectionRoundDetails';
import { ElectionRoundDetailsTab } from '@/features/election-rounds/components/ElectionRoundDetails/tabs';
import { electionRoundDetailsQueryOptions } from '@/features/election-rounds/queries';
import { redirectIfNotAuth, redirectIfNotPlatformAdmin } from '@/lib/utils';
import { createFileRoute, redirect } from '@tanstack/react-router';

const coerceTabSlug = (slug: string) => {
  if (slug?.toLowerCase()?.trim() === ElectionRoundDetailsTab.EventDetails) return ElectionRoundDetailsTab.EventDetails;
  if (slug?.toLowerCase()?.trim() === ElectionRoundDetailsTab.PollingStations)
    return ElectionRoundDetailsTab.PollingStations;
  if (slug?.toLowerCase()?.trim() === ElectionRoundDetailsTab.PsiForm) return ElectionRoundDetailsTab.PsiForm;
  if (slug?.toLowerCase()?.trim() === ElectionRoundDetailsTab.CitizenReporting)
    return ElectionRoundDetailsTab.CitizenReporting;
  if (slug?.toLowerCase()?.trim() === ElectionRoundDetailsTab.Locations) return ElectionRoundDetailsTab.Locations;
  if (slug?.toLowerCase()?.trim() === ElectionRoundDetailsTab.MonitoringNgos)
    return ElectionRoundDetailsTab.MonitoringNgos;
  if (slug?.toLowerCase()?.trim() === ElectionRoundDetailsTab.FormTemplates)
    return ElectionRoundDetailsTab.FormTemplates;

  return ElectionRoundDetailsTab.EventDetails;
};

export const Route = createFileRoute('/election-rounds/$electionRoundId/$tab')({
  component: ElectionRoundDetails,
  loader: ({ context: { queryClient }, params: { electionRoundId } }) =>
    queryClient.ensureQueryData(electionRoundDetailsQueryOptions(electionRoundId)),
  beforeLoad: ({ params: { tab, electionRoundId } }) => {
    redirectIfNotAuth();
    redirectIfNotPlatformAdmin();

    const coercedTab = coerceTabSlug(tab);
    if (tab !== coercedTab) {
      throw redirect({
        to: `/election-rounds/$electionRoundId/$tab`,
        params: { tab: coercedTab, electionRoundId },
        replace: true,
      });
    }
  },
});
