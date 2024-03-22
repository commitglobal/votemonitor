import { authApi } from '@/common/auth-api';
import { DataTableParameters, PageResponse } from '@/common/types';
import Layout from '@/components/layout/Layout';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { Button } from '@/components/ui/button';
import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog';
import { ElectionRound, electionRoundColDefs } from '@/features/election-round/models/ElectionRound';
import { PlusIcon } from '@heroicons/react/24/outline';
import { UseQueryResult, useQuery } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';
import { useTranslation } from 'react-i18next';

export const Route = createFileRoute('/election-rounds/')({
  component: ElectionRounds,
});

function useElectionRounds(p: DataTableParameters): UseQueryResult<PageResponse<ElectionRound>, Error> {
  return useQuery({
    queryKey: ['electionRounds', p.pageNumber, p.pageSize, p.sortColumnName, p.sortOrder],
    queryFn: async () => {
      const response = await authApi.get<PageResponse<ElectionRound>>('/election-rounds', {
        params: {
          PageNumber: p.pageNumber,
          PageSize: p.pageSize,
          SortColumnName: p.sortColumnName,
          SortOrder: p.sortOrder,
        },
      });

      if (response.status !== 200) {
        throw new Error('Failed to fetch electionRounds');
      }

      return response.data;
    },
  });
}

const CreateElectionRoundButton = () => {
  const { t } = useTranslation();

  return (
    <Dialog>
      <DialogTrigger asChild>
        <Button variant='default'>
          <PlusIcon className='w-5 h-5 mr-2 -ml-1.5' />
          <span>
            {t('election-round.action.create')}
          </span>
        </Button>
      </DialogTrigger>
      <DialogContent className='sm:max-w-[475px]'>
        <DialogHeader>
          <DialogTitle>Save preset</DialogTitle>
          <DialogDescription>
            This will save the current playground state as a preset which you can access later or share with others.
          </DialogDescription>
        </DialogHeader>
        <div className='grid gap-4 py-4'>
          <div className='grid gap-2'>
            <label htmlFor='name'>Name</label>
            <input id='name' autoFocus />
          </div>
          <div className='grid gap-2'>
            <label htmlFor='description'>Description</label>
            <input id='description' />
          </div>
        </div>
        <DialogFooter>
          <Button type='submit'>Save</Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

function ElectionRounds() {
  return (
    <Layout title={'Election Rounds'} actions={<CreateElectionRoundButton />}>
      <QueryParamsDataTable columns={electionRoundColDefs} useQuery={useElectionRounds} />
    </Layout>
  );
}
