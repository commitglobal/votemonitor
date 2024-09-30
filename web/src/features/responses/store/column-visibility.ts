import { VisibilityState } from '@tanstack/react-table';
import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import {
  citizenReportsDefaultColumns,
  FormSubmissionsViewBy,
  formSubmissionsDefaultColumns,
  quickReportsDefaultColumns,
  IncidentReportsViewBy,
  incidentReportsDefaultColumns,
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

type IncidentReportsColumnVisibilityStore = {
  columns: Record<IncidentReportsViewBy, VisibilityState>;
  toggleColumn: (byFilter: IncidentReportsViewBy, id: string, checked: boolean) => void;
};

const useIncidentReportsColumnVisibilityStore = create(
  persist<IncidentReportsColumnVisibilityStore>(
    (set) => ({
      columns:incidentReportsDefaultColumns,
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
    { name: 'incident-reports-column-visibility' }
  )
);

export const useIncidentReportsColumnsVisibility = () => useIncidentReportsColumnVisibilityStore((state) => state.columns);

export const useIncidentReportsByEntryColumns = () => useIncidentReportsColumnVisibilityStore((state) => state.columns.byEntry);
export const useIncidentReportsByFormColumns = () => useIncidentReportsColumnVisibilityStore((state) => state.columns.byForm);
export const useIncidentReportsByObserverColumns = () => useIncidentReportsColumnVisibilityStore((state) => state.columns.byObserver);

export const useIncidentReportsToggleColumn = () => useIncidentReportsColumnVisibilityStore((state) => state.toggleColumn);
