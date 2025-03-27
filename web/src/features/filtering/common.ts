import { FILTER_KEY } from "./filtering-enums";

export interface ActiveFilterProps {
  filterId: string;
  value: string;
  isArray?: boolean;
}
export function defaultLocalizator(value: any): string {
  return (value ?? '').toString();
}

export function isBooleanType(value: any): boolean {
  const trimmedValue = value.toString().toLowerCase().trim();

  return trimmedValue === 'true' || trimmedValue === 'false';
}

export function isDateType(value: any): boolean {
  return value instanceof Date && !isNaN(value.getTime());
}

export type SearchParams = {
  [key: string]: any;
};


export const HIDDEN_FILTERS = [
  FILTER_KEY.PageSize,
  FILTER_KEY.PageNumber,
  FILTER_KEY.ViewBy,
  FILTER_KEY.Tab,
  FILTER_KEY.SortOrder,
  FILTER_KEY.SortColumnName,
  FILTER_KEY.DataSource,
];