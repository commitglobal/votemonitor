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
  firstName: z
    .string()
    .min(1, { message: 'First name is required' })
    .max(256, { message: 'First name cannot exceed 256 characters' }),
  lastName: z
    .string()
    .min(1, { message: 'Last name is required' })
    .max(256, { message: 'Last name cannot exceed 256 characters' }),
  email: z
    .string()
    .email({ message: 'Invalid email format' })
    .max(256, { message: 'Email cannot exceed 256 characters' }),
  phoneNumber: z.string().max(256, { message: 'Phone number cannot exceed 256 characters' }).optional(),
  password: z
    .string()
    .min(1, { message: 'Password is required' })
    .max(256, { message: 'Password cannot exceed 256 characters' }),
});

export const editNgoAdminSchema = z.object({
  firstName: z
    .string()
    .min(2, { message: 'First name must be at least 2 characters' })
    .max(256, { message: 'First name cannot exceed 256 characters' }),
  lastName: z
    .string()
    .min(2, { message: 'Last name must be at least 2 characters' })
    .max(256, { message: 'Last name cannot exceed 256 characters' }),
  phoneNumber: z.coerce.string().max(256, { message: 'Phone number cannot exceed 256 characters' }).optional(),
});

export type NgoAdminFormData = z.infer<typeof ngoAdminSchema>;
export type EditNgoAdminFormData = z.infer<typeof editNgoAdminSchema>;
