import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import i18n from '@/i18n';
import { FC } from 'react';

export const LanguageSelector: FC = () => {
  const changeLanguage = (lng) => {
    i18n.changeLanguage(lng);
  };

  const options = [
    {
      label: 'English',
      value: 'en',
    },
    {
      label: 'Română',
      value: 'ro',
    },
  ];
  return (
    <Select onValueChange={(value) => changeLanguage(value)}>
      <SelectTrigger className='w-[180px]'>
        <SelectValue placeholder='Select language' />
      </SelectTrigger>
      <SelectContent>
        <SelectGroup>
          {options.map((option) => {
            return (
              <SelectItem key={`language-${option.value}`} value={option.value}>
                {option.label}
              </SelectItem>
            );
          })}
        </SelectGroup>
      </SelectContent>
    </Select>
  );
};
