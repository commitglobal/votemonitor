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
  type ExportedDataDetails,
  type DataExport,
  ExportStatus,
  ExportedDataType,
} from '../models/data-export';
import { buildURLSearchParams } from '@/lib/utils';

type UseFormSubmissionsExportOptions = UseMutationOptions<DataExport, Error, void>;

export function useStartDataExport(
  exportedDataType: ExportedDataType,
  options?: UseFormSubmissionsExportOptions
): UseMutationResult<DataExport, Error, void> {
  return useMutation({
    mutationFn: async () => {
      const electionRoundId = localStorage.getItem('electionRoundId');

      const response = await authApi.post<DataExport>(
        `/election-rounds/${electionRoundId}/exported-data`,
        {
          exportedDataType
        }
      );

      return response.data;
    },
    ...options,
  });
}

type UseFormSubmissionsExportedDataDetailsOptions = Omit<
  UseQueryOptions<ExportedDataDetails>,
  'queryKey'
>;

export function useFormSubmissionsExportedDataDetails(
  exportedDataId: string,
  options?: UseFormSubmissionsExportedDataDetailsOptions
): UseQueryResult<ExportedDataDetails> {
  return useQuery({
    queryKey: ['exported-data-details', exportedDataId],
    queryFn: async () => {
      const electionRoundId = localStorage.getItem('electionRoundId');

      const response = await authApi.get<ExportedDataDetails>(
        `/election-rounds/${electionRoundId}/exported-data/${exportedDataId}:details`,
      );

      return response.data;
    },
    enabled: options?.enabled && Boolean(exportedDataId),
    refetchInterval: ({ state }) => (state.data?.exportStatus === ExportStatus.Started ? 5 * 1000 : undefined),
  });
}