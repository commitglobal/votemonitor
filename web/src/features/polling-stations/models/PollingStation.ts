import type { ColumnDef } from '@tanstack/react-table';

export interface PollingStation {
  id: string;
  address: string;
  displayOrder: number;
  tags: {
    county: string;
    locality: string;
    sectionNumber: number;
    sectionName: string;
  };
}

export const pollingStationColDefs: ColumnDef<PollingStation>[] = [
  {
    header: 'Address',
    accessorKey: 'address',
  },
  {
    header: 'County',
    accessorKey: 'tags.county',
  },
  {
    header: 'Locality',
    accessorKey: 'tags.locality',
  },
  {
    header: 'Section Number',
    accessorKey: 'tags.sectionNumber',
  },
  {
    header: 'Section Name',
    accessorKey: 'tags.sectionName',
  },
];
