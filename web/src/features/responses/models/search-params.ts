/* eslint-disable unicorn/prefer-top-level-await */
import { z } from 'zod';

export const FormSubmissionsByEntrySearchParamsSchema = z.object({
  formCodeFilter: z.string().catch('').optional(),
  formTypeFilter: z.string().catch('').optional(),
  level1Filter: z.string().catch('').optional(),
  level2Filter: z.string().catch('').optional(),
  level3Filter: z.string().catch('').optional(),
  level4Filter: z.string().catch('').optional(),
  level5Filter: z.string().catch('').optional(),
  pollingStationNumberFilter: z.string().catch('').optional(),
  hasFlaggedAnswers: z.string().catch('').optional(),
  monitoringObserverId: z.string().catch('').optional(),
});

export type FormSubmissionsByEntrySearchParams = z.infer<typeof FormSubmissionsByEntrySearchParamsSchema>