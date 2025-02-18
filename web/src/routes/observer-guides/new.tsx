import AddTextGuide from '@/features/election-event/components/Guides/AddTextGuide';
import { GuidePageType } from '@/features/election-event/models/guide';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/observer-guides/new')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: NewObserverGuide,
});

function NewObserverGuide() {
  return <AddTextGuide guidePageType={GuidePageType.Observer} />;
}
