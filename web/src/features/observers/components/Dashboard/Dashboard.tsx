import Layout from '@/components/layout/Layout';
import PasswordSetterDialog from '@/components/PasswordSetterDialog/PasswordSetterDialog';
import { usePasswordSetterDialog } from '@/components/PasswordSetterDialog/usePasswordSetterDialog';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Input } from '@/components/ui/input';
import { Separator } from '@/components/ui/separator';
import { useDialog } from '@/components/ui/use-dialog';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { useDebouncedSearch } from '@/hooks/debounced-search';
import { observersRouteSearchSchema, Route } from '@/routes/observers';
import { Cog8ToothIcon, EllipsisVerticalIcon, FunnelIcon } from '@heroicons/react/24/outline';
import { useNavigate } from '@tanstack/react-router';
import { ColumnDef, createColumnHelper } from '@tanstack/react-table';
import { Plus } from 'lucide-react';
import { ReactElement } from 'react';
import { useObserverMutations, useObservers } from '../../hooks/observers-queries';
import { Observer, ObserverStatus } from '../../models/observer';
import CreateObserverDialog from '../CreateObserverDialog';
import { ObserversListFilters } from '../ObserversFiltering';

export default function ObserversDashboard(): ReactElement {
  const columnHelper = createColumnHelper<Observer>();
  const { deleteObserverWithConfirmation, toggleObserverStatus } = useObserverMutations();

  const observerColDefs: ColumnDef<Observer>[] = [
    columnHelper.display({
      id: 'firstName',
      enableSorting: true,
      header: ({ column }) => <DataTableColumnHeader title='First name' column={column} />,
      cell: ({ row }) => <div className='truncate'>{row.original.firstName}</div>,
    }),
    columnHelper.display({
      id: 'lastName',
      enableSorting: true,
      header: ({ column }) => <DataTableColumnHeader title='Last name' column={column} />,
      cell: ({ row }) => <div className='truncate'>{row.original.lastName}</div>,
    }),
    columnHelper.display({
      id: 'email',
      enableSorting: true,
      enableGlobalFilter: true,
      header: ({ column }) => <DataTableColumnHeader title='Email' column={column} />,
      cell: ({ row }) => <div className='truncate'>{row.original.email}</div>,
    }),

    columnHelper.display({
      id: 'phoneNumber',
      enableSorting: true,
      header: ({ column }) => <DataTableColumnHeader title='Phone number' column={column} />,
      cell: ({ row }) => (
        <div className='truncate'>{row.original.phoneNumber === '' ? 'N/A' : row.original.phoneNumber}</div>
      ),
    }),

    columnHelper.display({
      id: 'isAccountVerified',
      enableSorting: true,
      header: ({ column }) => <DataTableColumnHeader title='Is email verified' column={column} />,
      cell: ({ row }) => <div className='truncate'>{row.original.isAccountVerified ? 'Yes' : 'No'}</div>,
    }),

    columnHelper.display({
      id: 'monitoredElections',
      enableSorting: true,
      header: ({ column }) => <DataTableColumnHeader title='Elections monitored' column={column} />,
      cell: ({ row }) => <div className='truncate'>{row.original.monitoredElections?.length}</div>,
    }),
    {
      header: '',
      accessorKey: 'action',
      enableSorting: true,
      cell: ({ row }) => {
        const observerId = row.original.id;
        const displayName = row.original.firstName + ' ' + row.original.lastName;
        const isObserverActive = row.original.status === ObserverStatus.Active;
        return (
          <DropdownMenu>
            <DropdownMenuTrigger>
              <EllipsisVerticalIcon className='w-[24px] h-[24px] text-purple-600' />
            </DropdownMenuTrigger>
            <DropdownMenuContent>
              <DropdownMenuItem
                onClick={(e) => {
                  e.stopPropagation();
                  navigateToObserver({ observerId });
                }}>
                View
              </DropdownMenuItem>
              <DropdownMenuItem
                onClick={(e) => {
                  e.stopPropagation();
                  navigateToObserver({ observerId, isEditing: true });
                }}>
                Edit
              </DropdownMenuItem>
              <DropdownMenuItem
                onClick={(e) => {
                  e.stopPropagation();
                  toggleObserverStatus.mutate({ observerId, isObserverActive });
                }}>
                {isObserverActive ? 'Deactivate' : 'Activate'}
              </DropdownMenuItem>

              <DropdownMenuItem
                onClick={async (e) => {
                  e.stopPropagation();
                  handlePasswordSet({ userId: observerId, displayName });
                }}>
                Set password
              </DropdownMenuItem>

              <DropdownMenuItem
                className='text-red-600'
                onClick={(e) => {
                  e.stopPropagation();
                  deleteObserverWithConfirmation({
                    observerId: row.original.id,
                    name: displayName,
                  });
                }}>
                Delete
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        );
      },
    },
  ];

  const navigate = useNavigate();
  const createObserverDialog = useDialog();

  const { searchText, handleSearchInput, queryParams } = useDebouncedSearch(Route.id, observersRouteSearchSchema);
  const { isFilteringContainerVisible, toggleFilteringContainerVisibility } = useFilteringContainer();
  const { passwordSetterDialogProps, handlePasswordSet } = usePasswordSetterDialog();

  const navigateToObserver = ({ observerId, isEditing }: { observerId: string; isEditing?: boolean }) => {
    navigate({ to: `/observers/$observerId/${isEditing ? 'edit' : ''}`, params: { observerId } });
  };

  return (
    <Layout
      title={'Observers'}
      subtitle='View all observers you imported as an NGO admin and invite them to current election observation event.'>
      <Card className='w-full pt-0'>
        <CardHeader className='flex gap-2 flex-column'>
          <div className='flex flex-row items-center justify-between'>
            <CardTitle className='text-xl'>All observers</CardTitle>
            <div className='flex md:flex-row-reverse gap-4 table-actions'>
              <Button className='text-purple-900 bg-background hover:bg-purple-50 hover:text-purple-500'>
                <svg
                  className='mr-1.5'
                  xmlns='http://www.w3.org/2000/svg'
                  width='18'
                  height='18'
                  viewBox='0 0 18 18'
                  fill='none'>
                  <path
                    fillRule='evenodd'
                    clipRule='evenodd'
                    d='M12.1814 1.80462H12.4169C12.5338 1.80462 12.6462 1.85015 12.7302 1.93159L16.0632 5.16484C16.1505 5.24963 16.1998 5.36611 16.1998 5.48787V14.9709C16.1998 15.6774 15.6138 16.246 14.8992 16.246H4.90036C4.1858 16.246 3.59975 15.6775 3.59975 14.9709V14.3914H4.49975V14.9699C4.49975 15.1733 4.67689 15.3451 4.90036 15.3451H14.8992C15.1227 15.3451 15.2998 15.1733 15.2998 14.9699V6.05032H13.5213C12.5161 6.05032 11.6998 5.23071 11.6998 4.22332V2.70367H4.90046C4.67699 2.70367 4.49986 2.87547 4.49986 3.07885V5.4034H3.59986V3.07975C3.59986 2.37318 4.18591 1.80463 4.90046 1.80463H12.1246C12.1435 1.8034 12.1625 1.8034 12.1814 1.80463L12.1814 1.80462ZM12.5998 6.29982C13.5939 6.29982 14.3998 7.10573 14.3998 8.09982V11.6998C14.3998 12.6939 13.5939 13.4998 12.5998 13.4998H3.5998C2.60571 13.4998 1.7998 12.6939 1.7998 11.6998V8.09982C1.7998 7.10573 2.60571 6.29982 3.5998 6.29982H12.5998ZM12.5998 7.19982H3.5998C3.10275 7.19982 2.6998 7.60277 2.6998 8.09982V11.6998C2.6998 12.1969 3.10275 12.5998 3.5998 12.5998H12.5998C13.0969 12.5998 13.4998 12.1969 13.4998 11.6998V8.09982C13.4998 7.60277 13.0969 7.19982 12.5998 7.19982ZM5.27545 8.09982C5.55828 8.09982 5.81205 8.15666 6.03547 8.27079C6.25813 8.38446 6.43057 8.52662 6.55156 8.69741C6.67373 8.87003 6.73648 9.03743 6.73648 9.19938C6.73648 9.30069 6.70004 9.39174 6.62955 9.46809C6.5573 9.54637 6.46619 9.58721 6.36219 9.58721C6.25362 9.58721 6.16566 9.55944 6.10274 9.49979C6.06805 9.46698 6.03283 9.42139 5.99633 9.3628L5.94043 9.26501C5.85992 9.11366 5.76705 9.00275 5.66253 8.93085C5.56122 8.86118 5.43454 8.82585 5.27996 8.82585C5.03369 8.82585 4.84226 8.91708 4.6973 9.10277C4.54865 9.29325 4.47261 9.57017 4.47261 9.93591C4.47261 10.1815 4.50665 10.3832 4.57345 10.5412C4.63843 10.6949 4.72843 10.807 4.84369 10.8803C4.96023 10.9545 5.0974 10.9918 5.25736 10.9918C5.43079 10.9918 5.57417 10.9498 5.6903 10.8664C5.80708 10.7827 5.89621 10.6581 5.95655 10.4934C5.98831 10.3963 6.0285 10.3155 6.07784 10.2512C6.14007 10.1701 6.23645 10.131 6.35768 10.131C6.46262 10.131 6.55526 10.1686 6.6305 10.2419C6.70661 10.316 6.74552 10.4102 6.74552 10.5189C6.74552 10.638 6.71669 10.7647 6.65997 10.8987C6.60325 11.0326 6.51513 11.1626 6.39618 11.2888C6.27525 11.417 6.12185 11.5206 5.93681 11.5994C5.75094 11.6786 5.53638 11.7178 5.2935 11.7178C5.11175 11.7178 4.94558 11.7005 4.79505 11.6656C4.64229 11.6303 4.50302 11.5749 4.37745 11.4996C4.25154 11.4241 4.13634 11.325 4.03128 11.2022C3.93865 11.092 3.85971 10.9688 3.79451 10.8328C3.72935 10.697 3.6806 10.5523 3.64814 10.3987C3.61586 10.2459 3.59974 10.084 3.59974 9.91337C3.59974 9.63587 3.64035 9.38591 3.72203 9.16383C3.80412 8.94065 3.92231 8.74875 4.0764 8.58909C4.23039 8.42942 4.4112 8.30766 4.6181 8.22429C4.824 8.14132 5.04332 8.09984 5.27546 8.09984L5.27545 8.09982ZM8.24425 8.09982C8.4576 8.09982 8.64381 8.12654 8.80288 8.18062C8.96226 8.23488 9.09615 8.30771 9.20383 8.3997C9.31124 8.49134 9.39103 8.58925 9.44236 8.69367C9.49381 8.79826 9.52006 8.90232 9.52006 9.0051C9.52006 9.10887 9.48297 9.20332 9.41119 9.28465C9.33649 9.36932 9.2398 9.41327 9.12766 9.41327C9.02776 9.41327 8.94549 9.3862 8.88573 9.32872C8.85344 9.2976 8.82104 9.25588 8.78741 9.20221L8.73585 9.11244C8.68124 8.99924 8.61719 8.91281 8.54419 8.85216C8.48009 8.79896 8.36542 8.76937 8.19907 8.76937C8.04473 8.76937 7.92456 8.80207 7.83608 8.86541C7.75388 8.92435 7.71708 8.98769 7.71708 9.06158C7.71708 9.10746 7.72874 9.14437 7.752 9.17542C7.77954 9.2121 7.81862 9.24468 7.8703 9.27298C7.92784 9.30445 7.98567 9.329 8.04397 9.34658L8.25649 9.4047L8.5426 9.47677C8.66155 9.50853 8.77258 9.54234 8.87583 9.57831C9.03538 9.63386 9.17226 9.70189 9.28627 9.78275C9.40498 9.86695 9.49809 9.974 9.56466 10.103C9.63175 10.2329 9.66468 10.3891 9.66468 10.5708C9.66468 10.7894 9.60784 10.9877 9.49446 11.1631C9.38084 11.3389 9.21543 11.4759 9.00039 11.5736C8.78821 11.67 8.53902 11.7178 8.25331 11.7178C7.91049 11.7178 7.6243 11.6523 7.39545 11.5192C7.23168 11.4225 7.09821 11.2932 6.99589 11.1319C6.89248 10.9689 6.83968 10.8072 6.83968 10.6477C6.83968 10.5412 6.87771 10.4474 6.95177 10.3714C7.02677 10.2943 7.12375 10.2552 7.23659 10.2552C7.33122 10.2552 7.41448 10.2867 7.48157 10.3486C7.52212 10.386 7.55763 10.4337 7.58839 10.4912L7.63111 10.5847C7.67077 10.6839 7.71302 10.7657 7.75755 10.83C7.79716 10.8873 7.85394 10.9354 7.92929 10.9743C8.00294 11.0124 8.10489 11.0325 8.23521 11.0325C8.41445 11.0325 8.55584 10.9919 8.66225 10.9126C8.76285 10.8376 8.80984 10.7502 8.80984 10.6431C8.80984 10.5575 8.78547 10.4925 8.73678 10.4428C8.68082 10.3855 8.60723 10.3411 8.51482 10.3096L8.33365 10.2552L8.09799 10.197C7.854 10.1399 7.64869 10.0727 7.48176 9.99509C7.30698 9.91388 7.16676 9.80185 7.06275 9.65959C6.95587 9.5134 6.90297 9.3334 6.90297 9.12257C6.90297 8.92078 6.95863 8.73943 7.06954 8.5814C7.18017 8.4239 7.33902 8.30366 7.54392 8.22094C7.74436 8.13996 7.97793 8.09982 8.24424 8.09982L8.24425 8.09982ZM12.223 8.09982C12.2973 8.09982 12.3672 8.11863 12.4309 8.15595C12.4941 8.19281 12.5444 8.24261 12.5807 8.30443C12.6169 8.36619 12.6357 8.4307 12.6357 8.49673C12.6357 8.53763 12.6303 8.58075 12.6196 8.62611L12.6029 8.6888L12.5416 8.86459L11.74 11.0277L11.6533 11.2662C11.6222 11.3472 11.5859 11.419 11.5441 11.4817C11.4981 11.5507 11.437 11.6072 11.3619 11.6506C11.2837 11.6958 11.1908 11.7178 11.0843 11.7178C10.9781 11.7178 10.8854 11.6963 10.8075 11.6522C10.7318 11.6094 10.6701 11.5522 10.6232 11.4815C10.5809 11.4175 10.5442 11.3454 10.5132 11.2654L9.59677 8.76819C9.58077 8.72612 9.56741 8.68106 9.55663 8.63302C9.54544 8.58292 9.53964 8.53926 9.53964 8.50124C9.53964 8.39817 9.58101 8.30436 9.6597 8.22374C9.73986 8.14165 9.8424 8.09981 9.96139 8.09981C10.1055 8.09981 10.2169 8.14797 10.2841 8.24723C10.3243 8.30647 10.3634 8.38844 10.4024 8.49332L11.0997 10.5546L11.7889 8.50817C11.8152 8.43147 11.8361 8.37505 11.8519 8.33801C11.8779 8.27708 11.9199 8.22329 11.9768 8.17688C12.0407 8.12462 12.1239 8.09977 12.223 8.09977L12.223 8.09982ZM12.6152 3.11787L12.5997 4.25624C12.5997 4.76735 12.9953 5.15958 13.5025 5.15958L14.7778 5.15179L12.6152 3.11787Z'
                    fill='#5F288D'
                  />
                </svg>
                Export observer list
              </Button>
              <Button title='Add observer' onClick={() => createObserverDialog.trigger()}>
                <Plus className='mr-2' width={18} height={18} />
                Add observer
              </Button>
              <CreateObserverDialog {...createObserverDialog.dialogProps} />
            </div>
          </div>
          <Separator />

          <div className='flex flex-row justify-end gap-4  filters'>
            <Input value={searchText} onChange={handleSearchInput} className='max-w-md' placeholder='Search' />
            <FunnelIcon
              onClick={toggleFilteringContainerVisibility}
              className='w-[20px] text-purple-900 cursor-pointer'
              fill={isFilteringContainerVisible ? '#5F288D' : 'rgba(0,0,0,0)'}
            />{' '}
            <Cog8ToothIcon className='w-[20px] text-purple-900' />
          </div>
          {isFilteringContainerVisible && <ObserversListFilters />}
        </CardHeader>

        <CardContent>
          <QueryParamsDataTable
            columns={observerColDefs}
            useQuery={useObservers}
            onRowClick={(observerId) => navigateToObserver({ observerId })}
            queryParams={queryParams}
          />
          <PasswordSetterDialog {...passwordSetterDialogProps} />
        </CardContent>
      </Card>
    </Layout>
  );
}
