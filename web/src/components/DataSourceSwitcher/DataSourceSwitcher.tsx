import { usePrevDataSource, useSetPrevDataSource } from '@/common/prev-data-source-store';
import { DataSources, type FunctionComponent } from '@/common/types';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useNavigate, useSearch } from '@tanstack/react-router';
import { useCallback, useEffect, useState } from 'react';
import { Label } from '../ui/label';
import { Switch } from '../ui/switch';

export function DataSourceSwitcher(): FunctionComponent {
  const isCoalitionLeader = useCurrentElectionRoundStore((s) => s.isCoalitionLeader);

  const navigate = useNavigate();

  const search: any = useSearch({
    strict: false,
  });

  const prevDataSource = usePrevDataSource();
  const setPrevDataSource = useSetPrevDataSource();

  const navigateHandler = useCallback(
    (dataSource: DataSources) => {
      void navigate({
        search: (prev) => {
          const newSearch: Record<string, string | undefined | string[] | number | Date | boolean> = {
            ...prev,
            dataSource,
          };
          setPrevDataSource(dataSource);
          return newSearch;
        },
      });
    },
    [navigate, setPrevDataSource]
  );
  const [isCoalition, setIsCoalition] = useState(false);

  useEffect(() => {
    if (search.dataSource === undefined) {
      navigateHandler(prevDataSource ?? DataSources.MyNgo);
      return;
    }

    setIsCoalition((search.dataSource ?? prevDataSource) === DataSources.Coalition);
  }, [search.dataSource]);

  return isCoalitionLeader ? (
    <div className='flex items-center space-x-4'>
      <Switch
        id='data-source'
        checked={isCoalition}
        onCheckedChange={(checked) => navigateHandler(checked ? DataSources.Coalition : DataSources.MyNgo)}
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
