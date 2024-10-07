import { create } from 'zustand';

import { persist } from 'zustand/middleware';

interface DashboardExpandedChartsStore {
  expandedCharts: Set<string>;
  toggleChart: (value: string) => void;
  clearSet: () => void;
}

const useDashboardExpandedChartsStore = create<DashboardExpandedChartsStore>()(
  persist(
    (set) => ({
      expandedCharts: new Set(),
      toggleChart: (chartId) =>
        set((state) => {
          const newExpandedCards = new Set(state.expandedCharts);
          if (newExpandedCards.has(chartId)) {
            newExpandedCards.delete(chartId);
          } else {
            newExpandedCards.add(chartId);
          }

          return {
            expandedCharts: newExpandedCards,
          };
        }),
      clearSet: () => set(() => ({ expandedCharts: new Set() })),
    }),
    {
      name: 'dashboard-expanded-charts-storage',
      partialize: (state) => ({
        stringSet: Array.from(state.expandedCharts), // Convert Set to array for storage
      }),
      merge: (persistedState, currentState) => ({
        ...currentState,
        expandedCharts: new Set((persistedState as DashboardExpandedChartsStore)?.expandedCharts || []), // Rehydrate set from stored array
      }),
    }
  )
);

export default useDashboardExpandedChartsStore;
