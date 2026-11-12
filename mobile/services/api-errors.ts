import axios from "axios";

const UUID_REGEX = /[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}/gi;

// Replace ids so the same endpoint is grouped into a single Sentry issue
export const normalizeUrl = (url?: string) =>
  (url ?? "").split("?")[0].replace(UUID_REGEX, "{id}").replace(/^\//, "");

/**
 * Endpoints that answer 404 when there is nothing to fetch (e.g. a round without citizen reporting forms,
 * locations or a polling station information form). Anywhere else a 404 is a real error and is reported.
 */
const EXPECTED_NOT_FOUND = new Set([
  "election-rounds/{id}/citizen-reporting-forms",
  "election-rounds/{id}/locations:version",
  "election-rounds/{id}/locations:fetchAll",
  "election-rounds/{id}/polling-stations:version",
  "election-rounds/{id}/polling-stations:fetchAll",
  "election-rounds/{id}/polling-station-information-form",
]);

// the request got no response: no connectivity, an unreachable server or a timeout
export const isNetworkError = (error: unknown) =>
  axios.isAxiosError(error) && !error.response && !axios.isCancel(error);

/**
 * Timeouts and requests without a response (no connectivity) are expected on observers' phones, and
 * the queries retry them; so is a 404 from an endpoint where it means "nothing there" (EXPECTED_NOT_FOUND).
 * Reporting those only buries real errors: Sentry's beforeSend drops them wherever they're captured.
 */
export const isExpectedApiError = (error: unknown) => {
  if (axios.isAxiosError(error)) {
    const status = error.response?.status;
    if (status === 404) return EXPECTED_NOT_FOUND.has(normalizeUrl(error.config?.url));
    return !status && !!error.request;
  }
  // fetch (uploads, push tokens) without connectivity
  return error instanceof TypeError && error.message === "Network request failed";
};

/*
  Incident report saves (the report and its notes) must not be lost when the connection drops: a request
  that got no response is retried with a growing delay. While offline the retries are paused (that does
  not use them up); with a connection but no answer (server down, timeouts) they stop after
  MAX_NETWORK_RETRIES (~3.5 minutes with retryDelay), so the saves queued behind it in the report's scope (notes) are
  not blocked forever. A send that fails for good is shown to the observer (PersistQueryContext).
  Errors the server answered (e.g. a locked report) are not retried.
*/
const MAX_NETWORK_RETRIES = 10;
export const retryWhileNoConnection = (failureCount: number, error: unknown) =>
  isNetworkError(error) && failureCount < MAX_NETWORK_RETRIES;
