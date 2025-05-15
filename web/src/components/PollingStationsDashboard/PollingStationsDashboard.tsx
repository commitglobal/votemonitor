import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { ColumnDef } from '@tanstack/react-table';

import { DataTableRowAction, ElectionRoundStatus, PollingStation } from '@/common/types';
import { PollingStationsFilters } from '@/components/PollingStationsFilters/PollingStationsFilters';
import { FilterBadge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { AuthContext } from '@/context/auth.context';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useElectionRoundDetails } from '@/features/election-event/hooks/election-event-hooks';
import { ExportDataButton } from '@/features/responses/components/ExportDataButton/ExportDataButton';
import { ExportedDataType } from '@/features/responses/models/data-export';
import i18n from '@/i18n';
import { ArrowUpTrayIcon, FunnelIcon } from '@heroicons/react/24/outline';
import { Link, useNavigate, useSearch } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { Plus } from 'lucide-react';
import { useCallback, useContext, useMemo, useState, type ReactElement } from 'react';
import { DeletePollingStationDialog } from '../DeletePollingStationDialog/DeletePollingStationDialog';
import UpsertPollingStationDialog from '../UpsertPollingStationDialog/UpsertPollingStationDialog';
import { getPollingStationColDefs, PollingStationAction } from './column-defs';
import { usePollingStations } from './hooks';

export default function PollingStationsDashboard(): ReactElement {
  const navigate = useNavigate();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: electionRound } = useElectionRoundDetails(currentElectionRoundId);
  const { userRole } = useContext(AuthContext);
  const [rowAction, setRowAction] = useState<DataTableRowAction<PollingStation, PollingStationAction> | null>(null);

  const columns: ColumnDef<PollingStation>[] = useMemo(() => {
    return getPollingStationColDefs(userRole, setRowAction);
  }, [userRole]);

  const search = useSearch({ strict: false }) as {
    level1Filter?: string;
    level2Filter?: string;
    level3Filter?: string;
    level4Filter?: string;
    level5Filter?: string;
    pollingStationNumberFilter?: string;
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
      ['pollingStationNumberFilter', debouncedSearch.pollingStationNumberFilter],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params);
  }, [debouncedSearch]);

  return (
    <>
      <Card className='pt-0'>
        <CardHeader className='flex gap-2 flex-column'>
          <div className='flex items-center justify-between'>
            <CardTitle>{i18n.t('electionEvent.pollingStations.cardTitle')}</CardTitle>

            <div className='flex items-center gap-4'>
              <ExportDataButton exportedDataType={ExportedDataType.PollingStations} />
              {userRole === 'PlatformAdmin' && (
                <>
                  <Button
                    variant='secondary'
                    disabled={electionRound?.status === ElectionRoundStatus.Archived}
                    onClick={() => setRowAction({ variant: 'add' })}>
                    <Plus className='mr-2' width={18} height={18} />
                    {i18n.t('electionEvent.pollingStations.addPollingStation.addBtnText')}
                  </Button>
                </>
              )}

              {userRole === 'PlatformAdmin' && (
                <Link
                  to={'/election-rounds/$electionRoundId/polling-stations/import'}
                  params={{ electionRoundId: currentElectionRoundId }}>
                  <Button
                    className='bg-purple-900 hover:bg-purple-600'
                    disabled={electionRound?.status === ElectionRoundStatus.Archived}>
                    <div className='flex items-center gap-2'>
                      <ArrowUpTrayIcon className='size-4 text-white' aria-hidden='true' />
                      Import polling stations
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
              <PollingStationsFilters />

              <Button
                onClick={() => {
                  navigate({});
                }}
                variant='ghost-primary'>
                {i18n.t('electionEvent.pollingStations.resetFilters')}
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
                <FilterBadge
                  label={`Location - L5: ${search.level5Filter}`}
                  onClear={onClearFilter(['level5Filter'])}
                />
              )}

              {search.pollingStationNumberFilter && (
                <FilterBadge
                  label={`Polling station number: ${search.pollingStationNumberFilter}`}
                  onClear={onClearFilter(['pollingStationNumberFilter'])}
                />
              )}
            </div>
          )}
        </CardHeader>
        <CardContent className='flex flex-col items-baseline gap-6'>
          <QueryParamsDataTable
            columns={columns!}
            useQuery={(params) => usePollingStations(currentElectionRoundId, params)}
            queryParams={queryParams}
          />
        </CardContent>
      </Card>

      <UpsertPollingStationDialog
        open={rowAction?.variant === 'edit' || rowAction?.variant === 'add'}
        onOpenChange={() => setRowAction(null)}
        pollingStation={rowAction?.row?.original}
      />

      <DeletePollingStationDialog
        open={rowAction?.variant === 'delete'}
        onOpenChange={() => setRowAction(null)}
        pollingStation={rowAction?.row?.original}
        showTrigger={false}
      />
    </>
  );
}
