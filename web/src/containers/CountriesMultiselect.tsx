import MultipleSelector, { Option } from '@/components/ui/multiple-selector'
import { useCountries } from '@/features/countries/queries';
import { ReactNode, useEffect, useMemo, useState } from 'react'

export interface CountriesMultiselectProps {
    value?: string[];
    placeholder: string;
    emptyIndicator: ReactNode;
    onChange: (values: string[]) => void;
}

function CountriesMultiselect({ value, placeholder, emptyIndicator, onChange }: CountriesMultiselectProps) {
    const { data: countries } = useCountries();
    const [selected, setSelected] = useState<Option[]>([]);

    var countriesDictionary: { [countryId: string]: Option } = useMemo(() => {
        if (!countries) return {};

        return countries?.reduce((acc: { [countryId: string]: Option }, country) => {
            acc[country.id] = {
                value: country.id,
                label: `${country.name} / ${country.fullName}`,
            }
            return acc;
        }, {});
    }, [countries]);

    const countryOptions = Object.values(countriesDictionary);

    useEffect(() => {
        if (value && countries?.length) {
            setSelected(value.map(code => countriesDictionary[code]!));
        }
    }, [value, countries]);

    function handleSearchCountries(value: string): Promise<Option[]> {
        if (value) {
            return Promise.resolve(countryOptions?.filter(country => country.label.includes(value)));
        }

        return Promise.resolve(countryOptions);
    }

    function handleOnChange(options: Option[]) {
        onChange(options.map(option => option.value));
    };

    return (
        <MultipleSelector
            value={selected}
            onChange={handleOnChange}
            defaultOptions={countryOptions}
            onSearch={handleSearchCountries}
            triggerSearchOnFocus
            placeholder={placeholder}
            emptyIndicator={emptyIndicator}
        />
    )
}

export default CountriesMultiselect
