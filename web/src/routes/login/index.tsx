import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/login/')({
  beforeLoad: ({ context }) => {
    console.log(context.authContext.isAuthenticated);
  },
  component: () => <div>Hello /login/!</div>,
});
