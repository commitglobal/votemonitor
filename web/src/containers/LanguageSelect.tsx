import { Button } from '@/components/ui/button';
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
import { useLanguages } from '@/features/languages/queries';
import { cn } from '@/lib/utils';
import { Check, ChevronsUpDown } from 'lucide-react';
import { useMemo, useState } from "react";
import { useTranslation } from "react-i18next";

export interface LanguageSelectProps {
    languageCode: string;
    excludeLanguageCodes?: string[];
    disabled?: boolean;
    onLanguageSelected?: (value: string) => void;
}

function LanguageSelect({ disabled, languageCode, onLanguageSelected, excludeLanguageCodes = [] }: LanguageSelectProps) {
    const { data: languages } = useLanguages();
    const [open, setOpen] = useState(false);
    const [value, setValue] = useState(languageCode);
    const { t } = useTranslation();

    const placeholder = useMemo(() => {
        if (value && languages?.length) {
            const language = languages?.find((l) => l.code === value);

            return `${language?.name} / ${language?.nativeName}`;
        }

        return t('containers.languageSelect.placeholder');
    }, [languages, value]);

    return (
        <Popover open={open} onOpenChange={setOpen}>
            <PopoverTrigger asChild>
                <Button
                    variant="ghost"
                    role="combobox"
                    aria-expanded={open}
                    disabled={disabled}
                    className="w-full justify-between my-1 flex h-10 rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
                >
                    {placeholder}
                    <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                </Button>
            </PopoverTrigger>
            <PopoverContent className="w-[350px] py-1 px-0">
                <Command filter={(value, search, keywords) => {
                    const extendValue = value + ' ' + keywords?.join(' ');
                    if (extendValue.toLocaleLowerCase().includes(search.toLocaleLowerCase())) return 1
                    return 0
                }}>
                    <CommandInput placeholder={t('containers.languageSelect.searchPlaceholder')} />
                    <CommandEmpty>{t('containers.languageSelect.noResults')}</CommandEmpty>
                    <CommandList>
                        <CommandGroup>
                            {languages?.filter(language => !excludeLanguageCodes.includes(language.code)).map((language) => (
                                <CommandItem
                                    key={language.code}
                                    value={language.code}
                                    keywords={[language.code, language.name, language.nativeName]}
                                    onSelect={(currentValue) => {
                                        setValue(currentValue);
                                        setOpen(false);
                                        onLanguageSelected && onLanguageSelected(currentValue);
                                    }}
                                >
                                    <Check
                                        className={cn(
                                            "mr-2 h-4 w-4",
                                            value === language.code ? "opacity-100" : "opacity-0"
                                        )}
                                    />
                                    {`${language.name} / ${language.nativeName}`}
                                </CommandItem>
                            ))}
                        </CommandGroup>
                    </CommandList>
                </Command>
            </PopoverContent>
        </Popover>
    )
}

export default LanguageSelect;
