import { useDataSource, useSetDataSource } from '@/common/data-source-store';
import { DataSources, type FunctionComponent } from '@/common/types';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useNavigate, useSearch } from '@tanstack/react-router';
import { useCallback, useEffect, useState } from 'react';
import { Label } from '../ui/label';
import { Switch } from '../ui/switch';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { omit } from '../../lib/utils';

export function DataSourceSwitcher(): FunctionComponent {
  const isCoalitionLeader = useCurrentElectionRoundStore((s) => s.isCoalitionLeader);

  const navigate = useNavigate();

  const search: any = useSearch({
    strict: false,
  });

  const dataSource = useDataSource();
  const setDataSource = useSetDataSource();

  const navigateHandler = useCallback(
    (dataSource: DataSources) => {
      void navigate({
        search: (prev) => {
          const newSearch: Record<string, string | undefined | string[] | number | Date | boolean> = {
            ...prev,
            dataSource,
          };
          setDataSource(dataSource);
          if (dataSource === DataSources.Ngo) {
            return omit(newSearch, FILTER_KEY.CoalitionMemberId);
          }

          return newSearch;
        },
      });
    },
    [navigate, dataSource]
  );
  const [isCoalition, setIsCoalition] = useState(false);

  useEffect(() => {
    if (search.dataSource === undefined) {
      navigateHandler(dataSource ?? DataSources.Ngo);
      return;
    }

    setIsCoalition((search.dataSource ?? dataSource) === DataSources.Coalition);
  }, [search.dataSource]);

  return isCoalitionLeader ? (
    <div className='flex items-center space-x-4'>
      <Switch
        id='data-source'
        checked={isCoalition}
        onCheckedChange={(checked) => navigateHandler(checked ? DataSources.Coalition : DataSources.Ngo)}
        className='data-[state=checked]:bg-primary'
      />
      <Label htmlFor='data-source' className={`text-md ${isCoalition ? 'font-bold' : ''}`}>
        Show coalition responses
      </Label>
    </div>
  ) : (
    <></>
  );
}
