import AddTextGuide from '@/features/election-event/components/Guides/AddTextGuide';
import { GuidePageType } from '@/features/election-event/models/guide';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/(app)/citizen-guides/new')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: NewCitizenGuide,
});

function NewCitizenGuide() {
  return <AddTextGuide guidePageType={GuidePageType.Citizen} />;
}
