/* eslint-disable unicorn/prefer-top-level-await */
import { z } from 'zod';

export const FormSubmissionsSearchParamsSchema = z.object({
  searchText: z.string().catch('').optional(),
  formTypeFilter: z.string().catch('').optional(),
  level1Filter: z.string().catch('').optional(),
  level2Filter: z.string().catch('').optional(),
  level3Filter: z.string().catch('').optional(),
  level4Filter: z.string().catch('').optional(),
  level5Filter: z.string().catch('').optional(),
  pollingStationNumberFilter: z.string().catch('').optional(),
  hasFlaggedAnswers: z.string().catch('').optional(),
  monitoringObserverId: z.string().catch('').optional(),
  tagsFilter: z.array(z.string()).optional().catch([]).optional(),
});

export type FormSubmissionsSearchParams = z.infer<typeof FormSubmissionsSearchParamsSchema>;
