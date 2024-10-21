/* eslint-disable unicorn/prefer-top-level-await */
import { QuestionsAnswered } from '@/common/types';
import { z } from 'zod';

export const PushMessageTargetedObserversSearchParamsSchema = z.object({
  searchText: z.string().catch('').optional(),
  monitoringObserverStatus: z.string().optional().catch(''),
  formTypeFilter: z.string().catch('').optional(),
  level1Filter: z.string().catch('').optional(),
  level2Filter: z.string().catch('').optional(),
  level3Filter: z.string().catch('').optional(),
  level4Filter: z.string().catch('').optional(),
  level5Filter: z.string().catch('').optional(),
  pollingStationNumberFilter: z.string().catch('').optional(),
  sortColumnName: z.string().catch('observerName').optional(),
  tags: z.array(z.string()).optional().catch([]).optional(),
  pageSize: z.number().catch(25).default(25).optional(),
  submissionsFromDate: z.coerce.date().optional(),
  submissionsToDate: z.coerce.date().optional(),
  questionsAnswered: z.nativeEnum(QuestionsAnswered).optional(),
  formId: z.string().optional(),
});

export type PushMessageTargetedObserversSearchParams = z.infer<typeof PushMessageTargetedObserversSearchParamsSchema>;
