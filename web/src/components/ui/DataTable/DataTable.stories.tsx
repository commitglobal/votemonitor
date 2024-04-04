import type { PageParameters, PageResponse } from '@/common/types';
import { useQuery, type UseQueryResult } from '@tanstack/react-query';
import { DataTable } from './DataTable';

export default {
  component: DataTable,
  title: 'DataTable',
  tags: ['datatable', 'table'],
};

type Task = { id: number; name: string };

export const Default = {
  args: {
    columns: [
      {
        header: 'ID',
        accessorKey: 'id',
      },
      {
        header: 'Task name',
        accessorKey: 'name',
      },
    ],
    useQuery: (p: PageParameters): UseQueryResult<PageResponse<Task>, Error> =>
      useQuery({
        queryKey: ['fetchData', p.pageNumber, p.pageSize],
        queryFn: () =>
          ({
            currentPage: p.pageNumber - 1,
            pageSize: 5,
            totalCount: 12,
            items: [
              { id: 1, name: 'Task 1' },
              { id: 2, name: 'Task 2' },
              { id: 3, name: 'Task 3' },
            ],
          }) as PageResponse<Task>,
      }),
  },
};
