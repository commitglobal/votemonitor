import Layout from '@/components/layout/Layout';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { FormBase } from '@/features/forms/models/form';
import { useForms } from '@/features/forms/queries';
import { cn, mapFormType } from '@/lib/utils';
import { ColumnDef, Row } from '@tanstack/react-table';
import { ChevronDownIcon, ChevronUpIcon } from 'lucide-react';
import { FC } from 'react';
import { useTranslation } from 'react-i18next';

export const FormBuilderScreenReuse: FC = () => {
  const { t } = useTranslation('translation', { keyPrefix: 'electionEvent.form' });
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);

  const getSubrows = (originalRow: FormBase, index: number): undefined | FormBase[] => {
    if (originalRow.languages.length === 0) return undefined;

    // we need to have subrows only for translations
    return originalRow.languages
      .filter((languageCode) => originalRow.defaultLanguage !== languageCode)
      .map((languageCode) => ({
        ...originalRow,
        languages: [],
        code: `${originalRow.code} - ${languageCode}`,
        defaultLanguage: languageCode,
      }));
  };

  const getRowClassName = (row: Row<FormBase>): string => cn({ 'bg-secondary-300 bg-opacity-[.15]': row.depth === 1 });

  const templatesColDefs: ColumnDef<FormBase>[] = [
    {
      header: '',
      id: 'colapse',
      cell: ({ row, getValue }) => (
        <div>
          {row.getCanExpand() ? (
            <button
              {...{
                onClick: row.getToggleExpandedHandler(),
                style: { cursor: 'pointer' },
              }}>
              {row.getIsExpanded() ? (
                <ChevronUpIcon className='w-4 h-4 ml-auto opacity-50' />
              ) : (
                <ChevronDownIcon className='w-4 h-4 ml-auto opacity-50' />
              )}
            </button>
          ) : (
            ''
          )}
          {getValue<boolean>()}
        </div>
      ),
      enableResizing: false,
    },
    {
      accessorKey: 'code',
      header: ({ column }) => <DataTableColumnHeader title={'Form code'} column={column} />,
    },

    {
      id: 'name',
      accessorFn: (row, _) => row.name[row.defaultLanguage],
      header: ({ column }) => <DataTableColumnHeader title={'Form name'} column={column} />,
    },

    {
      accessorKey: 'formTemplateType',
      accessorFn: (row, _) => mapFormType(row.formType),
      enableSorting: false,
      enableResizing: false,
      header: ({ column }) => <DataTableColumnHeader title={'Form type'} column={column} />,
      cell: ({ row }) => (row.depth === 0 ? row.original.formType : ''),
    },

    {
      accessorKey: 'defaultLanguage',
      enableSorting: false,
      enableResizing: false,
      header: ({ column }) => <DataTableColumnHeader title={'Language'} column={column} />,
    },
  ];

  return (
    <Layout title={t('reuse.title')} subtitle={t('reuse.description')}>
      <QueryParamsDataTable
        columns={templatesColDefs}
        useQuery={(params) => useForms(currentElectionRoundId, params)}
        getSubrows={getSubrows}
        getRowClassName={getRowClassName}
      />
    </Layout>
  );
};
