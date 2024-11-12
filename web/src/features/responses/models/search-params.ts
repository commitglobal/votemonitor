/* eslint-disable unicorn/prefer-top-level-await */
import {
  CitizenReportFollowUpStatus,
  FormSubmissionFollowUpStatus,
  IncidentReportFollowUpStatus,
  QuestionsAnswered,
  QuickReportFollowUpStatus,
} from '@/common/types';
import { z } from 'zod';
import { IncidentCategory, QuickReportLocationType } from './quick-report';
import { IncidentReportLocationType } from './incident-report';
import { ZDataSourceSearchSchema } from '@/routes';

export const ResponsesPageSearchParamsSchema = z.object({
  viewBy: z.enum(['byEntry', 'byObserver', 'byForm']).catch('byEntry').default('byEntry'),
  tab: z
    .enum(['form-answers', 'quick-reports', 'citizen-reports', 'incident-reports'])
    .catch('form-answers')
    .optional(),
});

export const FormSubmissionsSearchParamsSchema = ResponsesPageSearchParamsSchema.merge(
  z.object({
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
    followUpStatus: z.nativeEnum(FormSubmissionFollowUpStatus).optional(),
    quickReportFollowUpStatus: z.nativeEnum(QuickReportFollowUpStatus).optional(),
    citizenReportFollowUpStatus: z.nativeEnum(CitizenReportFollowUpStatus).optional(),
    incidentReportFollowUpStatus: z.nativeEnum(IncidentReportFollowUpStatus).optional(),

    quickReportLocationType: z.nativeEnum(QuickReportLocationType).optional(),
    incidentCategory: z.nativeEnum(IncidentCategory).optional(),

    incidentReportLocationType: z.nativeEnum(IncidentReportLocationType).optional(),
    questionsAnswered: z.nativeEnum(QuestionsAnswered).optional(),
    hasNotes: z.string().catch('').optional(),
    hasAttachments: z.string().catch('').optional(),
    formIsCompleted: z.string().catch('').optional(),
    formId: z.string().optional(),

    submissionsFromDate: z.coerce.date().optional(),
    submissionsToDate: z.coerce.date().optional(),
  })
).merge(ZDataSourceSearchSchema);

export type FormSubmissionsSearchParams = z.infer<typeof FormSubmissionsSearchParamsSchema>;

export const QuickReportsSearchParamsSchema = ZDataSourceSearchSchema.merge(
  z.object({
    level1Filter: z.string().catch('').optional(),
    level2Filter: z.string().catch('').optional(),
    level3Filter: z.string().catch('').optional(),
    level4Filter: z.string().catch('').optional(),
    level5Filter: z.string().catch('').optional(),
    pollingStationNumberFilter: z.string().catch('').optional(),
    quickReportFollowUpStatus: z.nativeEnum(QuickReportFollowUpStatus).optional(),
    quickReportLocationType: z.nativeEnum(QuickReportLocationType).optional(),
    incidentCategory: z.nativeEnum(IncidentCategory).optional(),
  })
);

export type QuickReportsSearchParams = z.infer<typeof QuickReportsSearchParamsSchema>;

export const CitizenReportsSearchParamsSchema = ZDataSourceSearchSchema.merge(
  z.object({
    citizenReportFollowUpStatus: z.nativeEnum(CitizenReportFollowUpStatus).optional(),
  })
);

export type CitizenReportsSearchParams = z.infer<typeof CitizenReportsSearchParamsSchema>;

export const IncidentReportsSearchParamsSchema = ZDataSourceSearchSchema.merge(
  z.object({
    viewBy: z.enum(['byEntry', 'byObserver', 'byForm']).catch('byEntry').default('byEntry'),
    tab: z
      .enum(['form-answers', 'quick-reports', 'citizen-reports', 'incident-reports'])
      .catch('form-answers')
      .optional(),
    searchText: z.string().catch('').optional(),
    level1Filter: z.string().catch('').optional(),
    level2Filter: z.string().catch('').optional(),
    level3Filter: z.string().catch('').optional(),
    level4Filter: z.string().catch('').optional(),
    level5Filter: z.string().catch('').optional(),
    pollingStationNumberFilter: z.string().catch('').optional(),
    hasFlaggedAnswers: z.string().catch('').optional(),
    monitoringObserverId: z.string().catch('').optional(),
    tagsFilter: z.array(z.string()).optional().catch([]).optional(),
    incidentReportFollowUpStatus: z.nativeEnum(IncidentReportLocationType).optional(),
    incidentReportLocationType: z.nativeEnum(IncidentReportLocationType).optional(),
    questionsAnswered: z.nativeEnum(QuestionsAnswered).optional(),
    hasNotes: z.string().catch('').optional(),
    hasAttachments: z.string().catch('').optional(),
  })
);

export type IncidentReportsSearchParams = z.infer<typeof IncidentReportsSearchParamsSchema>;
