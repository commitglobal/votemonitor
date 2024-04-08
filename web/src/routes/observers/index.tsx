import ObserversDashboard from '@/features/observers/components/Dashboard/Dashboard';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/observers/')({
  component: Observers,
});

function Observers() {
  return (
    <div className='p-2'>
      <ObserversDashboard />
    </div>
  );
}
