import { Fragment, useState } from 'react';
import { Disclosure, Menu, Transition } from '@headlessui/react';
import { Bars3Icon, XMarkIcon, ChevronDownIcon } from '@heroicons/react/24/outline';
import { UserCircleIcon } from '@heroicons/react/24/solid';
import type { ElectionRoundMonitoring, FunctionComponent } from '../../../common/types';
import Logo from '../../../assets/icons/logo.svg?react';
import { Link } from '@tanstack/react-router';
import clsx from 'clsx';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuRadioGroup,
  DropdownMenuRadioItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Badge } from '@/components/ui/badge';
import { useQuery } from '@tanstack/react-query';
import { authApi } from '@/common/auth-api';
import { Skeleton } from '@/components/ui/skeleton';
import { queryClient } from '@/main';

const user = {
  name: 'Tom Cook',
  email: 'tom@example.com',
  imageUrl:
    'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=facearea&facepad=2&w=256&h=256&q=80',
};
const navigation = [
  { name: 'Dashboard', to: '/' },
  { name: 'Election rounds', to: '/election-rounds' },
  { name: 'NGOs', to: '/ngos' },
  { name: 'Observers', to: '/observers' },
  { name: 'Monitoring Observers', to: '/monitoring-observers' },
  { name: 'Form templates', to: '/form-templates' },
];
const userNavigation = [{ name: 'Sign out', to: '#' }];

