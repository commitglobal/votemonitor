import { CitizenReportFollowUpStatus, FormSubmissionFollowUpStatus, IncidentReportFollowUpStatus, QuestionsAnswered, QuickReportFollowUpStatus } from '@/common/types';
import { QuickReportLocationType } from '../models/quick-report';
import { IncidentReportLocationType } from '../models/incident-report';

export function mapQuickReportLocationType(locationType: QuickReportLocationType): string {
  if (locationType === QuickReportLocationType.NotRelatedToAPollingStation) return 'Not Related To A Polling Station';
  if (locationType === QuickReportLocationType.OtherPollingStation) return 'Other Polling Station';
  if (locationType === QuickReportLocationType.VisitedPollingStation) return 'Visited Polling Station';

  return 'Unknown';
}

export function mapFormSubmissionFollowUpStatus(followUpStatus: FormSubmissionFollowUpStatus): string {
  if (followUpStatus === FormSubmissionFollowUpStatus.NotApplicable) return 'Not Applicable';
  if (followUpStatus === FormSubmissionFollowUpStatus.NeedsFollowUp) return 'Needs Follow-up';
  if (followUpStatus === FormSubmissionFollowUpStatus.Resolved) return 'Resolved';

  return 'Unknown';
}

export function mapQuickReportFollowUpStatus(followUpStatus: QuickReportFollowUpStatus): string {
  if (followUpStatus === QuickReportFollowUpStatus.NotApplicable) return 'Not Applicable';
  if (followUpStatus === QuickReportFollowUpStatus.NeedsFollowUp) return 'Needs Follow-up';
  if (followUpStatus === QuickReportFollowUpStatus.Resolved) return 'Resolved';

  return 'Unknown';
}

export function mapCitizenReportFollowUpStatus(followUpStatus: CitizenReportFollowUpStatus): string {
  if (followUpStatus === CitizenReportFollowUpStatus.NotApplicable) return 'Not Applicable';
  if (followUpStatus === CitizenReportFollowUpStatus.NeedsFollowUp) return 'Needs Follow-up';
  if (followUpStatus === CitizenReportFollowUpStatus.Resolved) return 'Resolved';

  return 'Unknown';
}

export function mapIncidentReportFollowUpStatus(followUpStatus: IncidentReportFollowUpStatus): string {
  if (followUpStatus === IncidentReportFollowUpStatus.NotApplicable) return 'Not Applicable';
  if (followUpStatus === IncidentReportFollowUpStatus.NeedsFollowUp) return 'Needs Follow-up';
  if (followUpStatus === IncidentReportFollowUpStatus.Resolved) return 'Resolved';

  return 'Unknown';
}

export function mapIncidentReportLocationType(locationType: IncidentReportLocationType): string {
  if (locationType === IncidentReportLocationType.PollingStation) return 'Polling station';
  if (locationType === IncidentReportLocationType.OtherLocation) return 'Other locations';

  return 'Unknown';
}

export function mapQuestionsAnswered(questionsAnswered: QuestionsAnswered): string {
  if (questionsAnswered === QuestionsAnswered.None) return 'None';
  if (questionsAnswered === QuestionsAnswered.Some) return 'Some';
  if (questionsAnswered === QuestionsAnswered.All) return 'All';

  return 'Unknown';
}
