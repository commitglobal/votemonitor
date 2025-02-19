import { z } from 'zod';
import { SortOrder } from './types';

export const PageParametersBaseSchema = z.object({
  pageNumber: z.number().catch(1),
  pageSize: z.number().catch(10),
});

export const SortParamsBaseSchema = z.object({
  sortColumnName: z.string().catch(''),
  searchText: z.coerce.string().optional(),
  sortOrder: z.nativeEnum(SortOrder).catch(SortOrder.asc),
});

export const DefaultSearchParamsSchema = PageParametersBaseSchema.merge(SortParamsBaseSchema);
