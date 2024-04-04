import { render } from '@testing-library/react';
import { DataTable, type DataTableProps } from './DataTable';
import { test, expect, vi } from 'vitest';

test('should call pagedQuery to get data', () => {
  const useQuery = vi.fn().mockImplementation(() => {
    return {
      data: {
        items: [],
        totalCount: 0,
        pageSize: 10,
      },
      isFetching: false,
    };
  }) as DataTableProps<null, null>['useQuery'];

  render(<DataTable columns={[]} useQuery={useQuery} />);
  expect(useQuery).toHaveBeenCalledOnce();
  expect(useQuery).toHaveBeenCalledWith({
    pageNumber: 1,
    pageSize: 10,
  });
});
