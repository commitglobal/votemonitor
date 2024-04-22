import MultipleSelector, { Option } from '@/components/ui/multiple-selector'
import { useLanguages } from '@/features/languages/queries';
import { ReactNode, useEffect, useMemo, useState } from 'react'

export interface LanguagesMultiselectProps {
    value?: string[];
    defaultLanguages: string[];
    placeholder: string;
    emptyIndicator?: ReactNode;
    onChange: (values: string[]) => void;
}

function LanguagesMultiselect({ value, defaultLanguages, placeholder, emptyIndicator, onChange }: LanguagesMultiselectProps) {
    const { data: languages } = useLanguages();
    const [selected, setSelected] = useState<Option[]>([]);

    var languagesDictionary: { [languageCode: string]: Option } = useMemo(() => {
        if (!languages) return {};

        return languages?.reduce((acc: { [languageCode: string]: Option }, language) => {
            acc[language.code] = {
                value: language.code,
                label: `${language.name} / ${language.nativeName}`,
                fixed: defaultLanguages?.includes(language.code)
            }
            return acc;
        }, {});
    }, [languages]);

    const languageOptions = Object.values(languagesDictionary);

    useEffect(() => {
        if (value && languages?.length) {
            setSelected(value.map(code => languagesDictionary[code]!));
        }
    }, [value, languages]);

    function handleSearchLanguages(search: string): Promise<Option[]> {
        if (search) {
            return Promise.resolve(languageOptions?.filter(language => language.label.toLocaleLowerCase().includes(search.toLocaleLowerCase()) ));
        }

        return Promise.resolve(languageOptions);
    }

    function handleOnChange(options: Option[]) {
        onChange(options.map(option => option.value));
    };

    return (
        <MultipleSelector
            value={selected}
            onChange={handleOnChange}
            defaultOptions={languageOptions}
            onSearch={handleSearchLanguages}
            hidePlaceholderWhenSelected={true}
            placeholder={placeholder}
            emptyIndicator={emptyIndicator}
        />
    )
}

export default LanguagesMultiselect
