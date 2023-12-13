/* eslint-disable unicorn/prefer-top-level-await */
import type { ColumnDef } from '@tanstack/react-table';
import {z} from 'zod';

export interface Observer {
  id: string;
  name: string;
  login: string;
  status: string;
}

export const observerSearchSchema = z.object({
  NameFilter: z.string().catch(''),
  PageNumber: z.number().catch(1),
  PageSize: z.number().catch(10),
  Status: z.enum(['Active', 'Inactive']).catch('Active'),
})

export type ObserverSearch = z.infer<typeof observerSearchSchema>;

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
