import API from '@/services/api'
import type { DataSource } from '@/types/common'

export interface TimestampsFilterOptions {
  firstSubmissionTimestamp: string
  lastSubmissionTimestamp: string
}

export interface FormFilterOption {
  formId: string
  formCode: string
  formName: string
}

export interface FormSubmissionsFilters {
  timestampsFilterOptions: TimestampsFilterOptions
  formFilterOptions: FormFilterOption[]
}

export const getFormSubmissionsFilters = async (
  electionRoundId: string,
  dataSource: DataSource
): Promise<FormSubmissionsFilters> => {
  return API.get<FormSubmissionsFilters>(
    `/election-rounds/${electionRoundId}/form-submissions:filters?dataSource=${dataSource}`
  ).then((res) => res.data)
}
