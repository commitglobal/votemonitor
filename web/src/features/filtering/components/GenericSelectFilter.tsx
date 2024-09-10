import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { FC } from 'react';

export type GenericSelectFilterOption = {
  value: string;
  label: string;
};

interface GenericSelectFilterProps {
  placeholder: string;
  value: string;
  options: GenericSelectFilterOption[];
  onChange: (value: string) => void;
}

export const GenericSelectFilter: FC<GenericSelectFilterProps> = (props) => {
  const { placeholder, value, options, onChange } = props;

  return (
    <Select value={value ?? ''} onValueChange={onChange}>
      <SelectTrigger className='w-[180px]'>
        <SelectValue placeholder={placeholder} />
      </SelectTrigger>
      <SelectContent>
        <SelectGroup>
          {options.map((option) => {
            return <SelectItem value={option.value}>{option.label}</SelectItem>;
          })}
        </SelectGroup>
      </SelectContent>
    </Select>
  );
};
