import { cn, getTagColor } from '@/lib/utils';
import { Combobox, Popover } from '@headlessui/react';
import { ChevronDown, Search, XIcon } from 'lucide-react';
import { FC, useEffect, useRef, useState } from 'react';
import { Badge } from './badge';
import { Input } from './input';
import { Separator } from './separator';

interface TagsSelectFormFieldProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  asChild?: boolean;
  options: string[];
  defaultValue?: string[];
  disabled?: boolean;
  placeholder: string;
  className?: string;
  onValueChange: (value: string[]) => void;
}

const TagsSelectFormField: FC<TagsSelectFormFieldProps> = (props) => {
  const { options, defaultValue, placeholder, onValueChange } = props;
  const [selectedValues, setSelectedValues] = useState<string[]>(defaultValue || []);
  const [query, setQuery] = useState('');
  const searchRef = useRef<HTMLInputElement>(null);
  const hasSelectedValues = selectedValues.length > 0;

  useEffect(() => {
    const valuesSet = new Set(selectedValues);
    onValueChange(Array.from(valuesSet));
  }, [selectedValues]);

  const handleInputKeyDown = (event: any) => {
    if (event.key !== 'Enter') return;
    setQuery('');
  };
  const toggleOption = (value: string) => {
    const currentTag = selectedValues.find((t) => t.toLocaleLowerCase() === value.trim().toLocaleLowerCase());

    if (currentTag) setSelectedValues(selectedValues.filter((v) => v !== value.trim()));
    else setSelectedValues([...selectedValues, value.trim()]);
  };

  const filteredOptions =
    query === ''
      ? options
      : options.filter((option) => {
          return option.toLowerCase().includes(query.toLowerCase());
        });

  const comboboxClasses = cn(
    "relative flex cursor-default select-none items-center rounded-sm px-2 py-1.5 text-sm outline-none hover:bg-accent hover:text-accent-foreground data-[focus]:bg-accent data-[focus]:text-accent-foreground data-[disabled='true']:pointer-events-none data-[disabled='true']:opacity-50 cursor-pointer"
  );

  return (
    <Combobox value={selectedValues} onChange={(value) => setSelectedValues(value)} multiple>
      <Popover className='relative w-full p-1 rounded-md border min-h-10 h-auto items-center justify-between bg-inherit hover:bg-card'>
        {({ open }) => (
          <>
            <Popover.Button className='flex justify-between items-center w-full min-h-8'>
              <div className='flex flex-wrap items-center'>
                {!hasSelectedValues ? (
                  <span className='text-sm text-muted-foreground mx-3'>{placeholder}</span>
                ) : (
                  selectedValues.map((value) => {
                    return (
                      <Badge key={value} className='m-1 text-slate-600' style={{ backgroundColor: getTagColor(value) }}>
                        {value}
                        <XIcon
                          className='ml-2 h-4 w-4 cursor-pointer'
                          onClick={(event) => {
                            event.stopPropagation();
                            toggleOption(value);
                          }}
                        />
                      </Badge>
                    );
                  })
                )}
              </div>

              <div className='flex items-center justify-between'>
                {hasSelectedValues && (
                  <>
                    <XIcon
                      className='h-4 mx-2 cursor-pointer text-muted-foreground'
                      onClick={(event) => {
                        setSelectedValues([]);
                        onValueChange([]);
                        event.stopPropagation();
                      }}
                    />
                    <Separator orientation='vertical' className='flex min-h-6 h-full' />
                  </>
                )}
                <Popover.Button>
                  <ChevronDown className='h-4 mx-2 cursor-pointer text-muted-foreground' />
                </Popover.Button>
              </div>
            </Popover.Button>

            <Popover.Panel className='w-[240px] flex flex-col gap-4 z-50 w-72  rounded-md border bg-popover p-4 text-popover-foreground shadow-md outline-none data-[state=open]:animate-in data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=open]:fade-in-0 data-[state=closed]:zoom-out-95 data-[state=open]:zoom-in-95 data-[side=bottom]:slide-in-from-top-2 data-[side=left]:slide-in-from-right-2 data-[side=right]:slide-in-from-left-2 data-[side=top]:slide-in-from-bottom-2 p-0 drop-shadow-sm absolute z-10  '>
              <div className='flex items-center border-b px-2'>
                <Search className='mr-2 h-4 w-4 shrink-0 opacity-50' />

                <Combobox.Input
                  as={Input}
                  ref={searchRef}
                  placeholder='Search....'
                  value={query}
                  onChange={(event) => setQuery(event.target.value)}
                  onKeyDown={handleInputKeyDown}
                />
              </div>

              <Combobox.Options className='overflow-y-auto max-h-[12rem] '>
                {query.length > 0 && (
                  <Combobox.Option value={query} className={comboboxClasses}>
                    Create <span className='font-bold'>"{query}"</span>
                  </Combobox.Option>
                )}

                {filteredOptions.map((option) => (
                  <Combobox.Option className={comboboxClasses} key={option} value={option}>
                    {option}
                  </Combobox.Option>
                ))}
              </Combobox.Options>
            </Popover.Panel>
          </>
        )}
      </Popover>
    </Combobox>
  );
};

export default TagsSelectFormField;
