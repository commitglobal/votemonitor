import type { PersistedClient } from "@tanstack/react-query-persist-client";

/*
  What is written to storage, without the errors of failed requests:
  - a query whose last refetch failed but that still has its data (kept so the app starts offline with
    data) is stored as a success, so after a restart screens show the data and not an error
  - a paused mutation keeps its payload but not the error of its last attempt
  The errors are AxiosErrors: serialized, they carry the request config, auth headers included.
*/
export const withoutRequestErrors = (client: PersistedClient): PersistedClient => ({
  ...client,
  clientState: {
    ...client.clientState,
    queries: client.clientState.queries.map((query) =>
      query.state.status === "error" && query.state.data !== undefined
        ? {
            ...query,
            state: {
              ...query.state,
              status: "success",
              error: null,
              errorUpdateCount: 0,
              fetchFailureCount: 0,
              fetchFailureReason: null,
            },
          }
        : query,
    ),
    mutations: client.clientState.mutations.map((mutation) => ({
      ...mutation,
      state: { ...mutation.state, error: null, failureReason: null },
    })),
  },
});
