import Header from '@/components/layout/Header/Header';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute, Outlet } from '@tanstack/react-router';

export const Route = createFileRoute('/(app)')({
  component: RouteComponent,
  beforeLoad: () => {
    redirectIfNotAuth();
  },
});

function RouteComponent() {
  return (
    <div className='flex flex-col min-h-screen pb-20'>
      <Header />
      <main className='container flex flex-col flex-1'>
        <Outlet />
      </main>
    </div>
  );
}
