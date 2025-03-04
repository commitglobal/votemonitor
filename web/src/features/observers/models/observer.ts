import { z } from 'zod';

export enum ObserverStatus {
  Active = 'Active',
  Pending = 'Pending',
  Deactivated = 'Deactivated',
}

export interface Observer {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  status: ObserverStatus;
  phoneNumber: string;
}

export const observerFormSchema = z.object({
  lastName: z.string().min(1, {
    message: 'Last name must be at least 1 characters long',
  }),
  firstName: z.string().min(1, {
    message: 'First name must be at least 1 characters long',
  }),
  email: z.string().min(1, { message: 'Email is required' }).email('Please enter a valid email address'),
  phoneNumber: z.string(),
});

export type ObserverFormData = z.infer<typeof observerFormSchema>;
