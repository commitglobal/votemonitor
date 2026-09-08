import { isProduction } from "@/lib/utils";
import React from "react";

export const TanStackRouterDevelopmentTools = isProduction
  ? (): null => null
  : React.lazy(() =>
      import("@tanstack/react-router-devtools").then((result) => ({
        default: result.TanStackRouterDevtools,
      }))
    );
