import ObserversDashboard from '@/features/observers/components/Dashboard/Dashboard';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/observers/')({
  beforeLoad: ({ context }) => {
    redirectIfNotAuth(context.authContext.isAuthenticated);
  },
  component: Observers,
});

function Observers() {
  return (
    <div className='p-2'>
      <ObserversDashboard />
    </div>
  );
}
