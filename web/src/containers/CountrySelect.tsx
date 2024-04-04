import {
    Command,
    CommandEmpty,
    CommandGroup,
    CommandInput,
    CommandItem,
    CommandList,
} from "@/components/ui/command";
import {
    Popover,
    PopoverContent,
    PopoverTrigger,
} from "@/components/ui/popover";
import { cn } from '@/lib/utils';
import { Check, ChevronsUpDown } from 'lucide-react';
import { useCountries } from '@/features/countries/queries';
import { Button } from '@/components/ui/button';
import { useMemo, useState } from "react";
import { useTranslation } from "react-i18next";

export interface CountrySelectProps {
    countryId: string;
    onSelect: (value: string) => void;
}

function CountrySelect({ countryId, onSelect }: CountrySelectProps) {
    const { data: countries } = useCountries();
    const [open, setOpen] = useState(false);
    const [value, setValue] = useState(countryId);
    const { t } = useTranslation();

    const placeholder = useMemo(() => {
        if (value && countries?.length) {
            const country = countries?.find((l) => l.id === value);

            return `${country?.name} / ${country?.fullName}`;
        }

        return t('containers.countrySelect.placeholder');
    }, [countries, value]);

    return (
        <Popover open={open} onOpenChange={setOpen}>
            <PopoverTrigger asChild>
                <Button
                    variant="outline"
                    role="combobox"
                    aria-expanded={open}
                    className="w-[350px] justify-between"
                >
                    {placeholder}
                    <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                </Button>
            </PopoverTrigger>
            <PopoverContent className="w-[350px] p-0">
                <Command filter={(value, search, keywords) => {
                    const extendValue = value + ' ' + keywords?.join(' ');
                    if (extendValue.toLocaleLowerCase().includes(search.toLocaleLowerCase())) return 1
                    return 0
                }}>
                    <CommandInput placeholder={t('containers.countrySelect.searchPlaceholder')} />
                    <CommandEmpty>{t('containers.countrySelect.noResults')}</CommandEmpty>
                    <CommandList>
                        <CommandGroup>
                            {countries?.map((country) => (
                                <CommandItem
                                    key={country.id}
                                    value={country.id}
                                    keywords={[country.name, country.fullName]}
                                    onSelect={(currentValue) => {
                                        setValue(currentValue);
                                        setOpen(false);
                                        onSelect(currentValue);
                                    }}
                                >
                                    <Check
                                        className={cn(
                                            "mr-2 h-4 w-4",
                                            value === country.id ? "opacity-100" : "opacity-0"
                                        )}
                                    />
                                    {country.name}
                                </CommandItem>
                            ))}
                        </CommandGroup>
                    </CommandList>
                </Command>
            </PopoverContent>
        </Popover>
    )
}

export default CountrySelect;
