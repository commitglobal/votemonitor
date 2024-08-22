import ReactDOM from 'react-dom/client';
import React, { useContext } from 'react';
import './styles/tailwind.css';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { createRouter, ErrorComponent, RouterProvider } from '@tanstack/react-router';
import { routeTree } from './routeTree.gen.ts';
import { I18nextProvider } from 'react-i18next';
import i18n from './i18n';
import AuthContextProvider, { AuthContext } from './context/auth.context';
import { AlertDialogProvider } from './components/ui/alert-dialog-provider.tsx';

export const queryClient = new QueryClient();

const router = createRouter({
  routeTree,
  defaultErrorComponent: ({ error }) => <ErrorComponent error={error} />,
  context: {
    queryClient,
    authContext: AuthContext as any,
  },
  defaultPreload: 'intent',
  // Since we're using React Query, we don't want loader calls to ever be stale
  // This will ensure that the loader is always called when the route is preloaded or visited
  defaultPreloadStaleTime: 0,
});

declare module '@tanstack/react-router' {
  interface Register {
    router: typeof router;
  }
}

function App() {
  const authContext = useContext(AuthContext);
  return (
    <>
      <I18nextProvider i18n={i18n}>
        <RouterProvider
          router={router}
          context={{
            authContext,
          }}
        />
      </I18nextProvider>
    </>
  );
}

const rootElement = document.querySelector('#root') as Element;
if (!rootElement.innerHTML) {
  const root = ReactDOM.createRoot(rootElement);
  root.render(
    <React.StrictMode>
      <QueryClientProvider client={queryClient}>
        <AuthContextProvider>
          <AlertDialogProvider>
            <App />
          </AlertDialogProvider>
        </AuthContextProvider>
      </QueryClientProvider>
    </React.StrictMode>
  );
}
