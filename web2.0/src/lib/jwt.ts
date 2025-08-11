import { jwtDecode } from "jwt-decode";

export const JWT_CLAIMS = {
  EMAIL: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
  ROLE: "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
  USER_ID: "user-id",
};

export const decodeToken = (token: string) => {
  try {
    const decodedToken = jwtDecode(token);
    return {
      //@ts-ignore
      email: decodedToken[JWT_CLAIMS.EMAIL],
      //@ts-ignore
      role: decodedToken[JWT_CLAIMS.ROLE],
      //@ts-ignore
      userId: decodedToken[JWT_CLAIMS.USER_ID],
    };
  } catch (error) {
    // Sentry.captureException(error);
    console.error("Error decoding token:", error);
    throw new Error("Error decoding token");
  }
};
