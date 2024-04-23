import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { ChangeEvent, ReactElement, useState } from 'react';


export default function NgoAdminDashboard(): ReactElement {
  return (
    <Layout title='Responses' subtitle='View all form answers and other issues reported by your observers.  '>
      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-2">
      <Card>
        <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle className="text-base font-normal">Total Revenue</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="text-2xl font-bold">$15,231.89</div>
          <p className="text-xs text-muted-foreground">
            +20.1% from last month
          </p>
          <div className="h-[80px]">
           
          </div>
        </CardContent>
      </Card>
      <Card>
        <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle className="text-base font-normal">Subscriptions</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="text-2xl font-bold">+2350</div>
          <p className="text-xs text-muted-foreground">
            +180.1% from last month
          </p>
          <div className="mt-4 h-[80px]">
        
          </div>
        </CardContent>
      </Card>
    </div>
    </Layout>
  );
}
