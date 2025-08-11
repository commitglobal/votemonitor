/* SPDX-FileCopyrightText: 2014-present Kriasoft */
/* SPDX-License-Identifier: MIT */

import { tanstackRouter } from "@tanstack/router-plugin/vite";
import react from "@vitejs/plugin-react-swc";
import { resolve } from "node:path";
import { defineProject } from "vitest/config";
import tailwindcss from "@tailwindcss/vite";

/**
 * Vite configuration.
 * https://vitejs.dev/config/
 */
export default defineProject(() => {
  return {
    build: {
      rollupOptions: {
        output: {
          manualChunks: {
            react: ["react", "react-dom"],
            tanstack: ["@tanstack/react-router"],
            ui: [
              "@radix-ui/react-slot",
              "class-variance-authority",
              "clsx",
              "tailwind-merge",
            ],
          },
        },
      },
    },

    resolve: {
      conditions: ["module", "browser", "development|production"],
      alias: {
        "@": resolve(__dirname, "./src"),
      },
    },

    plugins: [
      tanstackRouter({
        routesDirectory: "./src/routes",
        generatedRouteTree: "./src/routeTree.gen.ts",
        routeFileIgnorePrefix: "-",
        quoteStyle: "single",
        semicolons: false,
        autoCodeSplitting: true,
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
      }) as any,
      // https://github.com/vitejs/vite-plugin-react/tree/main/packages/plugin-react-swc
      react(),
      tailwindcss(),
    ],
  };
});
