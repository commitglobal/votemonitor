import { useQuery } from "@tanstack/react-query";
import { getCitizenElectionEvents } from "../api/citizen/get-citizen-election-rounds";

export const useGetCitizenElectionEvents = () => {
  return useQuery({
    queryKey: ["citizen-election-rounds"],
    queryFn: getCitizenElectionEvents,
  });
};
