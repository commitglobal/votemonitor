import { DataSources } from './types';
import { create } from 'zustand';
import { persist } from 'zustand/middleware';

type DataSourceStore = {
  dataSource: DataSources;
  setDataSource: (dataSource: DataSources) => void;
};

const useDataSourceStore = create(
  persist<DataSourceStore>(
    (set) => ({
      dataSource: DataSources.Ngo,
      setDataSource: (dataSource: DataSources) => {
        set({ dataSource });
      },
    }),
    { name: 'data-source' }
  )
);

export const useDataSource = () => useDataSourceStore((state) => state.dataSource);
export const useSetDataSource = () => useDataSourceStore((state) => state.setDataSource);
