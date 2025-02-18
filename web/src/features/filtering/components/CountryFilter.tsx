import { useCountries } from '@/hooks/countries';
import { useMemo } from 'react';
import { FILTER_KEY } from '../filtering-enums';
import { useFilteringContainer } from '../hooks/useFilteringContainer';
import { SelectFilter } from './SelectFilter';

export default function CountryFilter() {
  const { queryParams, navigateHandler } = useFilteringContainer();

  const onChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.CountryIdFilter]: value });
  };

  const { data: countries } = useCountries();

  const options = useMemo(
    () =>
      countries?.map((country) => ({
        value: country.id,
        label: country.fullName,
      })) ?? [],
    [countries]
  );

  return (
    <SelectFilter
      value={(queryParams as any)[FILTER_KEY.CountryIdFilter]}
      onChange={onChange}
      options={options}
      placeholder='Country'
    />
  );
}
