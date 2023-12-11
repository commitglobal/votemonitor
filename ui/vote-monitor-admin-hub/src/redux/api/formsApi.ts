import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { FormModel, FormInput } from "./types";

const BASE_URL = import.meta.env.VITE_SERVER_ENDPOINT as string;

export interface PagedResult<T> {
  currentPage: number
  pageSize: number
  totalCount: number
  items: T[]
}

export interface HasId<T> {
  id: T;
}

export const formsApi = createApi({
  reducerPath: "formsApi",
  baseQuery: fetchBaseQuery({
    baseUrl: `${BASE_URL}/api/`,
  }),
  tagTypes: [
    "Forms"
  ],
  endpoints: (builder) => ({
    getForms: builder.query<PagedResult<FormModel>, void>({
      query() {
        return {
          url: `forms`
        };
      },
      providesTags: (result, error, page) =>
        result
          ? [
            // Provides a tag for each post in the current page,
            // as well as the 'PARTIAL-LIST' tag.
            ...result.items.map(({ id }) => ({ type: 'Forms' as const, id })),
            { type: 'Forms', id: 'PARTIAL-LIST' },
          ]
          : [{ type: 'Forms', id: 'PARTIAL-LIST' }],
    }),

    getForm: builder.query<FormModel, string>({
      query(formId) {
        return {
          url: `forms/${formId}`
        };
      },
      providesTags: (result, error, page) => [{ type: 'Forms', id: result!.id }],
    }),
    createForm: builder.mutation<FormModel, FormInput>({
      query(newForm) {
        return {
          method: "POST",
          url: "forms",
          body: newForm,
        };
      },
      invalidatesTags: () => [{ type: 'Forms', id: 'PARTIAL-LIST' }],
    }),
    updateForm: builder.mutation<FormModel, FormInput & HasId<string>>({
      query(form) {
        return {
          method: "PUT",
          url: `forms/${form.id}`,
          body: form
        };
      },
      invalidatesTags: (result, error, request) => [
        { type: 'Forms', id: request.id },
        { type: 'Forms', id: 'PARTIAL-LIST' },
      ],
    }),
    deleteForm: builder.mutation<{ success: boolean; id: string }, string>({
      query(formId) {
        return {
          method: "DELETE",
          url: `forms/${formId}`
        };
      },
      invalidatesTags: (result, error, id) => [
        { type: 'Forms', id: id },
        { type: 'Forms', id: 'PARTIAL-LIST' },
      ],
    }),

  }),
});

export const {
  useGetFormsQuery,
  useGetFormQuery,
  useCreateFormMutation,
  useUpdateFormMutation,
  useDeleteFormMutation
} = formsApi;
