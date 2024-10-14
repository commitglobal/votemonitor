import { FC } from 'react';
import { SelectFilter, SelectFilterOption } from './SelectFilter';

interface BinarySelectFilterProps {
  placeholder: string;
  value: string;
  onChange: (value: string) => void;
}

export const BinarySelectFilter: FC<BinarySelectFilterProps> = (props) => {
  const options: SelectFilterOption[] = [
    { value: 'true', label: 'Yes' },
    { value: 'false', label: 'No' },
  ];

  return <SelectFilter options={options} {...props} />;
};
