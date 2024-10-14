import { FC } from 'react';

import { DateTimeFormat } from '@/common/formats';
import { DateTimePicker } from '@/components/ui/date-time-picker';

interface DateTimeFilterProps {
  placeholder: string;
  value: Date | undefined;
  onChange: (value: Date | undefined) => void;
  granularity: 'day' | 'hour' | 'minute' | 'second';
}

export const DateTimeFilter: FC<DateTimeFilterProps> = (props) => {
  const { placeholder, granularity, value, onChange } = props;

  return (
    <DateTimePicker
      granularity={granularity}
      placeholder={placeholder}
      value={value}
      onChange={onChange}
      displayFormat={{ hour24: DateTimeFormat }}
    />
  );
};
