import { FunctionComponent, importPollingStationSchema, PollingStation } from '@/common/types';
import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { FileUploader } from '@/components/ui/file-uploader';
import { Separator } from '@/components/ui/separator';
import Papa from 'papaparse';
import { useCallback, useMemo, useState } from 'react';
import { z, ZodIssue } from 'zod';

import API from '@/services/api';
import { Button } from '@/components/ui/button';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { pollingStationsKeys } from '@/hooks/polling-stations-levels';
import { downloadImportExample, TemplateType } from '@/lib/utils';
import { queryClient } from '@/main';
import { ArrowDownTrayIcon } from '@heroicons/react/24/outline';
import { useMutation } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { LoaderIcon } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import { ImportedPollingStationsDataTable } from './ImportedPollingStationsDataTable';
import { toast } from 'sonner';

export type ImportPollingStationRow = z.infer<typeof importPollingStationSchema> & { errors?: ZodIssue[] };
const standardFields = [
  'id',
  'level1',
  'level2',
  'level3',
  'level4',
  'level5',
  'number',
  'displayOrder',
  'address',
  'latitude',
  'longitude',
];
export function PollingStationsImport(): FunctionComponent {
  const [pollingStations, setPollingStations] = useState<ImportPollingStationRow[]>([]);
  const { t } = useTranslation('translation', { keyPrefix: 'electionEvent.pollingStations.addPollingStation' });
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const navigate = useNavigate();

  const hasInvalidPollingStations = useMemo(() => {
    return pollingStations.some((pollingStation) => pollingStation.errors && pollingStation.errors?.length > 0);
  }, [pollingStations]);

  const { mutate, isPending } = useMutation({
    mutationFn: ({
      electionRoundId,
      pollingStations,
    }: {
      electionRoundId: string;
      pollingStations: ImportPollingStationRow[];
    }) => {
      return API.post(`/election-rounds/${electionRoundId}/polling-stations`, { pollingStations });
    },

    onSuccess: (_, { electionRoundId }) => {
      toast.success(t('onSuccess'));

      queryClient.invalidateQueries({ queryKey: pollingStationsKeys.all(electionRoundId) });
      navigate({ to: '/election-rounds/$electionRoundId', params: { electionRoundId } });
    },
    onError: () => {
      toast.error(t('onError'), {
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
      <Card>
        <CardHeader>
          <CardDescription>
            In order to successfully import a list of polling stations, please use the template provided below. Download
            the template, fill it in with the observer information and then upload it. No other format is accepted for
            import.
          </CardDescription>
        </CardHeader>

        <CardContent className='flex flex-col gap-4'>
          <div
            onClick={() => downloadImportExample(TemplateType.PollingStations)}
            className='px-3 py-1 rounded-lg cursor-pointer bg-purple-50 w-[300px] '>
            <div className='flex flex-row gap-1 text-sm text-purple-900 '>
              <ArrowDownTrayIcon className='w-[15px]' />
              polling_station_import_template.csv
            </div>
          </div>
          <FileUploader
            accept={{ 'text/csv': [] }}
            multiple={false}
            maxSize={50 * 1024 * 1024}
            maxFileCount={1}
            onValueChange={(files: File[]) => {
              const file = files[0];
              if (!file) {
                setPollingStations([]);
              } else {
                Papa.parse<Record<keyof PollingStation, string>>(file, {
                  header: true,
                  skipEmptyLines: true,
                  // worker: true,
                  transformHeader: (header) => header.charAt(0).toLowerCase() + header.slice(1),
                  async complete(results) {
                    if (results.errors.length) {
                      // Optionally show an error message to the user.
                      toast.error('Parsing errors', {
                        description: 'Please check the file and try again',
                      });
                    }

                    const validatedPollingStations = results.data
                      .map((row) => {
                        const tags: Record<string, string> = {};
                        const pollingStation: Record<string, any> = {};

                        // Separate standard fields from tags
                        Object.entries(row).forEach(([header, value]) => {
                          if (standardFields.includes(header)) {
                            pollingStation[header as keyof ImportPollingStationRow] = value as string | null; // Keep standard fields as direct properties
                          } else if (value != null && value !== '') {
                            tags[header] = value; // Only add to tags if value exists
                          }
                        });

                        pollingStation['tags'] = tags;
                        return pollingStation;
                      })
                      .map((pollingStation) => {
                        const validationResult = importPollingStationSchema.safeParse(pollingStation);
                        const data: ImportPollingStationRow = {
                          ...(validationResult.success
                            ? validationResult.data
                            : (pollingStation as ImportPollingStationRow)),
                          id: validationResult.success ? validationResult.data.id : crypto.randomUUID(),
                          errors: validationResult.success ? [] : validationResult.error.errors,
                        };

                        return data;
                      });

                    setPollingStations(validatedPollingStations);
                  },
                });
              }
            }}
          />
        </CardContent>
      </Card>
      {pollingStations.length ? (
        <Card className='mt-8'>
          <CardHeader>
            <CardTitle>Polling stations to be imported</CardTitle>
            <CardDescription>
              <div className='flex flex-inline justify-between'>
                <span>Review the data and correct the errors before import </span>{' '}
                <Button onClick={handleImportPollingStations} disabled={hasInvalidPollingStations || isPending}>
                  {isPending && <LoaderIcon className='animate-spin' />}
                  Import {pollingStations.length} polling stations
                </Button>
              </div>{' '}
            </CardDescription>
            <Separator />
          </CardHeader>
          <CardContent className='p-0'>
            <div>
              <ImportedPollingStationsDataTable data={pollingStations} />
            </div>
          </CardContent>
        </Card>
      ) : null}
    </Layout>
  );
}
