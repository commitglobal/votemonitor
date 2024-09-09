import { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Button } from '@/components/ui/button';
import {
    Dialog,
    DialogClose,
    DialogContent,
    DialogFooter,
    DialogHeader,
    DialogTitle,
} from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { toast } from '@/components/ui/use-toast';
import { authApi } from '@/common/auth-api';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { queryClient } from '@/main';

export interface AddObserverDialogProps {
    open: boolean;
    onOpenChange: (open: boolean) => void;
}

function AddObserverDialog({ open, onOpenChange }: AddObserverDialogProps) {
    const [observer, setObserver] = useState({ firstName: '', lastName: '', email: '', phone: '' });
    const [errors, setErrors] = useState<{ [key: string]: string }>({});
    const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setObserver((prev) => ({ ...prev, [name]: value }));
        setErrors((prevErrors) => ({ ...prevErrors, [name]: '' })); // Clear error on input change
    };

    const validateInputs = () => {
        const newErrors: { [key: string]: string } = {};
        if (!observer['firstName'].trim()) newErrors['firstName'] = 'First name is required';
        if (!observer['lastName'].trim()) newErrors['lastName'] = 'Last name is required';
        if (!observer['email'].trim()) newErrors['email'] = 'Email is required';
        if (!observer['phone'].trim()) newErrors['phone'] = 'Phone number is required';
        else if (!/^\d{10,15}$/.test(observer['phone'])) newErrors['phone'] = 'Phone number is not valid';
        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const addObserverMutation = useMutation({
        mutationFn: async ({ electionRoundId, formData }: { electionRoundId: string; formData: FormData }) => {
            return await authApi.post(`/election-rounds/${electionRoundId}/monitoring-observers`, formData, {
                headers: {
                    'Content-Type': 'multipart/form-data',
                },
            });
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['monitoring-observers'] });
            onOpenChange(false);
            toast({
                title: 'Success',
                description: 'Observer added successfully',
            });
        },
        onError: (error: any) => {
            console.error('Error adding observer:', error); // Log the error for debugging
            const errorMessage = error.response?.data?.message || 'Failed to add observer. Please try again.';
            toast({
                title: 'Error adding observer',
                description: errorMessage,
                variant: 'destructive',
            });
        },
    });

    const handleSubmit = () => {
        if (validateInputs()) {
            const formData = new FormData();
            formData.append('firstName', observer.firstName);
            formData.append('lastName', observer.lastName);
            formData.append('email', observer.email);
            formData.append('phone', observer.phone);
            addObserverMutation.mutate({ electionRoundId: currentElectionRoundId, formData });
        } else {
            toast({
                title: 'Error',
                description: 'Please fill in all required fields correctly.',
                variant: 'destructive',
            });
        }
    };

    return (
        <Dialog open={open} onOpenChange={onOpenChange} modal={true}>
            <DialogContent className='max-w-lg'>
                <DialogHeader>
                    <DialogTitle>Add individual observer</DialogTitle>
                </DialogHeader>
                <div className='space-y-4 mt-4'>
                    {/* First Name Field */}
                    <div>
                        <label htmlFor='firstName' className='block text-sm font-medium text-gray-700'>
                            First name <span className='text-red-500'>*</span>
                        </label>
                        <Input
                            id='firstName'
                            type='text'
                            name='firstName'
                            placeholder='Enter first name'
                            value={observer['firstName']}
                            onChange={handleInputChange}
                            className={`mt-1 block w-full ${errors['firstName'] ? 'border-red-500' : ''}`}
                        />
                        {errors['firstName'] && <p className='text-red-500 text-sm'>{errors['firstName']}</p>}
                    </div>

                    {/* Last Name Field */}
                    <div>
                        <label htmlFor='lastName' className='block text-sm font-medium text-gray-700'>
                            Last name <span className='text-red-500'>*</span>
                        </label>
                        <Input
                            id='lastName'
                            type='text'
                            name='lastName'
                            placeholder='Enter last name'
                            value={observer['lastName']}
                            onChange={handleInputChange}
                            className={`mt-1 block w-full ${errors['lastName'] ? 'border-red-500' : ''}`}
                        />
                        {errors['lastName'] && <p className='text-red-500 text-sm'>{errors['lastName']}</p>}
                    </div>

                    {/* Email Field */}
                    <div>
                        <label htmlFor='email' className='block text-sm font-medium text-gray-700'>
                            Email <span className='text-red-500'>*</span>
                        </label>
                        <Input
                            id='email'
                            type='email'
                            name='email'
                            placeholder='Introduce email address of the observer'
                            value={observer['email']}
                            onChange={handleInputChange}
                            className={`mt-1 block w-full ${errors['email'] ? 'border-red-500' : ''}`}
                        />
                        {errors['email'] && <p className='text-red-500 text-sm'>{errors['email']}</p>}
                    </div>

                    {/* Phone Number Field */}
                    <div>
                        <label htmlFor='phone' className='block text-sm font-medium text-gray-700'>
                            Phone number <span className='text-red-500'>*</span>
                        </label>
                        <Input
                            id='phone'
                            type='text'
                            name='phone'
                            placeholder='Enter phone number'
                            value={observer['phone']}
                            onChange={handleInputChange}
                            className={`mt-1 block w-full ${errors['phone'] ? 'border-red-500' : ''}`}
                        />
                        {errors['phone'] && <p className='text-red-500 text-sm'>{errors['phone']}</p>}
                    </div>
                </div>
                <DialogFooter className='flex justify-between mt-6'>
                    <DialogClose asChild>
                        <Button variant='outline'>Cancel</Button>
                    </DialogClose>
                    <Button className='bg-purple-900 hover:bg-purple-600' onClick={handleSubmit}>
                        Add observer
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
}

export default AddObserverDialog;
