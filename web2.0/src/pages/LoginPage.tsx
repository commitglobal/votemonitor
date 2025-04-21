"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { useAuth } from "@/contexts/auth.context";
import {
  Link,
  useNavigate,
  useRouter,
  useRouterState,
} from "@tanstack/react-router";
import { Route } from "@/routes/(auth)/login";

const formSchema = z.object({
  email: z
    .string()
    .min(1, {
      message: "Email is mandatory",
    })
    .email({ message: "Email format is not correct" }),
  password: z.string().min(1, { message: "Password is mandatory" }),
});

const fallback = "/" as const;

function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const isLoading = useRouterState({ select: (s) => s.isLoading });
  const search = Route.useSearch();
  const router = useRouter();
  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      email: "",
      password: "",
    },
  });

  const isLoggingIn = isLoading || form.formState.isSubmitting;

  const onSubmit = async (values: z.infer<typeof formSchema>) => {
    await login(values.email, values.password);
    await router.invalidate();

    await navigate({ to: search.redirect || fallback });
  };

  return (
    <div className="flex min-h-svh w-full items-center justify-center p-6 md:p-10">
      <div className="w-full max-w-sm">
        <div className="flex flex-col gap-6">
          <Card>
            <CardHeader>
              <CardTitle className="text-2xl">Login</CardTitle>
              <CardDescription>
                Enter your email below to login to your account
              </CardDescription>
            </CardHeader>
            <Form {...form}>
              <form
                id="login-form"
                onSubmit={form.handleSubmit(onSubmit)}
                className="flex flex-col gap-6"
              >
                <CardContent className="grid gap-4">
                  <FormField
                    control={form.control}
                    name="email"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Email</FormLabel>
                        <FormControl>
                          <Input {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="password"
                    render={({ field }) => (
                      <FormItem className="grid gap-2">
                        <div className="flex items-center">
                          <FormLabel>Password</FormLabel>

                          <Button
                            className="ml-auto inline-block text-sm underline-offset-4 hover:underline"
                            variant="link"
                            asChild
                          >
                            <Link to="/forgot-password">
                              Forgot your password?
                            </Link>
                          </Button>
                        </div>
                        <FormControl>
                          <Input type="password" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </CardContent>
                <CardFooter>
                  <Button
                    type="submit"
                    className="w-full cursor-pointer"
                    disabled={isLoggingIn}
                  >
                    {isLoggingIn ? "Loading..." : "Login"}
                  </Button>
                </CardFooter>
              </form>
            </Form>
          </Card>
        </div>
      </div>
    </div>
  );
}

export default LoginPage;
