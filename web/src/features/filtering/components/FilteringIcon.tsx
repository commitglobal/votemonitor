import { FunnelIcon } from '@heroicons/react/24/outline';
import { FC, SetStateAction } from 'react';

interface FilteringIconProps {
  filteringIsExpanded: boolean;
  setFilteringIsExpanded: (value: SetStateAction<boolean>) => void;
}

export const FilteringIcon: FC<FilteringIconProps> = ({ filteringIsExpanded, setFilteringIsExpanded }) => {
  return (
    <FunnelIcon
      title={`${filteringIsExpanded ? 'Collapse' : 'Expand'} filtering`}
      className='w-[20px] text-purple-900 cursor-pointer'
      fill={filteringIsExpanded ? '#5F288D' : 'rgba(0,0,0,0)'}
      onClick={() => {
        setFilteringIsExpanded((prev) => !prev);
      }}
    />
  );
};
