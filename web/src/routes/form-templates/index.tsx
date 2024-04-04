import { SortOrder } from '@/common/types';
import Layout from '@/components/layout/Layout';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { Button } from '@/components/ui/button';
import { FormTemplateBase } from '@/features/formsTemplate/models/formTemplate';
import { EllipsisVerticalIcon } from '@heroicons/react/24/outline';
import { createFileRoute, useNavigate } from '@tanstack/react-router'
import { ColumnDef } from '@tanstack/react-table';
import { z } from 'zod';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { useFormTemplates } from '@/features/formsTemplate/queries';

const formTemplateRouteSearchSchema = z.object({
  nameFilter: z.string().catch(''),
  pageNumber: z.number().catch(1),
  pageSize: z.number().catch(10),
  sortColumnName: z.string().catch(''),
  sortOrder: z.enum([SortOrder.asc, SortOrder.desc]).catch(SortOrder.asc),
});


export const Route = createFileRoute('/form-templates/')({
  component: FormTemplatesList,
  validateSearch: formTemplateRouteSearchSchema
});

export const formTemplateColumnDefs: ColumnDef<FormTemplateBase>[] = [
  {
    header: 'ID',
    accessorKey: 'id',
  },
  {
    accessorKey: 'code',
    enableSorting: true,
    header: ({ column }) => <DataTableColumnHeader title='Code' column={column} />,
  },
  {
    accessorKey: 'type',
    enableSorting: false,
    header: ({ column }) => <DataTableColumnHeader title='Type' column={column} />,
  },
  {
    id: 'name',
    accessorFn: (row, _) => row.name[row.defaultLanguage],
    enableSorting: false,
    header: ({ column }) => <DataTableColumnHeader title='Name' column={column} />,
  },
  {
    accessorKey: 'defaultLanguage',
    enableSorting: false,
    header: ({ column }) => <DataTableColumnHeader title='Default Language' column={column} />,
  },
  {
    accessorKey: 'languages',
    enableSorting: false,
    header: ({ column }) => <DataTableColumnHeader title='Languages' column={column} />,
    cell: ({ row }) => row.original.languages.join(", ")
  },
  {
    id: 'actions',
    cell: ({ row }) => {

      const navigate = useNavigate();

      return (
        <div className='text-right'>
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button variant='ghost-primary' size='icon'>
                <span className='sr-only'>Actions</span>
                <EllipsisVerticalIcon className='w-6 h-6' />
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align='end'>
              <DropdownMenuItem onClick={() => navigate({ to: '/form-templates/$formTemplateId/edit', params: { formTemplateId: row.original.id } })}>Edit</DropdownMenuItem>
              <DropdownMenuItem>Deactivate</DropdownMenuItem>
              <DropdownMenuItem>Delete</DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      );
    },
  },
];


function FormTemplatesList() {
  return (
    <Layout title={'Form templates'}>
      <QueryParamsDataTable
        columns={formTemplateColumnDefs}
        useQuery={useFormTemplates}
      />
    </Layout>
  );
}