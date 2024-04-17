import MultipleSelector, { Option } from '@/components/ui/multiple-selector'
import { useLanguages } from '@/features/languages/queries';
import { ReactNode, useEffect, useMemo, useState } from 'react'

export interface LanguagesMultiselectProps {
    value?: string[];
    defaultLanguage: string;
    placeholder: string;
    emptyIndicator: ReactNode;
    onChange: (values: string[]) => void;
}

function LanguagesMultiselect({ value, defaultLanguage, placeholder, emptyIndicator, onChange }: LanguagesMultiselectProps) {
    const { data: languages } = useLanguages();
    const [selected, setSelected] = useState<Option[]>([]);

    var languagesDictionary: { [languageCode: string]: Option } = useMemo(() => {
        if (!languages) return {};

        return languages?.reduce((acc: { [languageCode: string]: Option }, language) => {
            acc[language.code] = {
                value: language.code,
                label: `${language.name} / ${language.nativeName}`,
                fixed: language.code == defaultLanguage
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


    function handleSearchLanguages(value: string): Promise<Option[]> {
        if (value) {
            return Promise.resolve(languageOptions?.filter(language => language.label.includes(value)));
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
            triggerSearchOnFocus
            placeholder={placeholder}
            emptyIndicator={emptyIndicator}
        />
    )
}

export default LanguagesMultiselect
