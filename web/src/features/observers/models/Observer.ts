/* eslint-disable unicorn/prefer-top-level-await */
import type { ColumnDef } from '@tanstack/react-table';
import {z} from 'zod';

export interface Observer {
  id: string;
  name: string;
  login: string;
  status: string;
}

export const observerRouteSearchSchema = z.object({
  nameFilter: z.string().catch(''),
  pageNumber: z.number().catch(1),
  pageSize: z.number().catch(10),
  status: z.enum(['Active', 'Inactive']).catch('Active'),
})

export const observerColDefs: ColumnDef<Observer>[] = [
  {
    header: 'Name',
    accessorKey: 'name',
  },
  {
    header: 'Login',
    accessorKey: 'login',
  },
  {
    header: 'Status',
    accessorKey: 'status',
  },
];
