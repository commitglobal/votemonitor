import type * as React from 'react'
import { type Table as TanstackTable, flexRender } from '@tanstack/react-table'
import { getCommonPinningStyles } from '@/lib/data-table'
import { cn } from '@/lib/utils'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import { DataTablePagination } from '@/components/data-table/data-table-pagination'
import { ChevronDown, ChevronRight } from 'lucide-react'
import { Button } from '@/components/ui/button'

interface DataTableProps<TData> extends React.ComponentProps<'div'> {
  table: TanstackTable<TData>
  actionBar?: React.ReactNode
}

export function DataTable<TData>({
  table,
  actionBar,
  children,
  className,
  ...props
}: DataTableProps<TData>) {
  return (
    <div
      className={cn('flex w-full flex-col gap-2.5 overflow-auto', className)}
      {...props}
    >
      {children}
      <div className='overflow-hidden rounded-md border'>
        <Table>
          <TableHeader>
            {table.getHeaderGroups().map((headerGroup) => (
              <TableRow key={headerGroup.id}>
                {headerGroup.headers.map((header) => (
                  <TableHead
                    key={header.id}
                    colSpan={header.colSpan}
                    style={{
                      ...getCommonPinningStyles({ column: header.column }),
                    }}
                  >
                    {header.isPlaceholder
                      ? null
                      : flexRender(
                          header.column.columnDef.header,
                          header.getContext()
                        )}
                  </TableHead>
                ))}
              </TableRow>
            ))}
          </TableHeader>
          <TableBody>
            {table.getRowModel().rows?.length ? (
              table.getRowModel().rows.map((row) => {
                const canExpand = row.getCanExpand()
                const isExpanded = row.getIsExpanded()
                const isSubrow = (row.original as { isSubrow?: boolean })
                  .isSubrow

                return (
                  <>
                    <TableRow
                      data-state={row.getIsSelected() && 'selected'}
                      className={cn(isSubrow && 'bg-muted/50')}
                    >
                      {row.getVisibleCells().map((cell, cellIndex) => {
                        // Add expand button to first visible cell if row can expand
                        const isFirstCell = cellIndex === 0
                        
                        return (
                          <TableCell
                            key={cell.id}
                            style={{
                              ...getCommonPinningStyles({ column: cell.column }),
                              paddingLeft: isSubrow
                                ? `${(row.depth + 1) * 1.5}rem`
                                : undefined,
                            }}
                          >
                            <div className={cn('flex items-center gap-2', isSubrow && 'pl-4')}>
                              {isFirstCell && canExpand && (
                                <Button
                                  variant='ghost'
                                  size='sm'
                                  className='h-6 w-6 p-0'
                                  onClick={(e) => {
                                    e.stopPropagation()
                                    row.toggleExpanded()
                                  }}
                                >
                                  {isExpanded ? (
                                    <ChevronDown className='h-4 w-4' />
                                  ) : (
                                    <ChevronRight className='h-4 w-4' />
                                  )}
                                </Button>
                              )}
                              {isFirstCell && !canExpand && isSubrow && (
                                <div className='w-6' /> // Spacer for alignment
                              )}
                              {flexRender(
                                cell.column.columnDef.cell,
                                cell.getContext()
                              )}
                            </div>
                          </TableCell>
                        )
                      })}
                    </TableRow>
                  </>
                )
              })
            ) : (
              <TableRow>
                <TableCell
                  colSpan={table.getAllColumns().length}
                  className='h-24 text-center'
                >
                  No results.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>
      <div className='flex flex-col gap-2.5'>
        <DataTablePagination table={table} />
        {actionBar &&
          table.getFilteredSelectedRowModel().rows.length > 0 &&
          actionBar}
      </div>
    </div>
  )
}
