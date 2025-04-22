"use client";

import type { Option } from "@/types/data-table";
import { Check, PlusCircle, XCircle } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
  CommandSeparator,
} from "@/components/ui/command";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { Separator } from "@/components/ui/separator";
import { cn } from "@/lib/utils";
import * as React from "react";

export interface SingleSelectDataTableFacetedFilterProps {
  title: string;
  options: Option[];
  value: string;
  onValueChange: (value: string | undefined) => void;
}
export interface MultiSelectDataTableFacetedFilterProps {
  title: string;
  options: Option[];
  value: string[] | undefined;
  onValueChange: (value: string[] | undefined) => void;
}

export function SingleSelectDataTableFacetedFilter({
  title,
  options,
  onValueChange,
  value,
}: SingleSelectDataTableFacetedFilterProps) {
  return (
    <DataTableFacetedFilter
      title={title}
      options={options}
      onValueChange={(value) => onValueChange(value?.[0])}
      value={[value]}
      multiple={false}
    />
  );
}
export function MultiSelectDataTableFacetedFilter({
  title,
  options,
  onValueChange,
  value,
}: MultiSelectDataTableFacetedFilterProps) {
  return (
    <DataTableFacetedFilter
      title={title}
      options={options}
      onValueChange={onValueChange}
      value={value}
      multiple={true}
    />
  );
}

interface DataTableFacetedFilterProps {
  title: string;
  options: Option[];
  multiple?: boolean;
  value: string[] | undefined;
  onValueChange: (value: string[] | undefined) => void;
}

function DataTableFacetedFilter({
  value,
  onValueChange,
  title,
  options,
  multiple,
}: DataTableFacetedFilterProps) {
  const [open, setOpen] = React.useState(false);

  const selectedValues = new Set(Array.isArray(value) ? value : []);

  const onItemSelect = React.useCallback(
    (option: Option, isSelected: boolean) => {
      if (multiple) {
        const newSelectedValues = new Set(selectedValues);
        if (isSelected) {
          newSelectedValues.delete(option.value);
        } else {
          newSelectedValues.add(option.value);
        }
        const filterValues = Array.from(newSelectedValues);
        onValueChange(filterValues.length ? filterValues : undefined);
      } else {
        onValueChange(isSelected ? undefined : [option.value]);
        setOpen(false);
      }
    },
    [onValueChange, multiple, selectedValues]
  );

  const onReset = React.useCallback(
    (event?: React.MouseEvent) => {
      event?.stopPropagation();
      onValueChange(undefined);
    },
    [onValueChange]
  );

  return (
    <Popover open={open} onOpenChange={setOpen}>
      <PopoverTrigger asChild>
        <Button variant="outline" size="sm" className="border-dashed">
          {selectedValues?.size > 0 ? (
            <div
              role="button"
              aria-label={`Clear ${title} filter`}
              tabIndex={0}
              onClick={onReset}
              className="rounded-sm opacity-70 transition-opacity hover:opacity-100 focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring"
            >
              <XCircle />
            </div>
          ) : (
            <PlusCircle />
          )}
          {title}
          {selectedValues?.size > 0 && (
            <>
              <Separator
                orientation="vertical"
                className="mx-0.5 data-[orientation=vertical]:h-4"
              />
              <Badge
                variant="secondary"
                className="rounded-sm px-1 font-normal lg:hidden"
              >
                {selectedValues.size}
              </Badge>
              <div className="hidden items-center gap-1 lg:flex">
                {selectedValues.size > 2 ? (
                  <Badge
                    variant="secondary"
                    className="rounded-sm px-1 font-normal"
                  >
                    {selectedValues.size} selected
                  </Badge>
                ) : (
                  options
                    .filter((option) => selectedValues.has(option.value))
                    .map((option) => (
                      <Badge
                        variant="secondary"
                        key={option.value}
                        className="rounded-sm px-1 font-normal"
                      >
                        {option.label}
                      </Badge>
                    ))
                )}
              </div>
            </>
          )}
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-[12.5rem] p-0" align="start">
        <Command>
          <CommandInput placeholder={title} />
          <CommandList className="max-h-full">
            <CommandEmpty>No results found.</CommandEmpty>
            <CommandGroup className="max-h-[18.75rem] overflow-y-auto overflow-x-hidden">
              {options.map((option) => {
                const isSelected = selectedValues.has(option.value);

                return (
                  <CommandItem
                    key={option.value}
                    onSelect={() => onItemSelect(option, isSelected)}
                  >
                    <div
                      className={cn(
                        "flex size-4 items-center justify-center rounded-sm border border-primary",
                        isSelected
                          ? "bg-primary"
                          : "opacity-50 [&_svg]:invisible"
                      )}
                    >
                      <Check />
                    </div>
                    <span className="truncate">{option.label}</span>
                  </CommandItem>
                );
              })}
            </CommandGroup>
            {selectedValues.size > 0 && (
              <>
                <CommandSeparator />
                <CommandGroup>
                  <CommandItem
                    onSelect={() => onReset()}
                    className="justify-center text-center"
                  >
                    Clear filters
                  </CommandItem>
                </CommandGroup>
              </>
            )}
          </CommandList>
        </Command>
      </PopoverContent>
    </Popover>
  );
}
