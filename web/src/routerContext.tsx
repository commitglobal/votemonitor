import { QueryClient } from '@tanstack/react-query';
import { AuthContextType } from './context/auth.context';
import { CurrentElectionRoundStoreType } from './context/election-round.store';

export type RouterContext = {
  queryClient: QueryClient;
  authContext: AuthContextType;
  currentElectionRoundContext: CurrentElectionRoundStoreType;
};
