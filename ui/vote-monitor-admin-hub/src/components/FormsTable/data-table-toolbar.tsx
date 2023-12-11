
import { Button } from "../ui/button"

import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
} from "@/components/ui/dialog"
import { Input } from "../ui/input"
import { Textarea } from "../ui/textarea"
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "../ui/form"
import { useForm } from "react-hook-form"
import { FormInput, ZFormInput } from "@/redux/api/types";

import { zodResolver } from "@hookform/resolvers/zod"
import { useEffect, useState } from "react"
import { useNavigate } from "react-router-dom"
import { useCreateFormMutation } from "@/redux/api/formsApi"
import { toast } from "react-toastify";

export function DataTableToolbar() {

    const navigate = useNavigate();
    const [open, setOpen] = useState(false);

    const form = useForm<FormInput>({
        resolver: zodResolver(ZFormInput),
        defaultValues: {
            code: "",
            description: "",
            languageCode: "",
            questions: []
        },
    })

    const [
        createForm,
        {
            data: newForm,
            error: error,
            isSuccess: isSubmitSuccess,
        },
    ] = useCreateFormMutation();

    const onSubmit = (form: FormInput) => {
        createForm(form);
    }

    useEffect(() => {
        if (isSubmitSuccess && newForm) {
            navigate(`/forms/${newForm.id}/edit`);
        }
    }, [navigate, isSubmitSuccess, newForm?.id]);

    useEffect(() => {
        if (error?.data?.errors?.length > 0) {
            toast.error(error?.data?.errors[0].reason);
        }
    }, [error?.data?.errors?.length]);

    return (
        <Dialog open={open} onOpenChange={setOpen} >
            <DialogTrigger asChild>
                <Button variant="outline">New Form</Button>
            </DialogTrigger>
            <DialogContent className="sm:max-w-[625px]" onPointerDownOutside={(e) => e.preventDefault()}>
                <DialogHeader>
                    <DialogTitle>New Form</DialogTitle>
                    <DialogDescription>
                    </DialogDescription>
                </DialogHeader>

                <Form {...form}>
                    <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-8">
                        <FormField
                            control={form.control}
                            name="code"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Form Code</FormLabel>
                                    <FormControl>
                                        <Input {...field} />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name="languageCode"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Language Code</FormLabel>
                                    <FormControl>
                                        <Input {...field} />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name="description"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Description</FormLabel>
                                    <FormControl>
                                        <Textarea className="resize-none" {...field} />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <Button type="submit">Submit</Button>
                    </form>
                </Form>
            </DialogContent>
        </Dialog>
    )
}