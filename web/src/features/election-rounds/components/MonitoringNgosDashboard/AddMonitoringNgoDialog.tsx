import { Button } from '@/components/ui/button';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import API from '@/services/api';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { ColumnDef } from '@tanstack/react-table';
import { useDebounce } from '@uidotdev/usehooks';
import { Plus } from 'lucide-react';
import { ChangeEvent, useMemo, useState } from 'react';
import { toast } from 'sonner';
import { MonitoringNgoModel } from '../../models/types';
import { monitoringNgoKeys, useAvailableMonitoringNgos } from './queries';

export interface AddMonitoringNgoDialogProps {
  electionRoundId: string;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

const useMonitoringNgoSearch = () => {
  const [searchText, setSearchText] = useState('');
  const debouncedSearchText = useDebounce(searchText, 300);

  const handleSearchInput = (ev: ChangeEvent<HTMLInputElement>): void => {
    setSearchText(ev.currentTarget.value);
  };

  const queryParams = useMemo(() => {
    const params = [['searchText', debouncedSearchText]].filter(([_, value]) => value);

    return Object.fromEntries(params);
  }, [debouncedSearchText]);

  return { searchText, queryParams, handleSearchInput };
};

function AddMonitoringNgoDialog({ open, onOpenChange, electionRoundId }: AddMonitoringNgoDialogProps) {
  const { searchText, handleSearchInput, queryParams } = useMonitoringNgoSearch();
  const queryClient = useQueryClient();
  const addMonitoringNgoMutation = useMutation({
    mutationFn: async (ngoId: string) => {
      return await API.post(`election-rounds/${electionRoundId}/monitoring-ngos`, { ngoId });
    },

    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: monitoringNgoKeys.all(electionRoundId) });
      onOpenChange(false);
      toast.success('Added monitoring NGO');
    },
    //TODO Add error handling
  });

  const monitoringNgosColDefs: ColumnDef<MonitoringNgoModel>[] = [
    {
      accessorKey: 'name',
      enableSorting: true,
      header: ({ column }) => <DataTableColumnHeader title='Name' column={column} />,
    },

    {
      id: 'actions',
      cell: ({ row }) => {
        const ngoId = row.original.id;

        return (
          <Button
            title='Add'
            onClick={() => {
              addMonitoringNgoMutation.mutate(ngoId);
            }}>
            <Plus className='mr-2' width={18} height={18} />
            Add
          </Button>
        );
      },
    },
  ];

  return (
    <Dialog open={open} onOpenChange={onOpenChange} modal={true}>
      <DialogContent
        className='min-w-[450px] max-h-[950px]'
        onInteractOutside={(e) => {
          e.preventDefault();
        }}
        onEscapeKeyDown={(e) => {
          e.preventDefault();
        }}>
        <DialogHeader>
          <DialogTitle className='mb-3.5'>Add monitoring NGO</DialogTitle>
          <Input value={searchText} onChange={handleSearchInput} className='max-w-md' placeholder='Search' />
        </DialogHeader>
        <div className='flex flex-col gap-3 overflow-auto h-[650px]'>
          <QueryParamsDataTable
            columns={monitoringNgosColDefs}
            useQuery={(params) => useAvailableMonitoringNgos(electionRoundId, params)}
            queryParams={queryParams}
          />
        </div>
      </DialogContent>
    </Dialog>
  );
}

export default AddMonitoringNgoDialog;
