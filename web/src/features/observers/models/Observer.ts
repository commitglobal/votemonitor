/* eslint-disable unicorn/prefer-top-level-await */
import { z } from 'zod';

export interface Observer {
  id: string;
  name: string;
  email: string;
  status: string;
  phoneNumber: string;
}

export const observerRouteSearchSchema = z.object({
  nameFilter: z.string().catch(''),
  pageNumber: z.number().catch(1),
  pageSize: z.number().catch(10),
  status: z.enum(['Active', 'Inactive']).catch('Active'),
});
