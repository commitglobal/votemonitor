import { StrictMode } from 'react'
import ReactDOM from 'react-dom/client'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { RouterProvider, createRouter } from '@tanstack/react-router'
import { AuthProvider, useAuth } from '@/contexts/auth.context'
import '@/styles.css'
// import i18n (needs to be bundled ;))
import countries from 'i18n-iso-countries'
import enCountries from 'i18n-iso-countries/langs/en.json'
import roCountries from 'i18n-iso-countries/langs/ro.json'
import { useTranslation } from 'react-i18next'
import { Toaster } from '@/components/ui/sonner'
import { TooltipProvider } from '@/components/ui/tooltip'
import { TailwindIndicator } from '@/components/TailwindIndicator.tsx'
import { ThemeProvider } from '@/components/ThemeProvider.tsx'
import { TanStackQueryDevelopmentTools } from '@/components/development-tools/TanStackQueryDevelopmentTools'
import { TanStackRouterDevelopmentTools } from '@/components/development-tools/TanStackRouterDevelopmentTools'
import './i18n'
// Import the generated route tree
import { routeTree } from './routeTree.gen'

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      // Data remains fresh for 10 minutes - prevents redundant API calls during
      // typical user sessions while ensuring data updates within reasonable time
      staleTime: 10 * 60 * 1000,
      // Garbage collection after 5 minutes - balances memory usage with instant
      // data availability when navigating back to recently viewed pages
      gcTime: 5 * 60 * 1000,
      // Retry strategy: 3 attempts with exponential backoff (1s, 2s, 4s) capped at 30s
      // Handles transient network issues without overwhelming the server
      retry: 1,
      retryDelay: (attemptIndex) => Math.min(1000 * 2 ** attemptIndex, 30000),
      // Auto-refetch when user returns to tab - ensures displayed data is current
      // after context switches (critical for collaborative features)
      refetchOnWindowFocus: false,
      // Always refetch after network reconnection - prevents stale data after
      // connectivity issues (overrides staleTime check)
      refetchOnReconnect: 'always',
    },
    mutations: {
      // Single retry for mutations - prevents duplicate operations while handling
      // momentary network blips (user can manually retry for persistent failures)
      retry: 1,
      retryDelay: 1000,
      // Global error handler for mutations
      onError: (error) => {
        console.error('Mutation error:', error)
      },
    },
  },
})
// Create a new router instance
export const router = createRouter({
  routeTree,
  context: {
    queryClient,
    auth: undefined!, // This will be set after we wrap the app in an AuthProvider
  },
  defaultPreload: 'intent',
  scrollRestoration: true,
  defaultStructuralSharing: true,
  defaultPreloadStaleTime: 0,
})

// Register the router instance for type safety
declare module '@tanstack/react-router' {
  interface Register {
    router: typeof router
  }
}

function InnerApp() {
  const auth = useAuth()
  const { i18n } = useTranslation()

  const lang = (i18n.language || 'en').split('-')[0]
  const localeMap: Record<string, unknown> = {
    en: enCountries,
    ro: roCountries,
  }
  countries.registerLocale((localeMap[lang] ?? enCountries) as any)
  if (auth.isLoading) {
    return (
      <div className='flex h-screen items-center justify-center'>
        Loading...
      </div>
    ) // or spinner
  }

  return (
    <ThemeProvider
      attribute='class'
      defaultTheme='system'
      enableSystem
      disableTransitionOnChange
      enableColorScheme
    >
      <div vaul-drawer-wrapper=''>
        <div className='bg-background relative flex min-h-screen flex-col'>
          <RouterProvider router={router} context={{ auth, queryClient }} />
        </div>
      </div>
      <TailwindIndicator />
      <Toaster duration={5000} richColors />
      <TanStackRouterDevelopmentTools router={router} position='bottom-left' />
      <TanStackQueryDevelopmentTools client={queryClient} position='right' />
    </ThemeProvider>
  )
}

// Render the app
const rootElement = document.getElementById('app')
if (rootElement && !rootElement.innerHTML) {
  const root = ReactDOM.createRoot(rootElement)
  root.render(
    <StrictMode>
      <QueryClientProvider client={queryClient}>
        <AuthProvider>
          <InnerApp />
        </AuthProvider>
      </QueryClientProvider>
    </StrictMode>
  )
}

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
// reportWebVitals(console.log);
