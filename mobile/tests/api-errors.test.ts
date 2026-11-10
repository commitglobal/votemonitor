import { describe, test } from "node:test";
import assert from "node:assert/strict";
import { AxiosError } from "axios";
import {
  isExpectedApiError,
  isNetworkError,
  retryWhileNoConnection,
} from "../services/api-errors.ts";

const ID = "7f1c2a3b-0000-4000-8000-000000000001";

const withStatus = (url: string, status: number) =>
  new AxiosError(
    "failed",
    "ERR_BAD_REQUEST",
    { url, headers: {} } as any,
    {},
    {
      status,
      data: {},
      statusText: "",
      headers: {},
      config: { url, headers: {} } as any,
    },
  );

const noResponse = (url: string, code = "ERR_NETWORK") =>
  new AxiosError("Network Error", code, { url, headers: {} } as any, {});

describe("isExpectedApiError (not reported to Sentry)", () => {
  test("404 only from endpoints where it means there is nothing to fetch", () => {
    for (const url of [
      `/election-rounds/${ID}/citizen-reporting-forms`,
      `/election-rounds/${ID}/locations:version`,
      `/election-rounds/${ID}/locations:fetchAll`,
      `election-rounds/${ID}/polling-stations:version`,
      `election-rounds/${ID}/polling-stations:fetchAll?x=1`,
      `election-rounds/${ID}/polling-station-information-form`,
    ]) {
      assert.equal(isExpectedApiError(withStatus(url, 404)), true, url);
    }
  });

  test("a 404 anywhere else is reported (e.g. an endpoint broken by a deploy)", () => {
    for (const url of [
      `election-rounds/${ID}/incident-reports`,
      `election-rounds/${ID}/incident-reports:my`,
      `election-rounds/${ID}/form-submissions:my`,
      `election-rounds/${ID}/quick-reports:my`,
    ]) {
      assert.equal(isExpectedApiError(withStatus(url, 404)), false, url);
    }
  });

  test("other statuses are reported, requests without a response are not", () => {
    const url = `election-rounds/${ID}/incident-reports`;
    assert.equal(isExpectedApiError(withStatus(url, 500)), false);
    assert.equal(isExpectedApiError(withStatus(url, 409)), false);
    assert.equal(isExpectedApiError(noResponse(url)), true);
    assert.equal(isExpectedApiError(noResponse(url, "ECONNABORTED")), true);
    assert.equal(isExpectedApiError(new TypeError("Network request failed")), true);
    assert.equal(isExpectedApiError(new Error("boom")), false);
  });
});

describe("isNetworkError", () => {
  test("only requests that got no response", () => {
    const url = `election-rounds/${ID}/incident-reports`;
    assert.equal(isNetworkError(noResponse(url)), true);
    assert.equal(isNetworkError(withStatus(url, 500)), false);
    assert.equal(isNetworkError(new Error("x")), false);
  });
});

describe("retryWhileNoConnection", () => {
  test("retries a request without a response up to 10 times", () => {
    const error = noResponse("x");
    assert.equal(retryWhileNoConnection(0, error), true);
    assert.equal(retryWhileNoConnection(9, error), true);
    assert.equal(retryWhileNoConnection(10, error), false);
  });

  test("never retries an error the server answered", () => {
    assert.equal(retryWhileNoConnection(0, withStatus("x", 409)), false);
    assert.equal(retryWhileNoConnection(0, withStatus("x", 500)), false);
  });
});
