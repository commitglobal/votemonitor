import { archiveElectionRound } from '@/api/election-rounds/archive-election-round';
import { startElectionRound } from '@/api/election-rounds/start-election-round';
import { unarchiveElectionRound } from '@/api/election-rounds/unarchive-election-round';
import { unstartElectionRound } from '@/api/election-rounds/unstart-election-round';
import { queryClient } from '@/main';
import { useMutation } from '@tanstack/react-query';
import { electionRoundKeys } from './queries';

export function useArchiveElectionRound() {
  return useMutation({
    mutationFn: ({ electionRoundId }: { electionRoundId: string; onSuccess?: () => void; onError?: () => void }) => {
      return archiveElectionRound(electionRoundId);
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
      return unarchiveElectionRound(electionRoundId);
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
      return startElectionRound(electionRoundId);
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
      return unstartElectionRound(electionRoundId);
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
      return unstartElectionRound(electionRoundId);
    },

    onSuccess: (_, { onSuccess }) => {
      queryClient.invalidateQueries({ queryKey: electionRoundKeys.all });
      onSuccess?.();
    },

    onError: (_, { onError }) => onError?.(),
  });
}
