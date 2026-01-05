import React from 'react'
import { useSuspenseListForms } from '@/queries/forms'
import { Route } from '@/routes/(app)/elections/$electionRoundId/forms'
import { useDataTable } from '@/hooks/use-data-table'
import { DataTable } from '@/components/ui/data-table'
import { DataTableToolbar } from '@/components/data-table/data-table-toolbar'
import TableFilters from './Filters'
import { FormsProvider } from './FormsProvider'
import { getFormsTableColumns } from './TableColumns'
import type { FormModel } from '@/types/form'
import type { Language } from '@/types/language'

export interface FormRowWithSubrows extends FormModel {
  isSubrow?: boolean
  subrowLanguage?: Language
  parentId?: string
}

export default function Table() {
  const { electionRoundId } = Route.useParams()
  const search = Route.useSearch()
  const navigate = Route.useNavigate()
  const { data } = useSuspenseListForms(electionRoundId, search)
  const columns = React.useMemo(() => getFormsTableColumns(), [])

  // Transform data to include subrows for each non-default language
  const transformedData = React.useMemo<FormRowWithSubrows[]>(() => {
    return data.items.map((form) => {
      // Create subrows for each language except defaultLanguage
      const subrows: FormRowWithSubrows[] = form.languages
        .filter((lang) => lang !== form.defaultLanguage)
        .map((lang) => ({
          ...form,
          isSubrow: true,
          subrowLanguage: lang,
          parentId: form.id,
          id: `${form.id}-${lang}`, // Unique ID for subrow
        }))
      
      // Return parent row with subrows
      return {
        ...form,
        isSubrow: false,
        subRows: subrows.length > 0 ? subrows : undefined,
      } as FormRowWithSubrows & { subRows?: FormRowWithSubrows[] }
    })
  }, [data.items])

  // Create expanded state to expand all rows with subrows by default
  const expandedState = React.useMemo(() => {
    const expanded: Record<string, boolean> = {}
    transformedData.forEach((form) => {
      if ((form as FormRowWithSubrows & { subRows?: FormRowWithSubrows[] }).subRows) {
        expanded[form.id] = true
      }
    })
    return expanded
  }, [transformedData])

  const { table } = useDataTable({
    tableName: 'forms',
    data: transformedData,
    columns,
    pageCount: Math.ceil(data.totalCount / data.pageSize),
    initialState: {
      sorting: [{ id: 'lastModifiedOn', desc: true }],
      columnPinning: { right: ['actions'] },
      expanded: expandedState,
    },
    getRowId: (originalRow) => originalRow.id,
    getSubRows: (row) => (row as FormRowWithSubrows & { subRows?: FormRowWithSubrows[] }).subRows,
    search,
    navigate,
  })

  return (
    <FormsProvider>
      <DataTable table={table}>
        <DataTableToolbar table={table}>
          <TableFilters />
        </DataTableToolbar>
      </DataTable>
    </FormsProvider>
  )
}
