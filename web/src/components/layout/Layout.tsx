import type { ReactNode } from 'react';

interface LayoutProps {
  title: string;
  breadcrumbs?: ReactNode;
  actions?: ReactNode;
  children: ReactNode;
}

const Layout = ({ title, breadcrumbs, actions, children }: LayoutProps) => {
  return (
    <>
      <header className='container py-4'>
        <div className='flex flex-col justify-between gap-4 md:items-center md:flex-row'>
          <h1 className='text-3xl font-bold tracking-tight text-gray-900'>{title}</h1>
          {!!actions && <div className='flex shrink-0'>{actions}</div>}
        </div>
      </header>
      <main className='container'>{children}</main>
    </>
  );
};

export default Layout;
