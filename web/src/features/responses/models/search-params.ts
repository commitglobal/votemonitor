/* eslint-disable unicorn/prefer-top-level-await */
import { z } from 'zod';
import { QuickReportLocationType } from './quick-report';
import { FollowUpStatus } from '@/common/types';

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
  hasFlaggedAnswers: z.string().catch('').optional(),
  monitoringObserverId: z.string().catch('').optional(),
  tagsFilter: z.array(z.string()).optional().catch([]).optional(),
  followUpStatus: z.enum([FollowUpStatus.NeedsFollowUp, FollowUpStatus.Resolved, FollowUpStatus.NotApplicable]).optional(),
  quickReportLocationType: z.enum([QuickReportLocationType.NotRelatedToAPollingStation, QuickReportLocationType.OtherPollingStation, QuickReportLocationType.VisitedPollingStation]).optional()
});

export type FormSubmissionsSearchParams = z.infer<typeof FormSubmissionsSearchParamsSchema>;

export const QuickReportsSearchParamsSchema = z.object({
  level1Filter: z.string().catch('').optional(),
  level2Filter: z.string().catch('').optional(),
  level3Filter: z.string().catch('').optional(),
  level4Filter: z.string().catch('').optional(),
  level5Filter: z.string().catch('').optional(),
  followUpStatus: z.enum([FollowUpStatus.NeedsFollowUp, FollowUpStatus.Resolved, FollowUpStatus.NotApplicable]).optional(),
  quickReportLocationType: z.enum([QuickReportLocationType.NotRelatedToAPollingStation, QuickReportLocationType.OtherPollingStation, QuickReportLocationType.VisitedPollingStation]).optional(),
});

export type QuickReportsSearchParams = z.infer<typeof QuickReportsSearchParamsSchema>;



export const CitizenReportsSearchParamsSchema = z.object({
  followUpStatus: z.enum([FollowUpStatus.NeedsFollowUp, FollowUpStatus.Resolved, FollowUpStatus.NotApplicable]).optional(),
});

export type CitizenReportsSearchParams = z.infer<typeof CitizenReportsSearchParamsSchema>;
