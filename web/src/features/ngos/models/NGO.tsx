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

export const newNgoSchema = z.object({
  firstName: z.string(),
  lastName: z.string(),
  email: z.string().email(),
  phoneNumber: z.string(),
  password: z.string().min(1, { message: 'This field is required' }),
  name: z.string(),
});

export type NgoCreationFormData = z.infer<typeof newNgoSchema>;
export const editNgoSchema = z.object({
  name: z.string().min(2, {
    message: 'This field is mandatory',
  }),
  status: z.string(),
});

export type EditNgoFormData = z.infer<typeof editNgoSchema>;
