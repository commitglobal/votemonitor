import { JWT_CLAIMS, ReportedError } from '@/common/types';
import * as Sentry from '@sentry/react';
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

type SendErrorToSentry = {
  error: ReportedError;
  title: string;
};

export const sendErrorToSentry = ({ error, title }: SendErrorToSentry) => {
  Sentry.captureMessage(title, 'error');
  Sentry.captureException(error);
};
