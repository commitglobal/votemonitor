import { createFileRoute, Navigate } from '@tanstack/react-router';

export const Route = createFileRoute('/election-event/')({
  component: Component
});

function Component() {
  return <Navigate to="/election-event/$tab" params={{ tab: 'event-details' }} />
}
