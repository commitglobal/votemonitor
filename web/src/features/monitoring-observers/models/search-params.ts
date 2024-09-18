/* eslint-disable unicorn/prefer-top-level-await */
import { z } from 'zod';

export const PushMessageTargetedObserversSearchParamsSchema = z.object({
  searchText: z.string().catch('').optional(),
  statusFilter: z.string().optional().catch(''),
  level1Filter: z.string().catch('').optional(),
  level2Filter: z.string().catch('').optional(),
  level3Filter: z.string().catch('').optional(),
  level4Filter: z.string().catch('').optional(),
  level5Filter: z.string().catch('').optional(),
  pollingStationNumberFilter: z.string().catch('').optional(),
  sortColumnName: z.string().catch('observerName').optional(),
  tagsFilter: z.array(z.string()).optional().catch([]).optional(),
  pageSize: z.number().catch(25).default(25).optional(),
});

export type PushMessageTargetedObserversSearchParams = z.infer<typeof PushMessageTargetedObserversSearchParamsSchema>;
