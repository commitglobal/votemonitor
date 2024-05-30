/* eslint-disable unicorn/prefer-top-level-await */
import { z } from 'zod';

export const FormSubmissionsSearchParamsSchema = z.object({
  viewBy: z.enum(['byEntry', 'byObserver', 'byForm']).catch('byEntry').default('byEntry'),
  tab: z.enum(['form-answers', 'quick-reports']).catch('form-answers').optional(),
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
  followUpStatus: z.enum(['', 'NotApplicable', 'NeedsFollowUp', 'Resolved']).optional().catch(''),
  quickReportLocationType: z.enum(['', 'NotRelatedToAPollingStation', 'OtherPollingStation', 'VisitedPollingStation']).optional().catch(''),
});

export type FormSubmissionsSearchParams = z.infer<typeof FormSubmissionsSearchParamsSchema>;

export const QuickReportsSearchParamsSchema = z.object({
  level1Filter: z.string().catch('').optional(),
  level2Filter: z.string().catch('').optional(),
  level3Filter: z.string().catch('').optional(),
  level4Filter: z.string().catch('').optional(),
  level5Filter: z.string().catch('').optional(),
  quickReportLocationType: z.enum(['', 'NotRelatedToAPollingStation', 'OtherPollingStation', 'VisitedPollingStation']).optional().catch(''),
  followUpStatus: z.enum(['', 'NotApplicable', 'NeedsFollowUp', 'Resolved']).optional().catch('')
});

export type QuickReportsSearchParams = z.infer<typeof QuickReportsSearchParamsSchema>;
