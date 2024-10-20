import { Buffer } from "buffer";
import * as Sentry from "@sentry/react-native";
export const decodeJwt = (token: string) => {
  const parts = token
    .split(".")
    .map((part) => Buffer.from(part.replace(/-/g, "+").replace(/_/g, "/"), "base64").toString());
  const payload = JSON.parse(parts[1]);
  return payload;
};

export const JWT_CLAIMS = {
  EMAIL: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
  ROLE: "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
  USER_ID: "user-id",
};

export const decodeAndSetUserInSentry = (token: string) => {
  try {
    const decodedToken = decodeJwt(token);
    Sentry.setUser({
      email: decodedToken[JWT_CLAIMS.EMAIL],
      role: decodedToken[JWT_CLAIMS.ROLE],
      userId: decodedToken[JWT_CLAIMS.USER_ID],
    });
  } catch (error) {
    Sentry.captureException(error);
    console.error("Error decoding token:", error);
  }
};
