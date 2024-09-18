import { ZFormType } from '@/common/types';
import LanguagesMultiselect from '@/containers/LanguagesMultiselect';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useLanguages } from '@/features/languages/queries';
import { mapFormType } from '@/lib/utils';
import { FC } from 'react';
import { SelectFilterOption } from '../../filtering/components/SelectFilter';
import { useFilteringContainer } from '../../filtering/hooks/useFilteringContainer';

const responseFormTypeOptions: SelectFilterOption[] = [
  {
    value: ZFormType.Values.Opening,
    label: mapFormType(ZFormType.Values.Opening),
  },

  {
    value: ZFormType.Values.Voting,
    label: mapFormType(ZFormType.Values.Voting),
  },

  {
    value: ZFormType.Values.ClosingAndCounting,
    label: mapFormType(ZFormType.Values.ClosingAndCounting),
  },
  {
    value: ZFormType.Values.Other,
    label: mapFormType(ZFormType.Values.Other),
  },
];

export const ResponseFormLanguageFilter: FC = () => {
  const { queryParams, navigateHandler } = useFilteringContainer();

  const data = useLanguages();
  console.log(data);

  const onStatusChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.FormTypeFilter]: value });
  };

  return <LanguagesMultiselect />;
};
