import { FunctionComponent, importPollingStationSchema } from '@/common/types';
import Layout from '@/components/layout/Layout';
import { Badge } from '@/components/ui/badge';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { FileUploader } from '@/components/ui/file-uploader';
import { Separator } from '@/components/ui/separator';
import Papa from 'papaparse';
import { useCallback, useMemo, useState } from 'react';
import { z, ZodIssue } from 'zod';

import { authApi } from '@/common/auth-api';
import { Button } from '@/components/ui/button';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { pollingStationsKeys } from '@/hooks/polling-stations-levels';
import { downloadImportExample, TemplateType } from '@/lib/utils';
import { queryClient } from '@/main';
import { ArrowDownTrayIcon, CheckCircleIcon, DocumentTextIcon, ExclamationCircleIcon } from '@heroicons/react/24/outline';
import { useMutation } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { LoaderIcon, Upload } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import { toast } from 'sonner';
import { ImportedPollingStationsDataTable } from './ImportedPollingStationsDataTable';

export type ImportPollingStationRow = z.infer<typeof importPollingStationSchema> & { errors?: ZodIssue[] };

export function PollingStationsImport(): FunctionComponent {
  const [pollingStations, setPollingStations] = useState<ImportPollingStationRow[]>([]);
  const { t } = useTranslation('translation', { keyPrefix: 'electionEvent.pollingStations.addPollingStation' });
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const navigate = useNavigate();

  function deletePollingStation(pollingStation: ImportPollingStationRow) {
    setPollingStations((prev) => [...prev.filter((obs) => obs.id !== pollingStation.id)]);
  }

  function updatePollingStation(pollingStation: ImportPollingStationRow) {
    const validationResult = importPollingStationSchema.safeParse(pollingStation);

    const pollingStationWithErorrs = {
      ...pollingStation,
      errors: validationResult.success ? [] : validationResult.error.errors,
    };

    setPollingStations((prevData) =>
      prevData.map((o) => (o.id === pollingStation.id ? { ...o, ...pollingStationWithErorrs } : o))
    );
  }

  const hasInvalidPollingStations = useMemo(() => {
    return pollingStations.some((pollingStation) => pollingStation.errors && pollingStation.errors?.length > 0);
  }, [pollingStations]);

  const validCount = useMemo(() => {
    return pollingStations.filter((ps) => !ps.errors || ps.errors.length === 0).length;
  }, [pollingStations]);

  const errorCount = useMemo(() => {
    return pollingStations.filter((ps) => ps.errors && ps.errors.length > 0).length;
  }, [pollingStations]);

  const { mutate, isPending } = useMutation({
    mutationFn: ({
      electionRoundId,
      pollingStations,
    }: {
      electionRoundId: string;
      pollingStations: ImportPollingStationRow[];
    }) => {
      return authApi.post(`/election-rounds/${electionRoundId}/polling-stations`, { pollingStations });
    },

    onSuccess: (_, { electionRoundId }) => {
      toast(t('onSuccess'));
      queryClient.invalidateQueries({ queryKey: pollingStationsKeys.all(electionRoundId) });
      navigate({ to: '/election-rounds/$electionRoundId', params: { electionRoundId } });
    },
    onError: () => {
      toast.error(t('onError'),{
        description: 'Please contact tech support',
      });
    },
  });

  const handleImportPollingStations = useCallback(
    () => mutate({ electionRoundId: currentElectionRoundId, pollingStations }),
    [currentElectionRoundId, pollingStations]
  );

  return (
    <Layout enableBreadcrumbs={true} title='Import polling stations list'>
      <div className='flex flex-col gap-6'>
        {/* Template Download Section */}
        <Card>
          <CardHeader>
            <CardTitle className='flex items-center gap-2 text-lg'>
              <DocumentTextIcon className='h-5 w-5 text-purple-600' />
              Step 1: Download Template
            </CardTitle>
            <CardDescription>
              Download the CSV template and fill it with your polling station data. The template includes columns for
              location levels, address, station number, display order, and GPS coordinates (latitude/longitude).
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Button
              variant='outline'
              onClick={() => downloadImportExample(TemplateType.PollingStations)}
              className='border-purple-200 bg-purple-50 text-purple-700 hover:bg-purple-100 hover:text-purple-800'>
              <ArrowDownTrayIcon className='mr-2 h-4 w-4' />
              Download polling_station_import_template.csv
            </Button>
          </CardContent>
        </Card>

        {/* Upload Section */}
        <Card>
          <CardHeader>
            <CardTitle className='flex items-center gap-2 text-lg'>
              <Upload className='h-5 w-5 text-purple-600' />
              Step 2: Upload Your File
            </CardTitle>
            <CardDescription>
              Upload your completed CSV file. The system will validate the data and highlight any errors that need to be
              corrected before import.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <FileUploader
              accept={{ 'text/csv': [] }}
              multiple={false}
              maxSize={5 * 1024 * 1024}
              maxFileCount={1}
              onValueChange={(files: File[]) => {
                const file = files[0];
                if (!file) {
                  setPollingStations([]);
                } else {
                  Papa.parse<ImportPollingStationRow>(file, {
                    header: true,
                    skipEmptyLines: true,
                    transformHeader: (header) => header.charAt(0).toLowerCase() + header.slice(1),
                    async complete(results) {
                      if (results.errors.length) {
                        toast.error('Parsing errors', {
                          description: 'Please check the file and try again',
                        });
                      }

                      const validatedPollingStations = results.data.map((pollingStation) => {
                        const validationResult = importPollingStationSchema.safeParse(pollingStation);
                        return {
                          ...(validationResult.success ? validationResult.data : pollingStation),
                          id: crypto.randomUUID(),
                          errors: validationResult.success ? [] : validationResult.error.errors,
                        };
                      });

                      setPollingStations(validatedPollingStations);
                    },
                  });
                }
              }}
            />
          </CardContent>
        </Card>

        {/* Review and Import Section */}
        {pollingStations.length > 0 && (
          <Card>
            <CardHeader className='pb-4'>
              <div className='flex items-center justify-between'>
                <div>
                  <CardTitle className='text-lg'>Step 3: Review and Import</CardTitle>
                  <CardDescription className='mt-1'>
                    Review the data below and correct any errors before importing.
                  </CardDescription>
                </div>
                <Button
                  onClick={handleImportPollingStations}
                  disabled={hasInvalidPollingStations || isPending}
                  size='lg'
                  className='bg-purple-600 hover:bg-purple-700'>
                  {isPending && <LoaderIcon className='mr-2 h-4 w-4 animate-spin' />}
                  Import {pollingStations.length} polling stations
                </Button>
              </div>

              {/* Stats Summary */}
              <div className='mt-4 flex items-center gap-4'>
                <Badge variant='outline' className='px-3 py-1 text-sm font-medium'>
                  Total: {pollingStations.length}
                </Badge>
                <Badge
                  variant='outline'
                  className='flex items-center gap-1 border-green-200 bg-green-50 px-3 py-1 text-sm font-medium text-green-700'>
                  <CheckCircleIcon className='h-4 w-4' />
                  Valid: {validCount}
                </Badge>
                {errorCount > 0 && (
                  <Badge
                    variant='outline'
                    className='flex items-center gap-1 border-red-200 bg-red-50 px-3 py-1 text-sm font-medium text-red-700'>
                    <ExclamationCircleIcon className='h-4 w-4' />
                    Errors: {errorCount}
                  </Badge>
                )}
              </div>

              <Separator className='mt-4' />
            </CardHeader>
            <CardContent className='p-0'>
              <ImportedPollingStationsDataTable
                data={pollingStations}
                updatePollingStation={updatePollingStation}
                deletePollingStation={deletePollingStation}
              />
            </CardContent>
          </Card>
        )}
      </div>
    </Layout>
  );
}
