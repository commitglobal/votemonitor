import { describe, test } from "node:test";
import assert from "node:assert/strict";
import { AxiosError } from "axios";
import {
  QueryClient,
  defaultShouldDehydrateQuery,
  dehydrate,
  hydrate,
  type Query,
} from "@tanstack/react-query";
import { withoutRequestErrors } from "../contexts/persist-query/persisted-client.ts";

const networkError = () =>
  new AxiosError(
    "Network Error",
    "ERR_NETWORK",
    {
      url: "election-rounds/1/incident-reports:my",
      headers: { Authorization: "Bearer SECRET-TOKEN" },
    } as any,
    {},
  );

// the same rule as PersistQueryContext's shouldDehydrateQuery
const shouldDehydrateQuery = (query: Query) =>
  defaultShouldDehydrateQuery(query) ||
  (query.state.status === "error" && query.state.data !== undefined);

describe("withoutRequestErrors (what is written to storage)", () => {
  test("a query whose refetch failed is kept with its data, as a success, without the error", async () => {
    const client = new QueryClient();
    const key = ["incident-reports", "1"];
    await client.fetchQuery({ queryKey: key, queryFn: async () => [{ id: "a" }] });
    await client
      .fetchQuery({
        queryKey: key,
        queryFn: async () => {
          throw networkError();
        },
        retry: false,
        staleTime: 0,
      })
      .catch(() => {});
    assert.equal(client.getQueryState(key)?.status, "error");

    const persisted = {
      timestamp: Date.now(),
      buster: "",
      clientState: dehydrate(client, { shouldDehydrateQuery }),
    };

    // without it, the AxiosError would be written with its request config, auth header included,
    // and the query would come back after a restart as an error
    const raw = JSON.stringify(persisted);
    assert.match(raw, /SECRET-TOKEN/);
    const restoredRaw = new QueryClient();
    hydrate(restoredRaw, JSON.parse(raw).clientState);
    assert.equal(restoredRaw.getQueryState(key)?.status, "error");

    const stored = JSON.stringify(withoutRequestErrors(persisted));
    assert.doesNotMatch(stored, /SECRET-TOKEN|Network Error/);
    const restored = new QueryClient();
    hydrate(restored, JSON.parse(stored).clientState);
    assert.equal(restored.getQueryState(key)?.status, "success");
    assert.equal(restored.getQueryState(key)?.error, null);
    assert.deepEqual(restored.getQueryData(key), [{ id: "a" }]);
  });

  test("successful queries and failed ones without data are left as they are", () => {
    const ok = {
      queryKey: ["a"],
      queryHash: "a",
      state: { status: "success", data: 1, error: null },
    };
    const failedNoData = {
      queryKey: ["b"],
      queryHash: "b",
      state: { status: "error", data: undefined, error: {} },
    };
    const out = withoutRequestErrors({
      timestamp: 0,
      buster: "",
      clientState: { queries: [ok, failedNoData], mutations: [] },
    } as any);
    assert.deepEqual(out.clientState.queries, [ok, failedNoData]);
  });

  test("a paused mutation keeps its payload but not the error of its last attempt", () => {
    const out = withoutRequestErrors({
      timestamp: 0,
      buster: "",
      clientState: {
        queries: [],
        mutations: [
          {
            mutationKey: ["incident-reports", "add"],
            state: {
              variables: { id: "r1" },
              isPaused: true,
              error: null,
              failureReason: networkError(),
            },
          },
        ],
      },
    } as any);
    const [mutation] = out.clientState.mutations;
    assert.deepEqual(mutation.state.variables, { id: "r1" });
    assert.equal(mutation.state.failureReason, null);
    assert.doesNotMatch(JSON.stringify(out), /SECRET-TOKEN/);
  });
});
