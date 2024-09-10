import { FilterBadge } from '@/components/ui/badge';
import { useNavigate } from '@tanstack/react-router';
import { FC, useCallback } from 'react';

interface ActiveFilterProps {
  filterId: string;
  value: string;
}

const ActiveFilter: FC<ActiveFilterProps> = ({ filterId, value }) => {
  const navigate = useNavigate();
  const onClearFilter = useCallback(
    (filter: keyof any) => () => {
      void navigate({ search: (prev) => ({ ...prev, [filter]: undefined }) });
    },
    [navigate]
  );
  return <FilterBadge label={`${filterId}: ${value}`} onClear={onClearFilter(filterId)} />;
};

interface ActiveFiltersProps {
  queryParams: any;
}

export const ActiveFilters: FC<ActiveFiltersProps> = ({ queryParams }) => {
  return (
    <div className='flex flex-wrap gap-2 col-span-full'>
      {Object.keys(queryParams).map((key) => {
        const value = queryParams[key];

        if (!Array.isArray(value)) return <ActiveFilter filterId={key} value={value} />;
        return value.map((item) => <ActiveFilter filterId={key} value={item as string} />);
      })}
    </div>
  );
};
