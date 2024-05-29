import { VisibilityState } from '@tanstack/react-table';
import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import {
  FilterBy,
  formSubmissionsDefaultColumns,
  quickReportsDefaultColumns,
} from '../utils/column-visibility-options';

type ColumnVisibilityStore = {
  columns: Record<FilterBy, VisibilityState>;
  toggleColumn: (byFilter: FilterBy, id: string, checked: boolean) => void;
};

const useColumnVisibilityStore = create(
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

export const useColumnsVisibility = () => useColumnVisibilityStore((state) => state.columns);

export const useByEntryColumns = () => useColumnVisibilityStore((state) => state.columns.byEntry);
export const useByFormColumns = () => useColumnVisibilityStore((state) => state.columns.byForm);
export const useByObserverColumns = () => useColumnVisibilityStore((state) => state.columns.byObserver);

export const useToggleColumn = () => useColumnVisibilityStore((state) => state.toggleColumn);

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
