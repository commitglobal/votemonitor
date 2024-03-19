import type { ReactElement } from 'react';
import { DataTable, type DataTableProps } from './DataTable';
import { SortOrder, type DataTableParameters } from '@/common/types';
import { useNavigate, useSearch } from '@tanstack/router';
import type { PaginationState, SortingState } from '@tanstack/react-table';

export function QueryParamsDataTable<TData, TValue>({
  columns,
  useQuery: pagedQuery,
}: DataTableProps<TData, TValue>): ReactElement {
  const queryParams: DataTableParameters = useSearch();

  const paginationState: PaginationState = {
    pageIndex: queryParams.pageNumber - 1,
    pageSize: queryParams.pageSize,
  };

  const sortingState: SortingState = [
    {
      id: queryParams.sortColumnName,
      desc: queryParams.sortOrder === SortOrder.desc,
    },
  ];

  const navigate = useNavigate();
  const setPaginationState = (p: PaginationState): void => {
    navigate({
      search: {
        ...queryParams,
        pageNumber: p.pageIndex + 1,
        pageSize: p.pageSize,
      },
    }).catch((error) => {
      throw error;
    });
  };

  const setSortingState = (s: SortingState): void => {
    if (!s.length) {
      return;
    }

    navigate({
      search: {
        ...queryParams,
        sortColumnName: s[0].id,
        sortOrder: s[0].desc ? SortOrder.desc : SortOrder.asc,
      },
    }).catch((error) => {
      throw error;
    });
  };

  return (
    <div>
      <DataTable
        columns={columns}
        useQuery={pagedQuery}
        paginationExt={paginationState}
        setPaginationExt={setPaginationState}
        sortingExt={sortingState}
        setSortingExt={setSortingState}
      />
    </div>
  );
}
