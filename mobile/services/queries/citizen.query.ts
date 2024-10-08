import { skipToken, useQuery } from "@tanstack/react-query";
import { getCitizenElectionEvents } from "../api/citizen/get-citizen-election-rounds";
import { getCitizenReportingForms } from "../api/citizen/get-citizen-reporting-forms";
import { getCitizenElectionRoundLocations } from "../api/citizen/get-election-round-locations";
import * as CitizenLocationsDB from "../../database/DAO/CitizenLocationsDAO";
import * as Sentry from "@sentry/react-native";

export const citizenQueryKeys = {
  all: ["citizen"] as const,
  electionRounds: () => [...citizenQueryKeys.all, "election-rounds"] as const,
  reportingForms: (electionRoundId: string) =>
    [...citizenQueryKeys.all, "reporting-forms", electionRoundId] as const,
  locations: (electionRoundId: string) =>
    [...citizenQueryKeys.all, "locations", electionRoundId] as const,
};

// Gets election rounds which can be monitored by citizens
export const useGetCitizenElectionEvents = () => {
  return useQuery({
    queryKey: citizenQueryKeys.electionRounds(),
    queryFn: getCitizenElectionEvents,
  });
};

// Gets all published citizen reporting forms for a given election round
export const useGetCitizenReportingForms = (electionRoundId: string) => {
  return useQuery({
    queryKey: citizenQueryKeys.reportingForms(electionRoundId),
    queryFn: electionRoundId ? () => getCitizenReportingForms(electionRoundId) : skipToken,
  });
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
