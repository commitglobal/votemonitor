/* SPDX-FileCopyrightText: 2014-present Kriasoft */
/* SPDX-License-Identifier: MIT */

import * as React from "react";
import "vite/client";

interface ImportMetaEnv {
  readonly VITE_APP_ENVIRONMENT: string;
  readonly VITE_API_URL: string;
}

declare module "*.css";

declare module "*.svg" {
  const content: React.FC<React.SVGProps<SVGElement>>;
  export default content;
}
