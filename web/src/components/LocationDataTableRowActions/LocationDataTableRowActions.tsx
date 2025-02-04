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

import { Location } from '@/common/types';
import { Dialog, DialogContent, DialogTrigger } from '@/components/ui/dialog';
import { MoreHorizontal, Pencil, Trash2 } from 'lucide-react';
import EditLocationModal from '../EditLocationModal/EditLocationModal';
import { useConfirm } from '../ui/alert-dialog-provider';

interface DataTableRowActionsProps {
  location: Location;
  updateLocation: (location: Location) => void;
  deleteLocation: (location: Location) => void;
}

export function LocationDataTableRowActions({
  location,
  deleteLocation,
  updateLocation,
}: DataTableRowActionsProps) {
  const [dialogContent, setDialogContent] = React.useState<React.ReactNode | null>(null);
  const confirm = useConfirm();

  const handleEditClick = React.useCallback(() => {
    setDialogContent(
      <EditLocationModal location={location} updateLocation={updateLocation} />
    );
  }, [location, updateLocation]);

  const handleDeleteClick = React.useCallback(async () => {
    if (
      await confirm({
        title: 'Delete location',
        body: 'Are you sure you want to delete this location?',
      })
    ) {
      deleteLocation(location);
    }
  }, [location, deleteLocation, confirm]);

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
          <DropdownMenuItem onSelect={handleDeleteClick} className='text-red-600'>
            <Trash2 className='mr-2 h-4 w-4' />
            Delete
          </DropdownMenuItem>
          <DropdownMenuSeparator />
        </DropdownMenuContent>
      </DropdownMenu>
      {dialogContent && <DialogContent>{dialogContent}</DialogContent>}
    </Dialog>
  );
}
