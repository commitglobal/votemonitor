"use client";

import { SortOrder } from "@/types/common";
import { useNavigate, useSearch } from "@tanstack/react-router";
import {
  type ColumnSort,
  type PaginationState,
  type SortingState,
  type TableOptions,
  type TableState,
  type Updater,
  type VisibilityState,
  getCoreRowModel,
  getFacetedMinMaxValues,
  getFacetedRowModel,
  getFacetedUniqueValues,
  getFilteredRowModel,
  getPaginationRowModel,
  getSortedRowModel,
  useReactTable,
} from "@tanstack/react-table";
import * as React from "react";

export interface ExtendedColumnSort<TData> extends Omit<ColumnSort, "id"> {
  id: Extract<keyof TData, string>;
}

interface UseDataTableProps<TData>
  extends Omit<
      TableOptions<TData>,
      | "state"
      | "pageCount"
      | "getCoreRowModel"
      | "manualFiltering"
      | "manualPagination"
      | "manualSorting"
    >,
    Required<Pick<TableOptions<TData>, "pageCount">> {
  initialState?: Omit<Partial<TableState>, "sorting"> & {
    sorting?: ExtendedColumnSort<TData>[];
  };
  history?: "push" | "replace";
  startTransition?: React.TransitionStartFunction;
}

export function useDataTable<TData>(props: UseDataTableProps<TData>) {
  const {
    columns,
    pageCount = -1,
    initialState,
    history = "replace",
    startTransition,
    ...tableProps
  } = props;

  const [columnVisibility, setColumnVisibility] =
    React.useState<VisibilityState>(initialState?.columnVisibility ?? {});

  const navigate = useNavigate();
  const search = useSearch({ strict: false });

  const pagination: PaginationState = React.useMemo(() => {
    return {
      pageIndex: (search.pageNumber ?? 1) - 1, // zero-based index -> one-based index
      pageSize: search.pageSize ?? 25,
    };
  }, [search.pageNumber, search.pageSize]);

  const sorting: SortingState = React.useMemo(() => {
    if (search.sortColumnName && search.sortOrder) {
      return [
        {
          id: search.sortColumnName,
          desc: SortOrder.Desc === search.sortOrder,
        },
      ];
    }
    return initialState?.sorting ?? [];
  }, [search.sortColumnName, search.sortOrder, initialState?.sorting]);

  const onPaginationChange = React.useCallback(
    (updaterOrValue: Updater<PaginationState>) => {
      if (typeof updaterOrValue === "function") {
        const newPagination = updaterOrValue(pagination);
        navigate({
          // @ts-ignore
          search: (prev) => ({
            ...prev,
            pageNumber: newPagination.pageIndex + 1,
            pageSize: newPagination.pageSize,
          }),
        });
      } else {
        navigate({
          // @ts-ignore
          search: (prev) => ({
            ...prev,
            pageNumber: updaterOrValue.pageIndex + 1,
            pageSize: updaterOrValue.pageSize,
          }),
        });
      }
    },
    [pagination, navigate]
  );

  const onSortingChange = React.useCallback(
    (updaterOrValue: Updater<SortingState>) => {
      if (typeof updaterOrValue === "function") {
        const newSorting = updaterOrValue(sorting);
        navigate({
          //@ts-ignore
          search: (prev) => ({
            ...prev,
            sortColumnName: newSorting[0].id,
            sortOrder: newSorting[0].desc ? SortOrder.Desc : SortOrder.Asc,
          }),
        });
      } else {
        navigate({
          //@ts-ignore
          search: (prev) => ({
            ...prev,
            sortColumnName: updaterOrValue[0].id,
            sortOrder: updaterOrValue[0].desc ? SortOrder.Desc : SortOrder.Asc,
          }),
        });
      }
    },
    [sorting, navigate]
  );

  const table = useReactTable({
    ...tableProps,
    columns,
    initialState,
    pageCount,
    state: {
      pagination,
      sorting,
      columnVisibility,
    },
    defaultColumn: {
      ...tableProps.defaultColumn,
      enableColumnFilter: false,
    },
    enableRowSelection: true,
    onPaginationChange,
    onSortingChange,
    onColumnVisibilityChange: setColumnVisibility,
    getCoreRowModel: getCoreRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFacetedRowModel: getFacetedRowModel(),
    getFacetedUniqueValues: getFacetedUniqueValues(),
    getFacetedMinMaxValues: getFacetedMinMaxValues(),
    manualPagination: true,
    manualSorting: true,
    manualFiltering: true,
  });

  return { table };
}
