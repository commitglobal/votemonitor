import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { useRouter } from '@tanstack/react-router';
import { FC } from 'react';
import { useTranslation } from 'react-i18next';
import { Label } from '../ui/label';

export const LanguageSelector: FC = () => {
  const { i18n } = useTranslation(); // not passing any namespace will use the defaultNS (by default set to 'translation')
  const router = useRouter();

  const changeLanguage = (lng: string) => {
    i18n.changeLanguage(lng);
    router.invalidate();
  };

  const options = [
    {
      label: 'English',
      value: 'en-US',
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
        defaultValue={localStorage.getItem('i18nextLng') || 'en-US'}
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
