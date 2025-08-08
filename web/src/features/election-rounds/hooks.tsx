import API from '@/services/api';
import { queryClient } from '@/main';
import { useMutation } from '@tanstack/react-query';
import { electionRoundKeys } from './queries';

export function useArchiveElectionRound() {
  return useMutation({
    mutationFn: ({ electionRoundId }: { electionRoundId: string; onSuccess?: () => void; onError?: () => void }) => {
      return API.post<void>(`/election-rounds/${electionRoundId}:archive`);
    },

    onSuccess: (_, { onSuccess }) => {
      queryClient.invalidateQueries({ queryKey: electionRoundKeys.all });
      onSuccess?.();
    },

    onError: (_, { onError }) => onError?.(),
  });
}

export function useUnarchiveElectionRound() {
  return useMutation({
    mutationFn: ({ electionRoundId }: { electionRoundId: string; onSuccess?: () => void; onError?: () => void }) => {
      return API.post<void>(`/election-rounds/${electionRoundId}:unarchive`);
    },

    onSuccess: (_, { onSuccess }) => {
      queryClient.invalidateQueries({ queryKey: electionRoundKeys.all });
      onSuccess?.();
    },

    onError: (_, { onError }) => onError?.(),
  });
}

export function useStartElectionRound() {
  return useMutation({
    mutationFn: ({ electionRoundId }: { electionRoundId: string; onSuccess?: () => void; onError?: () => void }) => {
      return API.post<void>(`/election-rounds/${electionRoundId}:start`);
    },

    onSuccess: (_, { onSuccess }) => {
      queryClient.invalidateQueries({ queryKey: electionRoundKeys.all });
      onSuccess?.();
    },

    onError: (_, { onError }) => onError?.(),
  });
}

export function useUnstartElectionRound() {
  return useMutation({
    mutationFn: ({ electionRoundId }: { electionRoundId: string; onSuccess?: () => void; onError?: () => void }) => {
      return API.post<void>(`/election-rounds/${electionRoundId}:unstart`);
    },

    onSuccess: (_, { onSuccess }) => {
      queryClient.invalidateQueries({ queryKey: electionRoundKeys.all });
      onSuccess?.();
    },

    onError: (_, { onError }) => onError?.(),
  });
}

export function useDeleteElectionRound() {
  return useMutation({
    mutationFn: ({ electionRoundId }: { electionRoundId: string; onSuccess?: () => void; onError?: () => void }) => {
      return API.post<void>(`/election-rounds/${electionRoundId}:unstart`);
    },

    onSuccess: (_, { onSuccess }) => {
      queryClient.invalidateQueries({ queryKey: electionRoundKeys.all });
      onSuccess?.();
    },

    onError: (_, { onError }) => onError?.(),
  });
}
