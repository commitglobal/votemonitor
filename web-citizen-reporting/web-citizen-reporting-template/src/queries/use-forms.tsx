import { getFormById, getForms } from "@/api/get-forms";
import type { FormModel } from "@/common/types";
import { queryClient } from "@/main";
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

export const formQueryOptions = (id: string) => {
  return queryOptions({
    queryKey: ["forms", id],
    queryFn: () => getFormById(id),
    staleTime: STALE_TIME,
    initialData: () => {
      const formData = (
        queryClient.getQueryData(["forms"]) as FormModel[]
      )?.find((form) => form.id === id);

      return formData;
    },
  });
};

export const useFormById = (id: string) => {
  return useQuery(formQueryOptions(id));
};
