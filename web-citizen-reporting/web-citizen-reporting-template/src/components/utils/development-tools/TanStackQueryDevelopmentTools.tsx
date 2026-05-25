import { isProduction } from "@/lib/utils";
import React from "react";

export const TanStackQueryDevelopmentTools = isProduction
  ? (): null => null
  : React.lazy(() =>
      import("@tanstack/react-query-devtools").then((result) => ({
        default: result.ReactQueryDevtools,
      }))
    );
