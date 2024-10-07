import { useCallback, useEffect, useState } from 'react';
import { authApi } from '@/common/auth-api';
import type { FunctionComponent } from '@/common/types';
import { CsvFileIcon } from '@/components/icons/CsvFileIcon';
import { Button } from '@/components/ui/button';
import { toast } from '@/components/ui/use-toast';
import { useExportedDataDetails, useStartDataExport } from '../../hooks/data-export';
import { ExportStatus, type ExportedDataType } from '../../models/data-export';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';

interface ExportDataButtonProps {
  exportedDataType: ExportedDataType;
}

export function ExportDataButton({ exportedDataType }: ExportDataButtonProps): FunctionComponent {
  const [exportedDataId, setExportedDataId] = useState('');
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);

  const { mutate: createExportData, isPending: isCreatingExportData } = useStartDataExport(
    {
      electionRoundId: currentElectionRoundId,
      exportedDataType,
    },
    {
      onSuccess: (data) => {
        setExportedDataId(data.exportedDataId);
      },
      onError: () => {
        toast({ title: 'Export failed, please try again later', variant: 'default' });
      },
    }
  );

  const downloadHandler = useCallback(() => {
    createExportData();
  }, [createExportData]);

  const { data: exportedDataDetails, isFetching: isFetchingExportedDataDetails } = useExportedDataDetails(
    { electionRoundId: currentElectionRoundId, exportedDataId },
    {
      enabled: !isCreatingExportData,
    }
  );

  const exportStatus = exportedDataDetails?.exportStatus;

  const isLoading = isCreatingExportData || isFetchingExportedDataDetails || exportStatus === ExportStatus.Started;

  const downloadExportedData = useCallback(async (): Promise<void> => {
    const response = await authApi.get<Blob>(
      `/election-rounds/${currentElectionRoundId}/exported-data/${exportedDataId}`,
      {
        responseType: 'blob',
      }
    );

    const exportedData = response.data;
    const blob = new Blob([exportedData], {
      type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;',
    });

    const url = window.URL.createObjectURL(blob);

    const a = document.createElement('a');
    a.style.display = 'none';
    a.href = url;
    a.download = 'exported-data.xlsx';

    document.body.append(a);
    a.click();

    window.URL.revokeObjectURL(url);
  }, [exportedDataId]);

  useEffect(() => {
    if (exportStatus === ExportStatus.Failed) {
      toast({ title: 'Export failed, please try again later', variant: 'default' });
    }

    if (exportStatus === ExportStatus.Completed) {
      void downloadExportedData();
    }
  }, [downloadExportedData, exportStatus]);

  return (
    <Button
      disabled={isLoading}
      className='flex gap-2 text-purple-900 bg-background hover:bg-purple-50 hover:text-purple-500'
      variant='outline'
      onClick={downloadHandler}>
      <CsvFileIcon />
      {isLoading ? 'Please wait...' : 'Export data'}
    </Button>
  );
}
