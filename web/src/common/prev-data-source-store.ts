import { DataSources } from './types';
import { create } from 'zustand';
import { persist } from 'zustand/middleware';

type PrevDataSourceStore = {
  dataSource: DataSources;
  setDataSource: (dataSource: DataSources) => void;
};

const usePrevDataSourceStore = create(
  persist<PrevDataSourceStore>(
    (set) => ({
      dataSource: DataSources.Ngo,
      setDataSource: (dataSource: DataSources) => {
        set({ dataSource });
      },
    }),
    { name: 'data-source' }
  )
);

export const usePrevDataSource = () => usePrevDataSourceStore((state) => state.dataSource);
export const useSetPrevDataSource = () => usePrevDataSourceStore((state) => state.setDataSource);
