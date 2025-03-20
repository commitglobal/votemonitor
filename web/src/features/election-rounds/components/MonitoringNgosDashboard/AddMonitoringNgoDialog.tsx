import { DefaultSearchParamsSchema } from '@/common/zod-schemas';
import { Button } from '@/components/ui/button';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { useDebouncedSearch } from '@/hooks/debounced-search';
import { Route } from '@/routes/election-rounds/$electionRoundId/$tab';
import { ColumnDef } from '@tanstack/react-table';
import { Plus } from 'lucide-react';
import { MonitoringNgoModel } from '../../models/types';
import { useAvailableMonitoringNgos } from './queries';

export interface AddMonitoringNgoDialogProps {
  electionRoundId: string;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

function AddMonitoringNgoDialog({ open, onOpenChange, electionRoundId }: AddMonitoringNgoDialogProps) {
  const { searchText, handleSearchInput, queryParams } = useDebouncedSearch(Route.id, DefaultSearchParamsSchema);
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
          <Button title='Add' onClick={() => {}}>
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
