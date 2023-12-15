import type { Preview } from "@storybook/react";
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import React from 'react';

import { withThemeByClassName } from "@storybook/addon-styling";

import "../src/styles/tailwind.css";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
    },
  },
});

const preview: Preview = {
  parameters: {
    actions: { argTypesRegex: "^on[A-Z].*" },
    controls: {
      matchers: {
        color: /(background|color)$/i,
        date: /Date$/,
      },
    },
  },

  decorators: [
    // Adds theme switching support.
    // NOTE: requires setting "darkMode" to "class" in your tailwind config
    withThemeByClassName({
      themes: {
        light: "light",
        dark: "dark",
      },
      defaultTheme: "light",
    }),
    Story => React.createElement(
      QueryClientProvider,
      { client: queryClient },
      React.createElement(Story)
    )
  ],
};

export default preview;
