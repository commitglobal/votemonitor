

import { Row } from '@tanstack/react-table';
import * as React from 'react';

import { Button } from '@/components/ui/button';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuLabel,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';

import { Dialog, DialogContent, DialogTrigger } from '@/components/ui/dialog';
import { MoreHorizontal, Pencil, Trash2 } from 'lucide-react';
import { ImportObserverRow } from './MonitoringObserversImport';
import DeleteDialog from './modals/delete-modal';
import EditDialog from './modals/edit-modal';

interface DataTableRowActionsProps {
  row: Row<ImportObserverRow>;
  updateObserver: (observer: ImportObserverRow) => void;
  deleteObserver: (observer: ImportObserverRow) => void;
}

export function ImportObserverDataTableRowActions({ row, deleteObserver, updateObserver }: DataTableRowActionsProps) {
  const [dialogContent, setDialogContent] = React.useState<React.ReactNode | null>(null);
  const [showDeleteDialog, setShowDeleteDialog] = React.useState<boolean>(false);

  const handleEditClick = () => {
    setDialogContent(<EditDialog observer={row.original} updateObserver={updateObserver} />);
  };

  return (
    <Dialog>
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button variant='ghost' className='flex h-8 w-8 p-0 data-[state=open]:bg-muted'>
            <MoreHorizontal className='h-4 w-4' />
            <span className='sr-only'>Open menu</span>
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent align='end' className='w-[200px]'>
          <DropdownMenuLabel>Actions</DropdownMenuLabel>
          <DropdownMenuSeparator />

          <DialogTrigger asChild onClick={handleEditClick}>
            <DropdownMenuItem>
              <Pencil className='mr-2 h-4 w-4' />
              Edit
            </DropdownMenuItem>
          </DialogTrigger>
          <DropdownMenuItem onSelect={() => setShowDeleteDialog(true)} className='text-red-600'>
            <Trash2 className='mr-2 h-4 w-4' />
            Delete
          </DropdownMenuItem>
          <DropdownMenuSeparator />
        </DropdownMenuContent>
      </DropdownMenu>
      {dialogContent && <DialogContent>{dialogContent}</DialogContent>}
      <DeleteDialog
        observer={row.original}
        isOpen={showDeleteDialog}
        showActionToggle={setShowDeleteDialog}
        deleteObserver={deleteObserver}
      />
    </Dialog>
  );
}
