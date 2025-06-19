import { LanguageSelector } from '@/components/LanguageSelector/LanguageSelector';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuLabel,
  DropdownMenuRadioGroup,
  DropdownMenuRadioItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Skeleton } from '@/components/ui/skeleton';
import { useAuth } from '@/context/auth-context';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { ElectionEvent } from '@/features/election-event/models/election-event';
import { electionRoundKeys } from '@/features/election-rounds/queries';
import { staticDataKeys } from '@/hooks/query-keys';
import { sleep } from '@/lib/utils';
import { queryClient } from '@/main';
import API from '@/services/api';
import { Disclosure, Menu, Transition } from '@headlessui/react';
import { Bars3Icon, ChevronDownIcon, XMarkIcon } from '@heroicons/react/24/outline';
import { PauseCircleIcon, PlayCircleIcon, StopCircleIcon, UserCircleIcon } from '@heroicons/react/24/solid';
import { useQuery } from '@tanstack/react-query';
import { Link, useNavigate, useRouter } from '@tanstack/react-router';
import clsx from 'clsx';
import { sortBy } from 'lodash';
import { Fragment, useEffect, useMemo, useState } from 'react';
import { ElectionRoundStatus, type FunctionComponent } from '../../../common/types';
import Logo from './Logo';

const navigation = [
  { name: 'Dashboard', to: '/', roles: ['PlatformAdmin', 'NgoAdmin'] },
  { name: 'Election rounds', to: '/election-rounds', roles: ['PlatformAdmin'] },
  { name: 'NGOs', to: '/ngos', roles: ['PlatformAdmin'] },
  { name: 'Observers', to: '/observers', roles: ['PlatformAdmin'] },
  { name: 'Form templates', to: '/form-templates', roles: 'PlatformAdmin' },
  { name: 'Election event', to: '/election-event', roles: ['NgoAdmin'] },
  { name: 'Observers', to: '/monitoring-observers', roles: ['NgoAdmin'] },
  { name: 'Responses', to: '/responses', roles: ['NgoAdmin'] },
];
const userNavigation: { name: string; to: string }[] = [];

