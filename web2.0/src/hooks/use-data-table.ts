"use client";

import {
  getCoreRowModel,
  getFacetedMinMaxValues,
  getFacetedRowModel,
  getFacetedUniqueValues,
  getFilteredRowModel,
  getPaginationRowModel,
  getSortedRowModel,
  useReactTable,
  type ColumnSort,
  type TableOptions,
  type TableState,
  type VisibilityState,
} from "@tanstack/react-table";
import * as React from "react";
import {
  useTableUrlState,
  type NavigateFn,
  type SearchRecord,
} from "./use-table-url-state";
import { useLocalStorage } from "./use-local-storage";

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
  search: SearchRecord;
  navigate: NavigateFn;
  pagination?: {
    pageKey?: string;
    pageSizeKey?: string;
    defaultPage?: number;
    defaultPageSize?: number;
  };
  globalFilter?: {
    enabled?: boolean;
    key?: string;
    trim?: boolean;
  };
  columnFilters?: Array<
    | {
        columnId: string;
        searchKey: string;
        type?: "string";
        // Optional transformers for custom types
        serialize?: (value: unknown) => unknown;
        deserialize?: (value: unknown) => unknown;
      }
    | {
        columnId: string;
        searchKey: string;
        type: "array";
        serialize?: (value: unknown) => unknown;
        deserialize?: (value: unknown) => unknown;
      }
  >;
  tableName: string;
}

export function useDataTable<TData>(props: UseDataTableProps<TData>) {
  const {
    columns,
    pageCount = -1,
    initialState,
    history = "replace",
    startTransition,
    navigate,
    search,
    columnFilters: defaultColumnFilters,
    pagination: defaultPagination,
    globalFilter: defaultGlobalFilter,
    tableName,
    ...tableProps
  } = props;

  const [columnVisibility, setColumnVisibility] =
    useLocalStorage<VisibilityState>(
      `column-visibility-${tableName}`,
      initialState?.columnVisibility ?? {}
    );

  const {
    globalFilter,
    onGlobalFilterChange,
    columnFilters,
    onColumnFiltersChange,
    sorting,
    onSortingChange,
    pagination,
    onPaginationChange,
  } = useTableUrlState({
    search,
    navigate,
    pagination: defaultPagination,
    globalFilter: defaultGlobalFilter,
    columnFilters: defaultColumnFilters,
  });

  const table = useReactTable({
    ...tableProps,
    columns,
    initialState,
    pageCount,
    state: {
      pagination,
      sorting,
      columnVisibility,
      globalFilter,
      columnFilters,
    },
    defaultColumn: {
      ...tableProps.defaultColumn,
      enableColumnFilter: false,
    },
    enableRowSelection: props.enableRowSelection,
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
    onGlobalFilterChange,
    onColumnFiltersChange,
    manualPagination: true,
    manualSorting: true,
    manualFiltering: true,
  });

  return { table };
}
