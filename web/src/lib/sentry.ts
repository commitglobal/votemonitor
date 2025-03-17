import { JWT_CLAIMS, ProblemDetails } from '@/common/types';
import * as Sentry from '@sentry/react';
import { AxiosError } from 'axios';
import { parseJwt } from './utils';

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

type ReportedError = Error | AxiosError<unknown | ProblemDetails>;

export const sendErrorToSentry = (error: ReportedError, message: string) => {
  Sentry.captureMessage(message, 'error');
  Sentry.captureException(error);
};
