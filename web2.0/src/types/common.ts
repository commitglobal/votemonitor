export enum SortOrder {
  Asc = "Asc",
  Desc = "Desc",
}

export type PageResponse<T> = {
  currentPage: number;
  pageSize: number;
  totalCount: number;
  items: T[];
};
