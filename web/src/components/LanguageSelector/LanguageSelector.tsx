import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import i18n from '@/i18n';
import { FC } from 'react';
import { Label } from '../ui/label';

export const LanguageSelector: FC = () => {
  const changeLanguage = (lng: string) => {
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
    <div className='px-4 py-2 space-y-2 '>
      <Label>Language</Label>
      <Select
        defaultValue={localStorage.getItem('i18nextLng') || 'en'}
        onValueChange={(value) => changeLanguage(value)}>
        <SelectTrigger className='w-full'>
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
    </div>
  );
};
