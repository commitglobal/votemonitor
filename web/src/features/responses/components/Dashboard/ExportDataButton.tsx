import { authApi } from '@/common/auth-api';
import type { FunctionComponent } from '@/common/types';
import { CsvFileIcon } from '@/components/icons/CsvFileIcon';
import { Button } from '@/components/ui/button';
import { toast } from '@/components/ui/use-toast';
import { useCallback, useEffect, useState } from 'react';
import {
  useFormSubmissionsExportedDataDetails,
  useStartDataExport,
} from '../../hooks/form-export';
import { ExportStatus, ExportedDataType } from '../../models/data-export';
export interface ExportDataButtonProps {
  exportedDataType: ExportedDataType
}
export function ExportDataButton({ exportedDataType }: ExportDataButtonProps): FunctionComponent {
  const [exportedDataId, setExportedDataId] = useState('');

  const { mutate: createExportData, isPending: isCreatingExportData } = useStartDataExport(exportedDataType, {

    onSuccess: (data) => {
      setExportedDataId(data.exportedDataId);
    },
    onError: () => {
      toast({ title: 'Export failed, please try again later', variant: 'default' });
    },
  });
  const downloadHandler = useCallback(() => {
    createExportData();
  }, [createExportData]);

  const { data: exportedDataDetails, isFetching: isFetchingExportedDataDetails } =
    useFormSubmissionsExportedDataDetails(exportedDataId, {
      enabled: !isCreatingExportData,
    });

  const exportStatus = exportedDataDetails?.exportStatus;

  const isLoading = isCreatingExportData || isFetchingExportedDataDetails || exportStatus === ExportStatus.Started;

  const downloadExportedData = async () => {
    const electionRoundId = localStorage.getItem('electionRoundId');

    const response = await authApi.get(
      `/election-rounds/${electionRoundId}/exported-data/${exportedDataId}`,
      { responseType: "blob" }
    );

    const exportedData = response.data;
    const blob = new Blob([exportedData], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;' });

    const url = window.URL.createObjectURL(blob);

    const a = document.createElement('a');
    a.style.display = 'none';
    a.href = url;
    a.download = 'exported-data.xlsx';

    document.body.appendChild(a);
    a.click();

    window.URL.revokeObjectURL(url);
  };

  useEffect(() => {
    if (exportStatus === ExportStatus.Failed) {
      toast({ title: 'Export failed, please try again later', variant: 'default' });
    }

    if (exportStatus === ExportStatus.Completed) {
      downloadExportedData();
    }

  }, [exportStatus]);

  return (
    <Button
      disabled={isLoading}
      className='bg-background hover:bg-purple-50 hover:text-purple-500 text-purple-900 flex gap-2'
      variant='outline'
      onClick={downloadHandler}>
      <CsvFileIcon />
      {isLoading ? 'Please wait...' : 'Export data'}
    </Button>
  );
}
