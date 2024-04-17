import type { ReactElement } from 'react';
import { DataTable, type DataTableProps } from './DataTable';
import { SortOrder, type DataTableParameters } from '@/common/types';
import type { PaginationState, SortingState } from '@tanstack/react-table';
import { useNavigate, useSearch } from '@tanstack/react-router';
import { valueOrDefault } from '@/lib/utils';

export function QueryParamsDataTable<TData, TValue>({
  columns,
  useQuery: pagedQuery,
  getSubrows,
  getRowClassName
}: DataTableProps<TData, TValue>): ReactElement {
  const queryParams: DataTableParameters = useSearch({
    strict: false,
  });

  const paginationState: PaginationState = {
    pageIndex: valueOrDefault(queryParams.pageNumber, 1) - 1,
    pageSize: valueOrDefault(queryParams.pageSize, 10),
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
        pageNumber: valueOrDefault(p.pageIndex, 0) + 1,
        pageSize: valueOrDefault(p.pageSize, 10),
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
        sortColumnName: s[0]?.id,
        sortOrder: s[0]?.desc ? SortOrder.desc : SortOrder.asc,
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
        getSubrows={getSubrows}
        getRowClassName={getRowClassName}
      />
    </div>
  );
}
