import { authApi } from '@/common/auth-api';
import { DataTableParameters, PageResponse, SortOrder } from '@/common/types';
import Layout from '@/components/layout/Layout';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { Button } from '@/components/ui/button';
import { FormTemplate } from '@/features/formsTemplate/models/formTemplate';
import { EllipsisVerticalIcon } from '@heroicons/react/24/outline';
import { UseQueryResult, useQuery } from '@tanstack/react-query';
import { createFileRoute, useNavigate } from '@tanstack/react-router'
import { ColumnDef } from '@tanstack/react-table';
import { z } from 'zod';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';

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

function useFormTemplates(p: DataTableParameters): UseQueryResult<PageResponse<FormTemplate>, Error> {
  return useQuery({
    queryKey: ['form-templates', p.pageNumber, p.pageSize, p.sortColumnName, p.sortOrder],
    queryFn: async () => {
      const response = await authApi.get<PageResponse<FormTemplate>>('/form-templates', {
        params: {
          PageNumber: p.pageNumber,
          PageSize: p.pageSize,
          SortColumnName: p.sortColumnName,
          SortOrder: p.sortOrder,
        },
      });

      if (response.status !== 200) {
        throw new Error('Failed to fetch form templates');
      }

      return response.data;
    },
  });
}

export const formTemplateColumnDefs: ColumnDef<FormTemplate>[] = [
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
    id: 'name',
    accessorFn: (row, _) => row.name[row.defaultLanguage],
    enableSorting: false,
    header: ({ column }) => <DataTableColumnHeader title='Title' column={column} />,
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
    cell:({ row }) => row.original.languages.join(", ")
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
              <DropdownMenuItem onClick={() =>  navigate({ to: '/ngos/$ngoId', params: { ngoId: row.original.id } })}>Edit</DropdownMenuItem>
              <DropdownMenuItem>Deactivate</DropdownMenuItem>
              <DropdownMenuItem>Delete</DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      );
    },
  },
];



function FormTemplatesList(){
  return (
    <Layout title={'Form templates'}>
      <QueryParamsDataTable columns={formTemplateColumnDefs} useQuery={useFormTemplates} />
    </Layout>
  );
}