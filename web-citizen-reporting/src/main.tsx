import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import {
  createRouter,
  ErrorComponent,
  RouterProvider,
} from "@tanstack/react-router";
import React from "react";
import { createRoot } from "react-dom/client";
import { I18nextProvider } from "react-i18next";
import i18n from "./i18n";
import { routeTree } from "./routeTree.gen";
import "./styles/tailwind.css";

export const queryClient = new QueryClient();

const DEFAULT_LANGUAGE = import.meta.env["VITE_DEFAULT_LANGUAGE"];

const router = createRouter({
  routeTree,
  defaultErrorComponent: ({ error }) => <ErrorComponent error={error} />,
  context: {
    queryClient,
    DEFAULT_LANGUAGE
  },
  defaultPreload: "intent",
  // Since we're using React Query, we don't want loader calls to ever be stale
  // This will ensure that the loader is always called when the route is preloaded or visited
  defaultPreloadStaleTime: 0,
});

function App() {
  return (
    <>
      <I18nextProvider i18n={i18n}>
        <RouterProvider router={router} />
      </I18nextProvider>
    </>
  );
}

const rootElement = document.querySelector("#root") as Element;
if (!rootElement.innerHTML) {
  const root = createRoot(rootElement);
  root.render(
    <React.StrictMode>
      <QueryClientProvider client={queryClient}>
        <App />
      </QueryClientProvider>
    </React.StrictMode>
  );
}
