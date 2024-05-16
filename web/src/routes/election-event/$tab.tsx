import ElectionEventDashboard from '@/features/election-event/components/Dashboard/Dashboard';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/election-event/$tab')({
  component: ElectionEventDashboard,
});
