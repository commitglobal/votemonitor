import React, { ReactElement, ReactNode } from "react";

interface LayoutProps {
    title: string;
    children: ReactNode;
}

const Layout = ({ title, children }: LayoutProps) => {
  return (
    <>
      <header className='container py-6'>
        <h1 className='text-3xl font-bold tracking-tight text-gray-900'>{title}</h1>
      </header>
      <main className='container py-6'>
        {children}
      </main>
    </>
  );
}

export default Layout;
