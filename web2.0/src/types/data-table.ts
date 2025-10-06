import type { ColumnSort, Row, RowData } from "@tanstack/react-table";

declare module "@tanstack/react-table" {
  // biome-ignore lint/correctness/noUnusedVariables: <explanation>
  interface ColumnMeta<TData extends RowData, TValue> {
    label?: string;
  }
}

export interface Option {
  label: string;
  value: string;
  count?: number;
  icon?: React.FC<React.SVGProps<SVGSVGElement>>;
}

export interface ExtendedColumnSort<TData> extends Omit<ColumnSort, "id"> {
  id: Extract<keyof TData, string>;
}

// export interface ExtendedColumnFilter<TData> extends FilterItemSchema {
//   id: Extract<keyof TData, string>;
// }

export interface DataTableRowAction<TData> {
  row: Row<TData>;
  variant: "update" | "delete";
}