const Header = (): FunctionComponent => {
  const { isPlatformAdmin, userRole, logout } = useAuth();
  const navigate = useNavigate();
  const [selectedElectionRound, setSelectedElection] = useState<ElectionEvent>();
  const router = useRouter();
  const { setCurrentElectionRoundId, currentElectionRoundId } = useCurrentElectionRoundStore((s) => s);

  const handleSelectElectionRound = async (electionRound?: ElectionEvent): Promise<void> => {
    if (electionRound && selectedElectionRound?.id != electionRound.id) {
      setSelectedElection(electionRound);
      setCurrentElectionRoundId(electionRound.id);

      sleep(1);

      await queryClient.invalidateQueries({
        predicate: (query) => {
          if (query.queryKey === electionRoundKeys.all) {
            return false;
          }

          if (query.queryKey[0] === staticDataKeys.all[0]) {
            return false;
          }

          return true;
        },
      });

      router.invalidate();
    }
  };

  const { status, data: electionRounds } = useQuery({
    queryKey: electionRoundKeys.all,
    queryFn: async () => {
      const response = await API.get<{ electionRounds: ElectionEvent[] }>('/election-rounds:monitoring');

      (response.data.electionRounds ?? []).forEach((er) => {
        queryClient.setQueryData(electionRoundKeys.detail(er.id), er);
      });
      return response.data.electionRounds ?? [];
    },
    staleTime: 0,
    refetchOnWindowFocus: false,
    enabled: !isPlatformAdmin,
  });

  useEffect(() => {
    if (!!electionRounds) {
      const electionRound = electionRounds.find((x) => x.id === currentElectionRoundId);
      handleSelectElectionRound(electionRound ?? electionRounds[0]);
    }
  }, [electionRounds]);

  const activeElections = useMemo(() => {
    return sortBy(
      [...(electionRounds ?? [])].filter((er) => er.status !== ElectionRoundStatus.Archived),
      (er) => new Date(er.startDate).getTime(),
      (er) => er.title
    );
  }, [electionRounds]);

  const archivedElections = useMemo(() => {
    return sortBy(
      [...(electionRounds ?? [])].filter((er) => er.status === ElectionRoundStatus.Archived),
      (er) => new Date(er.startDate).getTime(),
      (er) => er.title
    );
  }, [electionRounds]);

  return (
    <Disclosure as='nav' className='mb-10 bg-white shadow-sm'>
      {({ open }) => (
        <>
          <div className='container'>
            <div className='flex items-center justify-between h-16 gap-6 md:gap-10'>
              <Link to='/'>
                <Logo width={48} height={48} />
              </Link>

              <div className='items-baseline flex-1 hidden gap-4 md:flex'>
                {navigation
                  .filter((nav) => nav.roles.includes(userRole ?? 'Unknown'))
                  .map((item) => (
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

              <div className='items-center hidden gap-2 md:flex'>
                {userRole !== 'NgoAdmin' ? (
                  <></>
                ) : status === 'pending' ? (
                  <Skeleton className='w-[360px] h-[26px] mr-2 rounded-lg bg-secondary-300 text-secondary-900 hover:bg-secondary-300/90' />
                ) : (
                  <DropdownMenu>
                    <DropdownMenuTrigger>
                      <Badge className='max-w-[360px] bg-secondary-300 text-secondary-900 hover:bg-secondary-300/90'>
                        <div className='flex items-center justify-between gap-2 w-full'>
                          <div className='truncate max-w-[300px]' title={selectedElectionRound?.title}>
                            {selectedElectionRound?.title}
                          </div>
                          <ChevronDownIcon className='w-[20px] ml-2' />
                        </div>
                      </Badge>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent>
                      <DropdownMenuRadioGroup
                        value={selectedElectionRound?.id ?? ''}
                        onValueChange={(value) => {
                          const electionRound = electionRounds?.find((er) => er.id === value);
                          handleSelectElectionRound(electionRound);
                        }}>
                        <DropdownMenuLabel> Upcomming elections </DropdownMenuLabel>

                        {activeElections?.map((electionRound) => (
                          <DropdownMenuRadioItem key={electionRound.id} value={electionRound.id}>
                            <div className='flex items-center gap-2'>
                              {electionRound?.status === ElectionRoundStatus.NotStarted ? (
                                <PauseCircleIcon className='w-4 h-4 text-slate-700' />
                              ) : null}
                              {electionRound?.status === ElectionRoundStatus.Started ? (
                                <PlayCircleIcon className='w-4 h-4 text-green-700' />
                              ) : null}
                              <div className='truncate max-w-[340px]' title={electionRound.title}>
                                {electionRound.title}
                              </div>
                            </div>
                          </DropdownMenuRadioItem>
                        ))}
                        <DropdownMenuSeparator />
                        <DropdownMenuLabel> Archived elections </DropdownMenuLabel>
                        {archivedElections?.map((electionRound) => (
                          <DropdownMenuRadioItem key={electionRound.id} value={electionRound.id}>
                            <div className='flex items-center gap-2'>
                              <StopCircleIcon className='w-4 h-4 text-yellow-700' />

                              <div className='truncate max-w-[340px]' title={electionRound.title}>
                                {electionRound.title}
                              </div>
                            </div>
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

                      <Menu.Item key='language-selector'>
                        <LanguageSelector />
                      </Menu.Item>

                      <Menu.Item key='sign-out'>
                        <Button
                          type='button'
                          variant='link'
                          onClick={() => {
                            logout();
                            navigate({ to: '/login' });
                          }}>
                          Sign out
                        </Button>
                      </Menu.Item>
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
              {navigation
                .filter((nav) => nav.roles.includes(userRole ?? 'Unknown'))
                .map((item) => (
                  <Disclosure.Button
                    key={item.name}
                    as={Link}
                    to={item.to}
                    className='block px-3 py-2 text-base font-medium rounded-md'
                    activeProps={{
                      className: 'bg-primary-100 text-primary-600 cursor-default',
                      'aria-current': 'page',
                    }}
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
                  <div className='text-base font-medium leading-none text-gray-800'>{selectedElectionRound?.title}</div>
                </div>
              </div>
              <div className='px-2 mt-3 space-y-1'>
                {userNavigation.map((item) => (
                  <Disclosure.Button
                    key={item.name}
                    as={Link}
                    to={item.to}
                    className='block px-4 py-2 text-base font-medium text-gray-500 hover:bg-gray-100 hover:text-gray-800'>
                    {item.name}
                  </Disclosure.Button>
                ))}

                <LanguageSelector />
                <Disclosure.Button
                  key='Sign Out'
                  as={Button}
                  onClick={() => {
                    logout();
                    navigate({ to: '/login' });
                  }}
                  variant='link'
                  className='block px-4 py-2 text-base font-medium text-gray-500 hover:bg-gray-100 hover:text-gray-800'>
                  Sign Out
                </Disclosure.Button>
              </div>
            </div>
          </Disclosure.Panel>
        </>
      )}
    </Disclosure>
  );
};

export default Header;
