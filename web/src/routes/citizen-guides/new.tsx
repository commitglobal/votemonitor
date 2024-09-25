import AddTextGuide from '@/features/election-event/components/CitizenGuides/AddTextGuide';
import { GuidePageType } from '@/features/election-event/models/guide';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/citizen-guides/new')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: NewCitizenGuide,
});

function NewCitizenGuide() {
  return (
    <div className='p-2'>
      <AddTextGuide guidePageType={GuidePageType.Citizen} />
    </div>
  );
}
