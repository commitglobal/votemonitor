/* eslint-disable unicorn/prefer-top-level-await */
import { z } from 'zod';

export interface NGO {
  id: string;
  name: string;
  status: NGOStatus;
  numberOfNgoAdmins: number;
  numberOfElectionsMonitoring: number;
  dateOfLastElection: string;
}

export enum NGOStatus {
  Activated = 'Activated',
  Pending = 'Pending',
  Deactivated = 'Deactivated',
}

export enum NgoAdminStatus {
  Active = 'Active',
  Deactivated = 'Deactivated',
}

export interface NgoAdmin {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  status: NgoAdminStatus;
}

export const ngoAdminSchema = z.object({
  firstName: z.string(),
  lastName: z.string(),
  email: z.string().email(),
  phoneNumber: z.string(),
});

export const newNgoSchema = ngoAdminSchema.extend({ name: z.string() });

export type NGOAdminFormData = z.infer<typeof ngoAdminSchema>;
export type NGOCreationFormData = z.infer<typeof newNgoSchema>;
