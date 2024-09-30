import { VisibilityState } from '@tanstack/react-table';
import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import {
  citizenReportsDefaultColumns,
  FormSubmissionsViewBy,
  formSubmissionsDefaultColumns,
  quickReportsDefaultColumns,
  IssueReportsViewBy,
  issueReportsDefaultColumns,
} from '../utils/column-visibility-options';

type FormSubmissionsColumnVisibilityStore = {
  columns: Record<FormSubmissionsViewBy, VisibilityState>;
  toggleColumn: (byFilter: FormSubmissionsViewBy, id: string, checked: boolean) => void;
};

const useFormSubmissionsColumnVisibilityStore = create(
  persist<FormSubmissionsColumnVisibilityStore>(
    (set) => ({
      columns: formSubmissionsDefaultColumns,
      toggleColumn: (byFilter, id, checked) => {
        set((state) => ({
          columns: {
            ...state.columns,
            [byFilter]: {
              ...state.columns[byFilter],
              [id]: checked,
            },
          },
        }));
      },
    }),
    { name: 'responses-column-visibility' }
  )
);

export const useFormSubmissionsColumnsVisibility = () => useFormSubmissionsColumnVisibilityStore((state) => state.columns);

export const useFormSubmissionsByEntryColumns = () => useFormSubmissionsColumnVisibilityStore((state) => state.columns.byEntry);
export const useFormSubmissionsByFormColumns = () => useFormSubmissionsColumnVisibilityStore((state) => state.columns.byForm);
export const useFormSubmissionsByObserverColumns = () => useFormSubmissionsColumnVisibilityStore((state) => state.columns.byObserver);

export const useFormSubmissionsToggleColumn = () => useFormSubmissionsColumnVisibilityStore((state) => state.toggleColumn);

type QuickReportsColumnVisibilityStore = {
  columns: VisibilityState;
  toggleColumn: (id: string, checked: boolean) => void;
};

const useQuickReportsColumnVisibilityStore = create(
  persist<QuickReportsColumnVisibilityStore>(
    (set) => ({
      columns: quickReportsDefaultColumns,
      toggleColumn: (id, checked) => {
        set((state) => ({
          columns: {
            ...state.columns,
            [id]: checked,
          },
        }));
      },
    }),
    { name: 'quick-reports-column-visibility' }
  )
);

export const useQuickReportsColumnsVisibility = () => useQuickReportsColumnVisibilityStore((state) => state.columns);
export const useQuickReportsToggleColumn = () => useQuickReportsColumnVisibilityStore((state) => state.toggleColumn);


type CitizenReportsColumnVisibilityStore = {
  columns: VisibilityState;
  toggleColumn: (id: string, checked: boolean) => void;
};

const useCitizenReportsColumnVisibilityStore = create(
  persist<CitizenReportsColumnVisibilityStore>(
    (set) => ({
      columns: citizenReportsDefaultColumns,
      toggleColumn: (id, checked) => {
        set((state) => ({
          columns: {
            ...state.columns,
            [id]: checked,
          },
        }));
      },
    }),
    { name: 'citizen-reports-column-visibility' }
  )
);

export const useCitizenReportsColumnsVisibility = () => useCitizenReportsColumnVisibilityStore((state) => state.columns);
export const useCitizenReportsToggleColumn = () => useCitizenReportsColumnVisibilityStore((state) => state.toggleColumn);


type IssueReportsColumnVisibilityStore = {
  columns: Record<IssueReportsViewBy, VisibilityState>;
  toggleColumn: (byFilter: IssueReportsViewBy, id: string, checked: boolean) => void;
};

const useIssueReportsColumnVisibilityStore = create(
  persist<IssueReportsColumnVisibilityStore>(
    (set) => ({
      columns:issueReportsDefaultColumns,
      toggleColumn: (byFilter, id, checked) => {
        set((state) => ({
          columns: {
            ...state.columns,
            [byFilter]: {
              ...state.columns[byFilter],
              [id]: checked,
            },
          },
        }));
      },
    }),
    { name: 'issue-reports-column-visibility' }
  )
);

export const useIssueReportsColumnsVisibility = () => useIssueReportsColumnVisibilityStore((state) => state.columns);

export const useIssueReportsByEntryColumns = () => useIssueReportsColumnVisibilityStore((state) => state.columns.byEntry);
export const useIssueReportsByFormColumns = () => useIssueReportsColumnVisibilityStore((state) => state.columns.byForm);
export const useIssueReportsByObserverColumns = () => useIssueReportsColumnVisibilityStore((state) => state.columns.byObserver);

export const useIssueReportsToggleColumn = () => useIssueReportsColumnVisibilityStore((state) => state.toggleColumn);
