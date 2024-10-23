/* eslint-disable unicorn/prefer-top-level-await */
import { FormSubmissionFollowUpStatus, QuestionsAnswered, QuickReportFollowUpStatus, ZFormType } from '@/common/types';
import { z } from 'zod';
import { MonitoringObserverStatus } from './monitoring-observer';
import { IncidentCategory } from '@/features/responses/models/quick-report';

export const PushMessageTargetedObserversSearchParamsSchema = z.object({
  searchText: z.string().catch('').optional(),
  statusFilter: z.nativeEnum(MonitoringObserverStatus).optional(),
  level1Filter: z.string().catch('').optional(),
  level2Filter: z.string().catch('').optional(),
  level3Filter: z.string().catch('').optional(),
  level4Filter: z.string().catch('').optional(),
  level5Filter: z.string().catch('').optional(),
  pollingStationNumberFilter: z.string().catch('').optional(),
  sortColumnName: z.string().catch('observerName').optional(),
  tagsFilter: z.array(z.string()).optional().catch([]).optional(),
  pageSize: z.number().catch(25).default(25).optional(),
  submissionsFromDate: z.coerce.date().optional(),
  submissionsToDate: z.coerce.date().optional(),
  questionsAnswered: z.nativeEnum(QuestionsAnswered).optional(),
  formId: z.string().optional(),
  formTypeFilter: ZFormType.optional(),
  formIsCompleted: z.string().optional(),
  followUpStatus: z.nativeEnum(FormSubmissionFollowUpStatus).optional(),
  hasFlaggedAnswers: z.string().catch('').optional(),
  monitoringObserverId: z.string().catch('').optional(),
  hasNotes: z.string().catch('').optional(),
  hasAttachments: z.string().catch('').optional(),
  monitoringObserverStatus: z.nativeEnum(MonitoringObserverStatus).optional(),
  hasQuickReports: z.string().catch('').optional(),
  incidentCategory: z.nativeEnum(IncidentCategory).optional(),
  quickReportFollowUpStatus: z.nativeEnum(QuickReportFollowUpStatus).optional(),
});

export type PushMessageTargetedObserversSearchParams = z.infer<typeof PushMessageTargetedObserversSearchParamsSchema>;
