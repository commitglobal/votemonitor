import { getRouteApi } from '@tanstack/react-router';
import type { VisibilityState } from '@tanstack/react-table';
import { useDebounce } from '@uidotdev/usehooks';
import { useMemo } from 'react';
import type { FunctionComponent } from '@/common/types';
import { CardContent } from '@/components/ui/card';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { useFormSubmissionsByEntry } from '../../hooks/form-submissions-queries';
import type { FormSubmissionsSearchParams } from '../../models/search-params';
import { formSubmissionsByEntryColumnDefs } from '../../utils/column-defs';
import { Route } from '@/routes/responses';

type FormsTableByEntryProps = {
  columnsVisibility: VisibilityState;
  searchText: string;
};

export function FormsTableByEntry({ columnsVisibility, searchText }: FormsTableByEntryProps): FunctionComponent {
  const search = Route.useSearch();
  const debouncedSearch = useDebounce(search, 300);

  const queryParams = useMemo(() => {
    const params = [
      ['searchText', searchText],
      ['pollingStationNumberFilter', debouncedSearch.pollingStationNumberFilter],
      ['formTypeFilter', debouncedSearch.formTypeFilter],
      ['hasFlaggedAnswers', debouncedSearch.hasFlaggedAnswers],
      ['level1Filter', debouncedSearch.level1Filter],
      ['level2Filter', debouncedSearch.level2Filter],
      ['level3Filter', debouncedSearch.level3Filter],
      ['level4Filter', debouncedSearch.level4Filter],
      ['level5Filter', debouncedSearch.level5Filter],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params) as FormSubmissionsSearchParams;
  }, [searchText, debouncedSearch]);

  return (
    <CardContent>
      <QueryParamsDataTable
        columnVisibility={columnsVisibility}
        columns={formSubmissionsByEntryColumnDefs}
        useQuery={useFormSubmissionsByEntry}
        queryParams={queryParams}
      />
    </CardContent>
  );
}
