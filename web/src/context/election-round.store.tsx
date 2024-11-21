import { create } from 'zustand';
import { persist } from 'zustand/middleware';

import { createContext, PropsWithChildren, useContext, useId, useRef } from 'react';
import { useStoreWithEqualityFn } from "zustand/traditional";
// can't see how to properly type things without copy/pasting from zustand
import type { StoreApi } from "zustand";

export type ExtractState<S> = S extends {
  getState: () => infer T;
}
  ? T
  : never;
export type ReadonlyStoreApi<T> = Pick<StoreApi<T>, "getState" | "subscribe">;
export type WithReact<S extends ReadonlyStoreApi<unknown>> = S & {
  getServerState?: () => ExtractState<S>;
};

// not copied from zustand source
export type ZustandStore<T> = WithReact<StoreApi<T>>;


export type CurrentElectionRoundState = {
  currentElectionRoundId: string;
  setCurrentElectionRoundId(electionRoundId: string): void;

  isMonitoringNgoForCitizenReporting: boolean;
  setIsMonitoringNgoForCitizenReporting(isMonitoringNgoForCitizenReporting: boolean): void;

  isCoalitionLeader: boolean;
  setIsCoalitionLeader(isCoalitionLeader: boolean): void;
}

export type CurrentElectionRoundStoreType = ZustandStore<CurrentElectionRoundState>;

export const CurrentElectionRoundContext = createContext<CurrentElectionRoundStoreType>(null!);

export const CurrentElectionRoundStoreProvider = ({ children }: PropsWithChildren) => {
  const storeRef = useRef<CurrentElectionRoundStoreType>();

  if (!storeRef.current) {
    storeRef.current = create<CurrentElectionRoundState>()(
      persist(
        (set) => ({
          currentElectionRoundId: '',
          setCurrentElectionRoundId: (electionRoundId: string) => set({ currentElectionRoundId: electionRoundId }),
    
          isMonitoringNgoForCitizenReporting: false,
          setIsMonitoringNgoForCitizenReporting: (isMonitoringNgoForCitizenReporting: boolean) => set({ isMonitoringNgoForCitizenReporting }),
    
          isCoalitionLeader: false,
          setIsCoalitionLeader: (isCoalitionLeader: boolean) => set({ isCoalitionLeader }),
        }),
        {
          name: 'current-election-round', // name of the item in the storage (must be unique)
        }
      )
    );
  }
  return (
    <CurrentElectionRoundContext.Provider value={storeRef.current}>
      {children}
    </CurrentElectionRoundContext.Provider>
  );
};

export function useCurrentElectionRoundStore<U>(
  selector: (state: ExtractState<CurrentElectionRoundStoreType>) => U,
) {
  const store = useContext(CurrentElectionRoundContext);
  if (!store) throw "Missing StoreProvider";
  return useStoreWithEqualityFn(store, selector);
}
