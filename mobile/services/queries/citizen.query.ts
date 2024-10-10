import { skipToken, useQuery } from "@tanstack/react-query";
import { getCitizenElectionRounds } from "../api/citizen/get-citizen-election-rounds";
import { getCitizenReportingForms } from "../api/citizen/get-citizen-reporting-forms";
import { getCitizenElectionRoundLocations } from "../api/citizen/get-election-round-locations";
import * as CitizenLocationsDB from "../../database/DAO/CitizenLocationsDAO";
import * as Sentry from "@sentry/react-native";
import { CitizenLocationVM } from "../../common/models/citizen-locations.model";
import { ElectionRoundsAllFormsAPIResponse } from "../definitions.api";
import { useCallback } from "react";

export const citizenQueryKeys = {
  all: ["citizen"] as const,
  electionRounds: () => [...citizenQueryKeys.all, "election-rounds"] as const,
  reportingForms: (electionRoundId: string) =>
    [...citizenQueryKeys.all, "reporting-forms", electionRoundId] as const,
  locations: (electionRoundId: string) =>
    [...citizenQueryKeys.all, "locations", electionRoundId] as const,
  locationsByParentId: (parentId: number, electionRoundId: string) =>
    [...citizenQueryKeys.all, "locations", electionRoundId, parentId] as const,
};

// Gets election rounds which can be monitored by citizens
export const useGetCitizenElectionRounds = () => {
  return useQuery({
    queryKey: citizenQueryKeys.electionRounds(),
    queryFn: getCitizenElectionRounds,
  });
};

// Gets all published citizen reporting forms for a given election round
export const useGetCitizenReportingForms = <TResult = ElectionRoundsAllFormsAPIResponse>(
  electionRoundId: string,
  select?: (data: ElectionRoundsAllFormsAPIResponse) => TResult,
) => {
  return useQuery({
    queryKey: citizenQueryKeys.reportingForms(electionRoundId),
    queryFn: electionRoundId ? () => getCitizenReportingForms(electionRoundId) : skipToken,
    select,
  });
};

export const useGetCitizenReportingFormById = (electionRoundId: string, formId: string) => {
  return useGetCitizenReportingForms(
    electionRoundId,
    useCallback(
      (data: ElectionRoundsAllFormsAPIResponse) => {
        const selectedForm = data?.forms?.find((form) => form.id === formId);
        return selectedForm;
      },
      [electionRoundId, formId],
    ),
  );
};

export const useGetCitizenLocations = (electionRoundId: string) => {
  return useQuery({
    queryKey: citizenQueryKeys.locations(electionRoundId),
    queryFn: electionRoundId
      ? async () => {
          try {
            // TODO: add cache busting
            const exists = await CitizenLocationsDB.getOne(electionRoundId);

            if (!exists) {
              const data = await getCitizenElectionRoundLocations(electionRoundId);
              await CitizenLocationsDB.addCitizenLocationsBulk(electionRoundId, data.nodes);

              return `[Citizen Locations] ADDED TO DB`;
            } else {
              return `[Citizen Locations] RETRIEVED FROM DB`;
            }
          } catch (err) {
            console.warn("useGetCitizenReportingForms", err);
            Sentry.captureMessage(
              `Failed to get citizen reporting forms. ElectionRoundId: ${electionRoundId}`,
            );
            Sentry.captureException(err);
            throw err;
          }
        }
      : skipToken,
    retry: 0,
    staleTime: 0,
    networkMode: "always",
  });
};

export const useGetCitizenLocationsByParentId = (parentId: number, electionRoundId: string) => {
  return useQuery({
    queryKey: citizenQueryKeys.locationsByParentId(parentId, electionRoundId),
    queryFn:
      parentId && electionRoundId
        ? async () => {
            const data = await CitizenLocationsDB.getCitizenLocationsByParentId(
              parentId,
              electionRoundId,
            );
            const toReturn: CitizenLocationVM[] = data.map((location) => ({
              id: location._id,
              name: location.name,
              parentId: location.parentId,
              electionRoundId: location.electionRoundId,
              locationId: location.locationId,
            }));
            return toReturn;
          }
        : skipToken,
    initialData: [],
    staleTime: 0,
    networkMode: "always",
  });
};
