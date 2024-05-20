import { authApi } from '@/common/auth-api';
import {
  type UseMutationResult,
  useMutation,
  type UseMutationOptions,
  useQuery,
  type UseQueryResult,
  type UseQueryOptions,
} from '@tanstack/react-query';
import {
  type FormSubmissionsExportedDataDetails,
  type FormSubmissionsExport,
  ExportStatus,
} from '../models/form-export';
import { buildURLSearchParams } from '@/lib/utils';

type UseFormSubmissionsExportOptions = UseMutationOptions<FormSubmissionsExport, Error, void>;

export function useFormSubmissionsExport(
  options?: UseFormSubmissionsExportOptions
): UseMutationResult<FormSubmissionsExport, Error, void> {
  return useMutation({
    mutationFn: async () => {
      const electionRoundId = localStorage.getItem('electionRoundId');

      const response = await authApi.post<FormSubmissionsExport>(
        `/election-rounds/${electionRoundId}/form-submissions:export`
      );

      return response.data;
    },
    ...options,
  });
}

type UseFormSubmissionsExportedDataDetailsOptions = Omit<
  UseQueryOptions<FormSubmissionsExportedDataDetails>,
  'queryKey'
>;

export function useFormSubmissionsExportedDataDetails(
  exportedDataId: string,
  options?: UseFormSubmissionsExportedDataDetailsOptions
): UseQueryResult<FormSubmissionsExportedDataDetails> {
  const params = buildURLSearchParams({ exportedDataId });

  return useQuery({
    queryKey: ['form-submissions-exported-data-details'],
    queryFn: async () => {
      const electionRoundId = localStorage.getItem('electionRoundId');

      const response = await authApi.get<FormSubmissionsExportedDataDetails>(
        `/election-rounds/${electionRoundId}/form-submissions:getExportedDataDetails`,
        { params }
      );

      return response.data;
    },
    enabled: options?.enabled && Boolean(exportedDataId),
    refetchInterval: ({ state }) => (state.data?.exportStatus === ExportStatus.Started ? 5 * 1000 : undefined),
  });
}

type UseFormSubmissionsExportedDataOptions = Omit<UseQueryOptions<unknown>, 'queryKey'>;

export function useFormSubmissionsExportedData(
  exportedDataId: string,
  options?: UseFormSubmissionsExportedDataOptions
): UseQueryResult<unknown> {
  const params = buildURLSearchParams({ exportedDataId });

  return useQuery({
    queryKey: ['form-submissions-exported-data'],
    queryFn: async () => {
      const electionRoundId = localStorage.getItem('electionRoundId');

      const response = await authApi.get<unknown>(
        `/election-rounds/${electionRoundId}/form-submissions:getExportedData`,
        { params }
      );

      return response.data;
    },
    enabled: options?.enabled && Boolean(exportedDataId),
  });
}
