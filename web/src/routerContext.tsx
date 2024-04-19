import { QueryClient } from '@tanstack/react-query';
import { AuthContextType } from './context/auth.context';

export type RouterContext = {
  queryClient: QueryClient;
  authContext: AuthContextType;
};
