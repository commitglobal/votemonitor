'use client'

import * as React from 'react'
import { format } from 'date-fns'
import { CalendarIcon, XCircle } from 'lucide-react'
import type { DateRange } from 'react-day-picker'
import { DateTimeFormat } from '@/constants/formats'
import { Button } from '@/components/ui/button'
import { Calendar } from '@/components/ui/calendar'
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from '@/components/ui/popover'
import { Separator } from '@/components/ui/separator'

type DateSelection = Date | DateRange

function getIsDateRange(value: DateSelection | undefined): value is DateRange {
  return (
    value !== undefined && typeof value === 'object' && !Array.isArray(value)
  )
}

interface DataTableDateFilterProps {
  value: DateSelection | undefined
  label?: string
  multiple?: boolean
  onValueChange: (value: DateSelection | undefined) => void
}

function DataTableDateFilter({
  label,
  multiple,
  value,
  onValueChange,
}: DataTableDateFilterProps) {
  // const selectedDates = React.useMemo<DateSelection>(() => {
  //   if (!value) {
  //     return multiple ? { from: undefined, to: undefined } : []
  //   }

  //   if (multiple) {
  //     const timestamps = parseColumnFilterValue(value)
  //     return {
  //       from: parseAsDate(timestamps[0]),
  //       to: parseAsDate(timestamps[1]),
  //     }
  //   }

  //   const timestamps = parseColumnFilterValue(value)
  //   const date = parseAsDate(timestamps[0])
  //   return date ? [date] : []
  // }, [value, multiple])

  const onSelect = React.useCallback(
    (date: Date | DateRange | undefined) => {
      if (!date) {
        onValueChange(undefined)
        return
      }

      if (multiple && !('getTime' in date)) {
        const from = date.from
        const to = date.to
        onValueChange(from || to ? { from, to } : undefined)
      } else if (!multiple && 'getTime' in date) {
        onValueChange(date)
      }
    },
    [onValueChange, multiple]
  )

  const onReset = React.useCallback(
    (event: React.MouseEvent) => {
      event.stopPropagation()
      onValueChange(undefined)
    },
    [onValueChange]
  )

  const hasValue = React.useMemo(() => {
    if (multiple) {
      if (!getIsDateRange(value)) return false
      return value.from || value.to
    }
    if (!Array.isArray(value)) return false
    return value.length > 0
  }, [multiple, value])

  const formatDateRange = React.useCallback((range: DateRange) => {
    if (!range.from && !range.to) return ''
    if (range.from && range.to) {
      return `${format(range.from, DateTimeFormat)} - ${format(range.to, DateTimeFormat)}`
    }
    return range.to ? format(range.from ?? range.to, DateTimeFormat) : ''
  }, [])

  const filterLabel = React.useMemo(() => {
    if (multiple) {
      if (!getIsDateRange(value)) return null

      const hasSelectedDates = value.from || value.to
      const dateText = hasSelectedDates
        ? formatDateRange(value)
        : 'Select date range'

      return (
        <span className='flex items-center gap-2'>
          <span>{label}</span>
          {hasSelectedDates && (
            <>
              <Separator
                orientation='vertical'
                className='mx-0.5 data-[orientation=vertical]:h-4'
              />
              <span>{dateText}</span>
            </>
          )}
        </span>
      )
    }

    if (getIsDateRange(value)) return null

    const hasSelectedDate = !!value
    const dateText = hasSelectedDate
      ? format(value, DateTimeFormat)
      : 'Select date'

    return (
      <span className='flex items-center gap-2'>
        <span>{label}</span>
        {hasSelectedDate && (
          <>
            <Separator
              orientation='vertical'
              className='mx-0.5 data-[orientation=vertical]:h-4'
            />
            <span>{dateText}</span>
          </>
        )}
      </span>
    )
  }, [value, multiple, formatDateRange, label])

  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button variant='outline' size='sm' className='border-dashed'>
          {hasValue ? (
            <div
              role='button'
              aria-label={`Clear ${label} filter`}
              tabIndex={0}
              onClick={onReset}
              className='focus-visible:ring-ring rounded-sm opacity-70 transition-opacity hover:opacity-100 focus-visible:ring-1 focus-visible:outline-none'
            >
              <XCircle />
            </div>
          ) : (
            <CalendarIcon />
          )}
          {filterLabel}
        </Button>
      </PopoverTrigger>
      <PopoverContent className='w-auto p-0' align='start'>
        {multiple ? (
          <Calendar
            captionLayout='dropdown'
            mode='range'
            selected={
              getIsDateRange(value) ? value : { from: undefined, to: undefined }
            }
            onSelect={onSelect}
          />
        ) : (
          <Calendar
            captionLayout='dropdown'
            mode='single'
            selected={!getIsDateRange(value) ? value : undefined}
            onSelect={onSelect}
          />
        )}
      </PopoverContent>
    </Popover>
  )
}

interface DateFilterProps {
  value: Date | undefined
  onValueChange: (date: Date | undefined) => void
  label: string
}

export function DateFilter({ value, onValueChange, label }: DateFilterProps) {
  return (
    <DataTableDateFilter
      value={value}
      multiple={false}
      onValueChange={(date) => onValueChange(date as Date)}
      label={label}
    />
  )
}

interface DateRangeFilterProps {
  value: DateRange | undefined
  onValueChange: (date: DateRange | undefined) => void
  label: string
}
export function DateRangeFilter({
  value,
  onValueChange,
  label,
}: DateRangeFilterProps) {
  return (
    <DataTableDateFilter
      value={value}
      label={label}
      multiple
      onValueChange={(date) => onValueChange(date as DateRange)}
    />
  )
}
