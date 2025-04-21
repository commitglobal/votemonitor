import { TailwindIndicator } from "@/components/TailwindIndicator.tsx";
import { ThemeProvider } from "@/components/ThemeProvider.tsx";
import { Toaster } from "@/components/ui/sonner";
import { RouterProvider, createRouter } from "@tanstack/react-router";
import { StrictMode } from "react";
import ReactDOM from "react-dom/client";
// Import the generated route tree
import { routeTree } from "./routeTree.gen";

import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { TanStackQueryDevelopmentTools } from "./components/utils/development-tools/TanStackQueryDevelopmentTools.tsx";
import { TanStackRouterDevelopmentTools } from "./components/utils/development-tools/TanStackRouterDevelopmentTools.tsx";
import "./styles.css";

import { TooltipProvider } from "./components/ui/tooltip.tsx";
import { AuthProvider, useAuth } from "./contexts/auth.context.tsx";

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: Infinity,
    },
  },
});
// Create a new router instance
export const router = createRouter({
  routeTree,
  context: {
    queryClient,
    auth: undefined!, // This will be set after we wrap the app in an AuthProvider
  },
  defaultPreload: "intent",
  scrollRestoration: true,
  defaultStructuralSharing: true,
  defaultPreloadStaleTime: 0,
});

// Register the router instance for type safety
declare module "@tanstack/react-router" {
  interface Register {
    router: typeof router;
  }
}
function InnerApp() {
  const auth = useAuth();
  return (
    <ThemeProvider
      attribute="class"
      defaultTheme="system"
      enableSystem
      disableTransitionOnChange
      enableColorScheme
    >
      <TooltipProvider>
        <div vaul-drawer-wrapper="">
          <div className="relative flex min-h-screen flex-col bg-background">
            <RouterProvider router={router} context={{ auth, queryClient }} />
          </div>
        </div>
        <TailwindIndicator />
        <Toaster />
        <TanStackRouterDevelopmentTools
          router={router}
          position="bottom-left"
        />
        <TanStackQueryDevelopmentTools client={queryClient} position="right" />
      </TooltipProvider>
    </ThemeProvider>
  );
}
function App() {
  return (
    <StrictMode>
      <QueryClientProvider client={queryClient}>
        <AuthProvider>
          <InnerApp />
        </AuthProvider>
      </QueryClientProvider>
    </StrictMode>
  );
}

// Render the app
const rootElement = document.getElementById("app");
if (rootElement && !rootElement.innerHTML) {
  const root = ReactDOM.createRoot(rootElement);
  root.render(<App />);
}

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
// reportWebVitals(console.log);
