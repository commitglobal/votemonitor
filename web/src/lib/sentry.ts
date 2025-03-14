import { JWT_CLAIMS } from '@/common/types';
import * as Sentry from '@sentry/react';
import { parseJwt } from './utils';

export const SENTRY_INIT_OPTIONS: Sentry.BrowserOptions = {
  dsn: import.meta.env['VITE_SENTRY_DSN'],
  debug: import.meta.env.DEV,
  environment: import.meta.env.MODE,
  tracesSampleRate: import.meta.env.PROD ? 0.2 : 0,
  enabled: !import.meta.env.PROD,
  tracePropagationTargets: ['localhost'],
  integrations: [],
};

export const parseAndSetUserInSentry = (token: string) => {
  try {
    const decodedToken = parseJwt(token);

    Sentry.setUser({
      email: decodedToken[JWT_CLAIMS.EMAIL],
      role: decodedToken[JWT_CLAIMS.USER_ROLE],
      userId: decodedToken[JWT_CLAIMS.USER_ID],
    });
  } catch (error) {
    Sentry.captureException(error);
    console.error('Error decoding token:', error);
  }
};
