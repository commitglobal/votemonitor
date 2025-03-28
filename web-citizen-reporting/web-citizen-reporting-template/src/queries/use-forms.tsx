import { getForms } from "@/api/get-forms";
import { queryOptions, useQuery } from "@tanstack/react-query";
const STALE_TIME = 1000 * 60 * 15; // 15 minutes

export const formsOptions = () => {
  return queryOptions({
    queryKey: ["forms"],
    placeholderData: [],
    queryFn: () => getForms(),
    staleTime: STALE_TIME,
  });
};

export const useForms = () => {
  return useQuery(formsOptions());
};
