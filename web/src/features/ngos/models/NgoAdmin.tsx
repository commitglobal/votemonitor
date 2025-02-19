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
  password: z.string().min(1, { message: 'This field is required' }),
});

export const editNgoAdminSchema = z.object({
  firstName: z.string().min(2, {
    message: 'This field is mandatory',
  }),
  lastName: z.string().min(2, {
    message: 'This field is mandatory',
  }),
  phoneNumber: z.string().min(1, { message: 'This field is required' }),
  status: z.string(),
});

export type NgoAdminFormData = z.infer<typeof ngoAdminSchema>;
export type EditNgoAdminFormData = z.infer<typeof editNgoAdminSchema>;
