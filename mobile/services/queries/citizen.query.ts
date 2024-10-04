import { useQuery } from "@tanstack/react-query";
import { getCitizenElectionEvents } from "../api/citizen/get-citizen-election-rounds";
import { getCitizenReportingForms } from "../api/citizen/get-citizen-reporting-forms";

// Gets election rounds which can be monitored by citizens
export const useGetCitizenElectionEvents = () => {
  return useQuery({
    queryKey: ["citizen-election-rounds"],
    queryFn: getCitizenElectionEvents,
  });
};

// Gets all published citizen reporting forms for a given election round
export const useGetCitizenReportingForms = (electionRoundId: string) => {
  return useQuery({
    queryKey: ["citizen-reporting-forms", electionRoundId],
    queryFn: () => getCitizenReportingForms(electionRoundId),
    enabled: !!electionRoundId,
  });
};
