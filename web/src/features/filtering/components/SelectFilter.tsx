import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { FC } from 'react';

export type SelectFilterOption = {
  value: string;
  label: string;
};

interface SelectFilterProps {
  placeholder: string;
  value: string;
  options: SelectFilterOption[];
  onChange: (value: string) => void;
}

export const SelectFilter: FC<SelectFilterProps> = (props) => {
  const { placeholder, value, options, onChange } = props;

  const selectId = crypto.randomUUID();

  return (
    <Select value={value ?? ''} onValueChange={onChange}>
      <SelectTrigger>
        <SelectValue placeholder={placeholder} />
      </SelectTrigger>
      <SelectContent>
        <SelectGroup>
          {options.map((option) => {
            return (
              <SelectItem key={`select-${selectId}-${option.value}`} value={option.value}>
                {option.label}
              </SelectItem>
            );
          })}
        </SelectGroup>
      </SelectContent>
    </Select>
  );
};

interface BinarySelectFilterProps extends Omit<SelectFilterProps, 'options'> {}

export const BinarySelectFilter: FC<BinarySelectFilterProps> = (props) => {
  const options: SelectFilterOption[] = [
    { value: 'true', label: 'Yes' },
    { value: 'false', label: 'No' },
  ];

  return <SelectFilter options={options} {...props} />;
};
