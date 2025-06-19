import { QueryClient } from '@tanstack/react-query';
import { CurrentElectionRoundStoreType } from './context/election-round.store';
import { AuthContextType } from './context/auth-context';

export type RouterContext = {
  queryClient: QueryClient;
  authContext: AuthContextType;
  currentElectionRoundContext: CurrentElectionRoundStoreType;
};
