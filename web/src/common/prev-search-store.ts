import { create } from 'zustand';
import { persist } from 'zustand/middleware';

type PrevSearchStore = {
  search: Record<string, string | undefined | string[] | number>;
  setSearch: (prevSearch: Record<string, string | undefined | string[] | number>) => void;
};

const usePrevSearchStore = create(
  persist<PrevSearchStore>(
    (set) => ({
      search: {},
      setSearch: (search) => {
        set({ search });
      },
    }),
    { name: 'prev-search' }
  )
);

export const usePrevSearch = () => usePrevSearchStore((state) => state.search);
export const useSetPrevSearch = () => usePrevSearchStore((state) => state.setSearch);
