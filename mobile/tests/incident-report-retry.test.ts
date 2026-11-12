/*
  How an incident report save behaves with the real retry rule in TanStack Query: it gives up after
  10 retries when there is a connection but no answer, so the saves queued behind it in the report's
  scope (notes) still run, and it waits (without using up retries) while offline.
*/
import { describe, test } from "node:test";
import assert from "node:assert/strict";
import { AxiosError } from "axios";
import { MutationObserver, QueryClient, onlineManager } from "@tanstack/react-query";
import { retryWhileNoConnection } from "../services/api-errors.ts";

const networkError = () => new AxiosError("Network Error", "ERR_NETWORK", {} as any, {});
const conflict = () =>
  new AxiosError(
    "Conflict",
    "ERR_BAD_REQUEST",
    {} as any,
    {},
    {
      status: 409,
      data: {},
      headers: {},
      statusText: "",
      config: {} as any,
    },
  );

describe("incident report saves", () => {
  test("with no answer, a send stops after 11 attempts instead of retrying forever", async () => {
    const client = new QueryClient();
    let attempts = 0;
    const send = new MutationObserver(client, {
      mutationFn: async () => {
        attempts++;
        throw networkError();
      },
      retry: retryWhileNoConnection,
      retryDelay: 0,
      scope: { id: "incident-report:1" },
    });
    await send.mutate().catch(() => {});
    assert.equal(attempts, 11); // the first try and 10 retries
    assert.equal(send.getCurrentResult().status, "error");
  });

  test("a note queued behind a send that failed for good still runs", async () => {
    const client = new QueryClient();
    let noteRan = false;
    const send = new MutationObserver(client, {
      mutationFn: async () => {
        throw networkError();
      },
      retry: retryWhileNoConnection,
      retryDelay: 0,
      scope: { id: "incident-report:2" },
    });
    const note = new MutationObserver(client, {
      mutationFn: async () => {
        noteRan = true;
      },
      scope: { id: "incident-report:2" },
    });
    await Promise.all([send.mutate().catch(() => {}), note.mutate()]);
    assert.equal(noteRan, true);
  });

  test("an error the server answered (e.g. a locked report) is not retried", async () => {
    const client = new QueryClient();
    let attempts = 0;
    const save = new MutationObserver(client, {
      mutationFn: async () => {
        attempts++;
        throw conflict();
      },
      retry: retryWhileNoConnection,
      retryDelay: 0,
    });
    await save.mutate().catch(() => {});
    assert.equal(attempts, 1);
  });

  test("offline, a send waits without using up retries and goes through once back online", async () => {
    const client = new QueryClient();
    client.mount();
    let attempts = 0;
    onlineManager.setOnline(false);
    try {
      const send = new MutationObserver(client, {
        mutationFn: async () => {
          attempts++;
          return "sent";
        },
        retry: retryWhileNoConnection,
        retryDelay: 0,
      });
      const result = send.mutate();
      await new Promise((resolve) => setTimeout(resolve, 50));
      assert.equal(attempts, 0);
      assert.equal(send.getCurrentResult().isPaused, true);

      onlineManager.setOnline(true);
      assert.equal(await result, "sent");
      assert.equal(attempts, 1);
    } finally {
      onlineManager.setOnline(true);
      client.unmount();
    }
  });
});
