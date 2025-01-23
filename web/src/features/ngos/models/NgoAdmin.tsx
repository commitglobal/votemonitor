import { z } from 'zod';

export enum NgoAdminStatus {
  Active = 'Active',
  Deactivated = 'Deactivated',
}

export type NgoAdminGetRequestParams = { ngoId: string; adminId: string };

export interface NgoAdmin {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  status: NgoAdminStatus;
  phoneNumber: string;
  latestActivityAt: any;
}

export const ngoAdminSchema = z.object({
  firstName: z.string(),
  lastName: z.string(),
  email: z.string().email(),
  phoneNumber: z.string(),
});
