import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { ColumnDef } from '@tanstack/react-table';

import { ElectionRoundStatus, ReportedError, type Location } from '@/common/types';
import { LocationsFilters } from '@/components/LocationsFilters/LocationsFilters';
import { FilterBadge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { AuthContext } from '@/context/auth.context';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useElectionRoundDetails } from '@/features/election-event/hooks/election-event-hooks';
import { ExportDataButton } from '@/features/responses/components/ExportDataButton/ExportDataButton';
import { ExportedDataType } from '@/features/responses/models/data-export';
import { locationsKeys } from '@/hooks/locations-levels';
import i18n from '@/i18n';
import { sendErrorToSentry } from '@/lib/sentry';
import { queryClient } from '@/main';
import { ArrowUpTrayIcon, FunnelIcon } from '@heroicons/react/24/outline';
import { Link, useNavigate, useRouter, useSearch } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { useCallback, useContext, useMemo, useState, type ReactElement } from 'react';
import { LocationDataTableRowActions } from '../LocationDataTableRowActions/LocationDataTableRowActions';
import { useToast } from '../ui/use-toast';
import { locationColDefs } from './column-defs';
import { useDeleteLocationMutation, useLocations, useUpdateLocationMutation } from './hooks';

export default function LocationsDashboard(): ReactElement {
  const navigate = useNavigate();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: electionRound } = useElectionRoundDetails(currentElectionRoundId);
  const { userRole } = useContext(AuthContext);
  const { toast } = useToast();
  const router = useRouter();

  const { mutate: deleteLocationMutation } = useDeleteLocationMutation();
  const { mutate: updateLocationMutation } = useUpdateLocationMutation();

  const deleteLocationCallback = useCallback(
    (location: Location) =>
      deleteLocationMutation({
        electionRoundId: currentElectionRoundId,
        locationId: location.id,
        onSuccess: () => {
          queryClient.invalidateQueries({ queryKey: locationsKeys.all(currentElectionRoundId) });
          router.invalidate();

          toast({
            title: 'Success',
            description: 'Location deleted',
          });
        },
        onError: (error: ReportedError) => {
          const title = 'Error occured when deleting location';
          sendErrorToSentry({ error, title });
          toast({
            title,
            variant: 'destructive',
          });
        },
      }),
    [currentElectionRoundId, deleteLocationMutation]
  );

  const updateLocationCallback = useCallback(
    (location: Location) =>
      updateLocationMutation({
        electionRoundId: currentElectionRoundId,
        locationId: location.id,
        location,
        onSuccess: () => {
          queryClient.invalidateQueries({ queryKey: locationsKeys.all(currentElectionRoundId) });
          router.invalidate();

          toast({
            title: 'Success',
            description: 'Location updated',
          });
        },
        onError: (error: ReportedError) => {
          const title = 'Error occured when updating location';
          sendErrorToSentry({ error, title });
          toast({
            title,
            variant: 'destructive',
          });
        },
      }),
    [currentElectionRoundId, updateLocationMutation]
  );

  const columns: ColumnDef<Location>[] = useMemo(() => {
    if (userRole === 'PlatformAdmin') {
      return [
        ...locationColDefs,
        {
          id: 'actions',
          header: '',
          cell: ({ row }) => (
            <LocationDataTableRowActions
              location={row.original}
              updateLocation={updateLocationCallback}
              deleteLocation={deleteLocationCallback}
            />
          ),
        },
      ];
    }

    return [...locationColDefs];
  }, [userRole]);

  const search = useSearch({ strict: false }) as {
    level1Filter?: string;
    level2Filter?: string;
    level3Filter?: string;
    level4Filter?: string;
    level5Filter?: string;
    locationNumberFilter?: string;
  };

  const [isFiltering, setFiltering] = useState(
    Object.keys(search).some(
      (k) =>
        k === 'level1Filter' ||
        k === 'level2Filter' ||
        k === 'level3Filter' ||
        k === 'level4Filter' ||
        k === 'level5Filter'
    )
  );

  const changeIsFiltering = () => {
    setFiltering((prev) => {
      return !prev;
    });
  };

  const onClearFilter = useCallback(
    (filter: string | string[]) => () => {
      const filters = Array.isArray(filter)
        ? Object.fromEntries(filter.map((key) => [key, undefined]))
        : { [filter]: undefined };
      // @ts-ignore
      navigate({ search: (prev) => ({ ...prev, ...filters }) });
    },
    [navigate]
  );

  const debouncedSearch = useDebounce(search, 300);

  const queryParams = useMemo(() => {
    const params = [
      ['level1Filter', debouncedSearch.level1Filter],
      ['level2Filter', debouncedSearch.level2Filter],
      ['level3Filter', debouncedSearch.level3Filter],
      ['level4Filter', debouncedSearch.level4Filter],
      ['level5Filter', debouncedSearch.level5Filter],
      ['locationNumberFilter', debouncedSearch.locationNumberFilter],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params);
  }, [debouncedSearch]);

  return (
    <Card className='pt-0'>
      <CardHeader className='flex gap-2 flex-column'>
        <div className='flex items-center justify-between'>
          <CardTitle>{i18n.t('electionEvent.locations.cardTitle')}</CardTitle>

          <div className='flex items-center gap-4'>
            <ExportDataButton exportedDataType={ExportedDataType.Locations} />
            {userRole === 'PlatformAdmin' && (
              <Link
                to={'/election-rounds/$electionRoundId/locations/import'}
                params={{ electionRoundId: currentElectionRoundId }}>
                <Button
                  className='bg-purple-900 hover:bg-purple-600'
                  disabled={electionRound?.status === ElectionRoundStatus.Archived}>
                  <div className='flex items-center gap-2'>
                    <ArrowUpTrayIcon className='size-4 text-white' aria-hidden='true' />
                    Import locations
                  </div>
                </Button>
              </Link>
            )}
          </div>
        </div>
        <Separator />
        <div className='flex flex-row justify-end gap-4 px-6 filters'>
          <>
            <FunnelIcon
              onClick={changeIsFiltering}
              className='w-[20px] text-purple-900 cursor-pointer'
              fill={isFiltering ? '#5F288D' : 'rgba(0,0,0,0)'}
            />
          </>
        </div>
        <Separator />
        {isFiltering && (
          <div className='grid items-center grid-cols-6 gap-4 mb-4'>
            <LocationsFilters />

            <Button
              onClick={() => {
                navigate({});
              }}
              variant='ghost-primary'>
              {i18n.t('electionEvent.locations.resetFilters')}
            </Button>
          </div>
        )}
        {Object.entries(search).length > 0 && (
          <div className='flex flex-wrap gap-2 col-span-full'>
            {search.level1Filter && (
              <FilterBadge
                label={`Location - L1: ${search.level1Filter}`}
                onClear={onClearFilter([
                  'level1Filter',
                  'level2Filter',
                  'level3Filter',
                  'level4Filter',
                  'level5Filter',
                ])}
              />
            )}

            {search.level2Filter && (
              <FilterBadge
                label={`Location - L2: ${search.level2Filter}`}
                onClear={onClearFilter(['level2Filter', 'level3Filter', 'level4Filter', 'level5Filter'])}
              />
            )}

            {search.level3Filter && (
              <FilterBadge
                label={`Location - L3: ${search.level3Filter}`}
                onClear={onClearFilter(['level3Filter', 'level4Filter', 'level5Filter'])}
              />
            )}

            {search.level4Filter && (
              <FilterBadge
                label={`Location - L4: ${search.level4Filter}`}
                onClear={onClearFilter(['level4Filter', 'level5Filter'])}
              />
            )}

            {search.level5Filter && (
              <FilterBadge label={`Location - L5: ${search.level5Filter}`} onClear={onClearFilter(['level5Filter'])} />
            )}
          </div>
        )}
      </CardHeader>
      <CardContent className='flex flex-col items-baseline gap-6'>
        <QueryParamsDataTable
          columns={columns!}
          useQuery={(params) => useLocations(currentElectionRoundId, params)}
          queryParams={queryParams}
        />
      </CardContent>
    </Card>
  );
}
