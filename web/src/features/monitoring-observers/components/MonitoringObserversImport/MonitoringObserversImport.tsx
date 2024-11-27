import { FunctionComponent } from '@/common/types';
import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { FileUploader } from '@/components/ui/file-uploader';
import { Separator } from '@/components/ui/separator';
import Papa from 'papaparse';
import { useMemo, useState } from 'react';
import { ZodIssue, z } from 'zod';

import { authApi } from '@/common/auth-api';
import { Button } from '@/components/ui/button';
import { toast } from '@/components/ui/use-toast';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { downloadImportExample } from '@/features/monitoring-observers/helpers';
import { queryClient } from '@/main';
import { ArrowDownTrayIcon } from '@heroicons/react/24/outline';
import { useMutation } from '@tanstack/react-query';
import { useTranslation } from 'react-i18next';
import { monitoringObserversKeys } from '../../hooks/monitoring-observers-queries';
import { LoaderIcon } from 'lucide-react';
import { ImportedObserversDataTable } from './ImportedObserversDataTable';
import { useNavigate } from '@tanstack/react-router';

export const importObserversSchema = z.object({
  firstName: z
    .string()
    .min(1, { message: 'First name is required.' })
    .max(256, { message: 'First name cannot exceed 256 characters.' }),
  lastName: z
    .string()
    .min(1, { message: 'Last name is required.' })
    .max(256, { message: 'Last name cannot exceed 256 characters.' }),
  email: z
    .string()
    .min(1, { message: 'Email is required.' })
    .refine((value) => z.string().email().safeParse(value).success, {
      message: 'Invalid email format.',
    }),
  phoneNumber: z.string().max(256, { message: 'Phone number cannot exceed 256 characters.' }).optional(),
});

export type ImportObserverRow = z.infer<typeof importObserversSchema> & { id: string; errors: ZodIssue[] };

export function MonitoringObserversImport(): FunctionComponent {
  const [observers, setObservers] = useState<ImportObserverRow[]>([]);
  const { t } = useTranslation('translation', { keyPrefix: 'observers.addObserver' });
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const navigate = useNavigate();

  function deleteObserver(observer: ImportObserverRow) {
    setObservers((prev) => [...prev.filter((obs) => obs.id !== observer.id)]);
  }

  function updateObserver(observer: ImportObserverRow) {
    const validationResult = importObserversSchema.safeParse(observer);

    const observerWithErorrs = {
      ...observer,
      errors: validationResult.success ? [] : validationResult.error.errors,
    };

    setObservers((prevData) => prevData.map((o) => (o.id === observer.id ? { ...o, ...observerWithErorrs } : o)));
  }

  const hasInvalidObservers = useMemo(() => {
    return observers.some((observer) => observer.errors.length > 0);
  }, [observers]);

  const { mutate, isPending } = useMutation({
    mutationFn: ({ electionRoundId, observers }: { electionRoundId: string; observers: ImportObserverRow[] }) => {
      return authApi.post(`/election-rounds/${electionRoundId}/monitoring-observers`, { observers });
    },

    onSuccess: (_, { electionRoundId }) => {
      toast({
        title: 'Success',
        description: t('onSuccess'),
      });

      queryClient.invalidateQueries({ queryKey: monitoringObserversKeys.all(electionRoundId) });
      navigate({ to: '/monitoring-observers' });
    },
    onError: () => {
      toast({
        title: t('onError'),
        description: 'Please contact tech support',
        variant: 'destructive',
      });
    },
  });

  function handleImportObservers() {
    mutate({ electionRoundId: currentElectionRoundId, observers });
  }

  return (
    <Layout breadcrumbs={false} enableBreadcrumbs={false}>
      <Card>
        <CardHeader>
          <CardTitle className='mb-3.5'>Import monitoring observer list</CardTitle>
          <Separator />
          <CardDescription>
            In order to successfully import a list of monitoring observers, please use the template provided below.
            Download the template, fill it in with the observer information and then upload it. No other format is
            accepted for import.
          </CardDescription>
        </CardHeader>

        <CardContent className='flex flex-col gap-4'>
          <div onClick={downloadImportExample} className='px-3 py-1 rounded-lg cursor-pointer bg-purple-50 w-[300px] '>
            <div className='flex flex-row gap-1 text-sm text-purple-900 '>
              <ArrowDownTrayIcon className='w-[15px]' />
              monitoring_observers_template.csv
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
                setObservers([]);
              } else {
                Papa.parse<ImportObserverRow>(file, {
                  header: true,
                  skipEmptyLines: true,
                  // worker: true,
                  transformHeader: (header) => header.charAt(0).toLowerCase() + header.slice(1),
                  async complete(results) {
                    if (results.errors.length) {
                      console.error('Parsing errors:', results.errors);
                      // Optionally show an error message to the user.
                    }

                    const validatedObservers = results.data.map((observer) => {
                      const observerWithId = {
                        ...observer,
                        id: crypto.randomUUID(),
                      };

                      const validationResult = importObserversSchema.safeParse(observerWithId);

                      return {
                        ...observerWithId,
                        errors: validationResult.success ? [] : validationResult.error.errors,
                      };
                    });

                    setObservers(validatedObservers);
                  },
                });
              }
            }}
          />
        </CardContent>
      </Card>
      {observers.length ? (
        <Card className='mt-8'>
          <CardHeader>
            <CardTitle>Observers to be imported</CardTitle>
            <CardDescription>
              <div className='flex flex-inline justify-between'>
                <span>Review the data and correct the errors before import </span>{' '}
                <Button onClick={handleImportObservers} disabled={hasInvalidObservers || isPending}>
                  {isPending && <LoaderIcon className='animate-spin' />}
                  Import {observers.length} observers
                </Button>
              </div>{' '}
            </CardDescription>
            <Separator/>
          </CardHeader>
          <CardContent className='p-0'>
            <div>
              {' '}
              <ImportedObserversDataTable
                data={observers}
                updateObserver={updateObserver}
                deleteObserver={deleteObserver}
              />{' '}
            </div>
          </CardContent>
        </Card>
      ) : null}
    </Layout>
  );
}