const Header = (): FunctionComponent => {
  const [selectedElection, setSelectedElection] = useState<ElectionRoundMonitoring | null | undefined>(null);

  const { status, data } = useQuery({
    queryKey: ['electionRounds'],
    queryFn: async () => {
      const response = await authApi.get<{ electionRounds: ElectionRoundMonitoring[] }>('/election-rounds:monitoring');

      if (response.status !== 200) {
        throw new Error('Failed to fetch observers');
      }

      console.log('refreshed', response.data.electionRounds[0]);

      handleSelectEelection(response.data.electionRounds[0] as ElectionRoundMonitoring);

      return response.data;
    },
    staleTime: 0,
    refetchOnWindowFocus: false,
  });

  const handleSelectEelection = (ev: ElectionRoundMonitoring) => {
    setSelectedElection(ev);
    localStorage.setItem('electionRoundId', ev.electionRoundId);
    localStorage.setItem('monitoringNgoId', ev.monitoringNgoId);

    queryClient.invalidateQueries({ queryKey: ['observers'] });
    queryClient.invalidateQueries({ queryKey: ['tags'] });
  };

  return (
    <Disclosure as='nav' className='bg-white shadow-sm'>
      {({ open }) => (
        <>
          <div className='container'>
            <div className='flex items-center justify-between h-16 gap-6 md:gap-10'>
              <Logo className='w-12 h-12 fill-primary-400 shrink-0' />

              <div className='items-baseline flex-1 hidden gap-4 md:flex'>
                {navigation.map((item) => (
                  <Link
                    to={item.to}
                    search={{}}
                    params={{}}
                    key={item.name}
                    className='px-3 py-2 text-sm font-medium rounded-md'
                    activeProps={{
                      className: 'bg-primary-100 text-primary-600 cursor-default',
                      'aria-current': 'page',
                    }}
                    inactiveProps={{
                      className:
                        'hover:text-primary-600 hover:bg-secondary-300 focus:text-primary-600 focus:bg-secondary-300',
                    }}>
                    {item.name}
                  </Link>
                ))}
              </div>

              <div className='items-center gap-2 hidden md:flex'>
                {status === 'pending' ? (
                  <Skeleton className='w-[160px] h-[26px] mr-2 rounded-lg bg-secondary-300 text-secondary-900 hover:bg-secondary-300/90' />
                ) : (
                  <DropdownMenu className='mr-4'>
                    <DropdownMenuTrigger>
                      <Badge className='bg-secondary-300 text-secondary-900 hover:bg-secondary-300/90'>
                        <span className='election-text'>{selectedElection?.englishTitle}</span>
                        <ChevronDownIcon className='w-[20px] ml-2' />
                      </Badge>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent>
                      <DropdownMenuRadioGroup value={selectedElection} onValueChange={handleSelectEelection}>
                        {data?.electionRounds.map((electionRound) => (
                          <DropdownMenuRadioItem key={electionRound.electionRoundId} value={electionRound}>
                            {electionRound.englishTitle}
                          </DropdownMenuRadioItem>
                        ))}
                      </DropdownMenuRadioGroup>
                    </DropdownMenuContent>
                  </DropdownMenu>
                )}
                <Menu as='div' className='relative'>
                  <div>
                    <Menu.Button className='relative flex text-sm bg-white rounded-full focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2'>
                      <span className='absolute -inset-1.5' />
                      <span className='sr-only'>Open user menu</span>
                      <UserCircleIcon className='w-8 h-8 fill-gray-400' />
                    </Menu.Button>
                  </div>
                  <Transition
                    as={Fragment}
                    enter='transition ease-out duration-100'
                    enterFrom='transform opacity-0 scale-95'
                    enterTo='transform opacity-100 scale-100'
                    leave='transition ease-in duration-75'
                    leaveFrom='transform opacity-100 scale-100'
                    leaveTo='transform opacity-0 scale-95'>
                    <Menu.Items className='absolute right-0 z-10 w-48 py-1 mt-2 origin-top-right bg-white rounded-md shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none'>
                      {userNavigation.map((item) => (
                        <Menu.Item key={item.name}>
                          <Link
                            to={item.to}
                            search={{}}
                            params={{}}
                            className='block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 focus:bg-gray-100'>
                            {item.name}
                          </Link>
                        </Menu.Item>
                      ))}
                    </Menu.Items>
                  </Transition>
                </Menu>
              </div>

              <div className='flex -mr-2 md:hidden'>
                {/* Mobile menu button */}
                <Disclosure.Button
                  className={clsx(
                    'relative inline-flex items-center justify-center p-2 text-gray-400 bg-white rounded-md hover:text-primary-600 hover:bg-secondary-300 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2',
                    { 'bg-secondary-300': open }
                  )}>
                  <span className='absolute -inset-0.5' />
                  <span className='sr-only'>Open main menu</span>
                  {open ? (
                    <XMarkIcon className='block w-6 h-6' aria-hidden='true' />
                  ) : (
                    <Bars3Icon className='block w-6 h-6' aria-hidden='true' />
                  )}
                </Disclosure.Button>
              </div>
            </div>
          </div>

          <Disclosure.Panel className='md:hidden'>
            <div className='px-2 pt-2 pb-3 space-y-1 sm:px-3'>
              {navigation.map((item) => (
                <Disclosure.Button
                  key={item.name}
                  as={Link}
                  to={item.to}
                  search={{}}
                  params={{}}
                  className='block px-3 py-2 text-base font-medium rounded-md'
                  activeProps={{ className: 'bg-primary-100 text-primary-600 cursor-default', 'aria-current': 'page' }}
                  inactiveProps={{
                    className:
                      'hover:text-primary-600 hover:bg-secondary-300 focus:text-primary-600 focus:bg-secondary-300',
                  }}>
                  {item.name}
                </Disclosure.Button>
              ))}
            </div>
            <div className='pt-4 pb-3 border-t border-gray-700'>
              <div className='flex items-center px-5'>
                <div className='flex-shrink-0'>
                  <UserCircleIcon className='w-10 h-10 fill-gray-400' />
                </div>
                <div className='ml-3'>
                  <div className='text-base font-medium leading-none text-gray-800'>{user.name}</div>
                  <div className='text-sm font-medium text-gray-500'>{user.email}</div>
                </div>
              </div>
              <div className='px-2 mt-3 space-y-1'>
                {userNavigation.map((item) => (
                  <Disclosure.Button
                    key={item.name}
                    as={Link}
                    to={item.to}
                    search={{}}
                    params={{}}
                    className='block px-4 py-2 text-base font-medium text-gray-500 hover:bg-gray-100 hover:text-gray-800'>
                    {item.name}
                  </Disclosure.Button>
                ))}
              </div>
            </div>
          </Disclosure.Panel>
        </>
      )}
    </Disclosure>
  );
};

export default Header;
