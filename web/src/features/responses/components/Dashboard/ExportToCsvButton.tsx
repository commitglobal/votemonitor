import { useCallback, useEffect, useState } from 'react';
import { CsvFileIcon } from '@/components/icons/CsvFileIcon';
import { Button } from '@/components/ui/button';
import {
  useFormSubmissionsExport,
  useFormSubmissionsExportedData,
  useFormSubmissionsExportedDataDetails,
} from '../../hooks/form-export';
import type { FunctionComponent } from '@/common/types';
import { ExportStatus } from '../../models/form-export';
import { toast } from '@/components/ui/use-toast';

export function ExportToCsvButton(): FunctionComponent {
  const [exportedDataId, setExportedDataId] = useState('');

  const { mutate: createExportData, isPending: isCreatingExportData } = useFormSubmissionsExport({
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

  const { data: exportData } = useFormSubmissionsExportedData(exportedDataId, {
    enabled: exportStatus === ExportStatus.Completed,
  });

  const isLoading = isCreatingExportData || isFetchingExportedDataDetails || exportStatus === ExportStatus.Started;

  useEffect(() => {
    if (exportStatus === ExportStatus.Failed) {
      toast({ title: 'Export failed, please try again later', variant: 'default' });
    }
  }, [exportStatus]);

  console.log(exportData);

  return (
    <Button
      disabled={isLoading}
      className='bg-background hover:bg-purple-50 hover:text-purple-500 text-purple-900 flex gap-2'
      variant='outline'
      onClick={downloadHandler}>
      <CsvFileIcon />
      {isLoading ? 'Please wait...' : 'Export to csv'}
    </Button>
  );
}
