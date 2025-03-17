import * as Sentry from '@sentry/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { createRouter, ErrorComponent, RouterProvider } from '@tanstack/react-router';
import React, { useContext } from 'react';
import ReactDOM from 'react-dom/client';
import { I18nextProvider } from 'react-i18next';
import { AlertDialogProvider } from './components/ui/alert-dialog-provider.tsx';
import { TanStackReactQueryDevelopmentTools } from './components/utils/development-tools/TanStackReactQueryDevelopmentTools.tsx';
import { TanStackRouterDevelopmentTools } from './components/utils/development-tools/TanStackRouterDevelopmentTools.tsx';
import AuthContextProvider, { AuthContext } from './context/auth.context';
import { CurrentElectionRoundContext, CurrentElectionRoundStoreProvider } from './context/election-round.store.tsx';
import i18n from './i18n';
import { routeTree } from './routeTree.gen.ts';
import './styles/tailwind.css';

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000,
    },
  },
});

const router = createRouter({
  routeTree,
  defaultErrorComponent: ({ error }) => <ErrorComponent error={error} />,
  context: {
    queryClient,
    authContext: AuthContext as any,
    currentElectionRoundContext: CurrentElectionRoundContext as any,
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

Sentry.init({
  dsn: import.meta.env['VITE_SENTRY_DSN'],
  debug: import.meta.env.DEV,
  environment: import.meta.env.MODE,
  tracesSampleRate: import.meta.env.PROD ? 0.2 : 0,
  enabled: !import.meta.env.PROD,
  tracePropagationTargets: ['localhost'],
  integrations: [Sentry.browserTracingIntegration(), Sentry.tanstackRouterBrowserTracingIntegration(router)],
  normalizeDepth: 5,
});

function App() {
  const authContext = useContext(AuthContext);
  const currentElectionRoundContext = useContext(CurrentElectionRoundContext);

  return (
    <>
      <I18nextProvider i18n={i18n}>
        <RouterProvider
          router={router}
          context={{
            authContext,
            currentElectionRoundContext,
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
        <TanStackReactQueryDevelopmentTools />
        <TanStackRouterDevelopmentTools position='bottom-left' router={router} />
        <AuthContextProvider>
          <CurrentElectionRoundStoreProvider>
            <AlertDialogProvider>
              <App />
            </AlertDialogProvider>
          </CurrentElectionRoundStoreProvider>
        </AuthContextProvider>
      </QueryClientProvider>
    </React.StrictMode>
  );
}
