import { FunctionComponent, importLocationSchema } from '@/common/types';
import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { FileUploader } from '@/components/ui/file-uploader';
import { Separator } from '@/components/ui/separator';
import Papa from 'papaparse';
import { useCallback, useMemo, useState } from 'react';
import { z, ZodIssue } from 'zod';

import { Button } from '@/components/ui/button';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { locationsKeys } from '@/hooks/locations-levels';
import { downloadImportExample, TemplateType } from '@/lib/utils';
import { queryClient } from '@/main';
import API from '@/services/api';
import { ArrowDownTrayIcon } from '@heroicons/react/24/outline';
import { useMutation } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { LoaderIcon } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import { toast } from 'sonner';
import { ImportedLocationsDataTable } from './ImportedLocationsDataTable';

export type ImportLocationRow = z.infer<typeof importLocationSchema> & { errors?: ZodIssue[] };

export function LocationsImport(): FunctionComponent {
  const [locations, setLocations] = useState<ImportLocationRow[]>([]);
  const { t } = useTranslation('translation', { keyPrefix: 'electionEvent.locations.addLocation' });
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const navigate = useNavigate();

  function deleteLocation(location: ImportLocationRow) {
    setLocations((prev) => [...prev.filter((obs) => obs.id !== location.id)]);
  }

  function updateLocation(location: ImportLocationRow) {
    const validationResult = importLocationSchema.safeParse(location);

    const locationWithErorrs = {
      ...location,
      errors: validationResult.success ? [] : validationResult.error.errors,
    };

    setLocations((prevData) => prevData.map((o) => (o.id === location.id ? { ...o, ...locationWithErorrs } : o)));
  }

  const hasInvalidLocations = useMemo(() => {
    return locations.some((location) => location.errors && location.errors?.length > 0);
  }, [locations]);

  const { mutate, isPending } = useMutation({
    mutationFn: ({ electionRoundId, locations }: { electionRoundId: string; locations: ImportLocationRow[] }) => {
      return API.post(`/election-rounds/${electionRoundId}/locations`, { locations });
    },

    onSuccess: (_, { electionRoundId }) => {
      toast.success(t('onSuccess'));

      queryClient.invalidateQueries({ queryKey: locationsKeys.all(electionRoundId) });
      navigate({ to: '/election-rounds/$electionRoundId', params: { electionRoundId } });
    },
    onError: () => {
      toast.error(t('onError'), {
        description: 'Please contact tech support',
      });
    },
  });

  const handleImportLocations = useCallback(
    () => mutate({ electionRoundId: currentElectionRoundId, locations }),
    [currentElectionRoundId, locations]
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
            onClick={() => downloadImportExample(TemplateType.Locations)}
            className='px-3 py-1 rounded-lg cursor-pointer bg-purple-50 w-[300px] '>
            <div className='flex flex-row gap-1 text-sm text-purple-900 '>
              <ArrowDownTrayIcon className='w-[15px]' />
              polling_station_import_template.csv
            </div>
          </div>
          <FileUploader
            accept={{ 'text/csv': [] }}
            multiple={false}
            maxSize={5 * 1024 * 1024}
            maxFileCount={1}
            onValueChange={(files: File[]) => {
              const file = files[0];
              if (!file) {
                setLocations([]);
              } else {
                Papa.parse<ImportLocationRow>(file, {
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

                    const validatedLocations = results.data.map((location) => {
                      const validationResult = importLocationSchema.safeParse(location);
                      return {
                        ...(validationResult.success ? validationResult.data : location),
                        id: crypto.randomUUID(),
                        errors: validationResult.success ? [] : validationResult.error.errors,
                      };
                    });

                    setLocations(validatedLocations);
                  },
                });
              }
            }}
          />
        </CardContent>
      </Card>
      {locations.length ? (
        <Card className='mt-8'>
          <CardHeader>
            <CardTitle>Locations to be imported</CardTitle>
            <CardDescription>
              <div className='flex flex-inline justify-between'>
                <span>Review the data and correct the errors before import </span>{' '}
                <Button onClick={handleImportLocations} disabled={hasInvalidLocations || isPending}>
                  {isPending && <LoaderIcon className='animate-spin' />}
                  Import {locations.length} locations
                </Button>
              </div>{' '}
            </CardDescription>
            <Separator />
          </CardHeader>
          <CardContent className='p-0'>
            <div>
              <ImportedLocationsDataTable
                data={locations}
                updateLocation={updateLocation}
                deleteLocation={deleteLocation}
              />
            </div>
          </CardContent>
        </Card>
      ) : null}
    </Layout>
  );
}
