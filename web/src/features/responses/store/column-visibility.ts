import { VisibilityState } from '@tanstack/react-table';
import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { FilterBy, formSubmissionsDefaultColumns } from '../utils/column-visibility-options';

type ColumnVisibilityState = {
  columns: Record<FilterBy, VisibilityState>;
  toggleColumn: (byFilter: FilterBy, id: string, checked: boolean) => void;
};

const useColumnVisibilityStore = create(
  persist<ColumnVisibilityState>(
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
