import { VisibilityState } from '@tanstack/react-table';
import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import {
  citizenReportsDefaultColumns,
  FormSubmissionsViewBy,
  formSubmissionsDefaultColumns,
  quickReportsDefaultColumns,
} from '../utils/column-visibility-options';

type ColumnVisibilityStore = {
  columns: Record<FormSubmissionsViewBy, VisibilityState>;
  toggleColumn: (byFilter: FormSubmissionsViewBy, id: string, checked: boolean) => void;
};

const useFormSubmissionsColumnVisibilityStore = create(
  persist<ColumnVisibilityStore>(
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

export const useColumnsVisibility = () => useFormSubmissionsColumnVisibilityStore((state) => state.columns);

export const useFormSubmissionsByEntryColumns = () => useFormSubmissionsColumnVisibilityStore((state) => state.columns.byEntry);
export const useFormSubmissionsByFormColumns = () => useFormSubmissionsColumnVisibilityStore((state) => state.columns.byForm);
export const useFormSubmissionsByObserverColumns = () => useFormSubmissionsColumnVisibilityStore((state) => state.columns.byObserver);

export const useToggleColumn = () => useFormSubmissionsColumnVisibilityStore((state) => state.toggleColumn);

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

