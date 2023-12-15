import type { ReactElement } from 'react';
import { DataTable, type DataTableProps } from './DataTable';
import type { PageParameters } from '@/common/types';
import { useNavigate, useSearch } from '@tanstack/router';
import type { PaginationState } from '@tanstack/react-table';

export function QueryParamsDataTable<TData, TValue>({
  columns,
  usePagedQuery: pagedQuery,
}: DataTableProps<TData, TValue>): ReactElement {
  const queryParams: PageParameters = useSearch();

  const paginationState: PaginationState = {
    pageIndex: queryParams.pageNumber - 1,
    pageSize: queryParams.pageSize,
  };

  const navigate = useNavigate();
  const setPaginationState = (p: PaginationState): void => {
    navigate({
      search: {
        pageNumber: p.pageIndex + 1,
        pageSize: p.pageSize,
      },
    }).catch(error => { throw error });
  };

  return (
    <div>
      <DataTable
        columns={columns}
        usePagedQuery={pagedQuery}
        paginationExt={paginationState}
        setPaginationExt={setPaginationState}
      />
    </div>
  );
}
