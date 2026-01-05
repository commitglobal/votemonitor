import { formEditOpts } from "@/components/form-builder/shared";
import {
    FieldGroup
} from "@/components/ui/field";
import { withForm } from "@/hooks/form";

export const FormDescription = withForm({
  ...formEditOpts,
  render: ({ form }) => {
    return (
        <FieldGroup>
        <div className='grid grid-cols-1 gap-4 md:grid-cols-3'>
          <div className='md:col-span-1'>
            <form.AppField name='code'>
              {(field) => {
                return (
                  <field.TextInput id='code' label='Form Code' required />
                )
              }}
            </form.AppField>
          </div>
          <div className='md:col-span-2'>
            <form.AppField name='name'>
              {(field) => (
                <field.TextInput id='name' label='Form Name' required />
              )}
            </form.AppField>
          </div>
        </div>

        <div className='grid grid-cols-1 gap-4 md:grid-cols-2'>
          <div className='md:col-span-1'>
            <form.AppField name='defaultLanguage'>
              {(field) => {
                return (
                  <field.LanguageSelect
                    label='Base language'
                    id='language'
                    description='The base language of the form'
                    required
                  />
                )
              }}
            </form.AppField>
          </div>
          <div className='md:col-span-1'>
            <form.AppField name='formType'>
              {(field) => {
                return (
                  <field.FormTypeSelect
                    label='Form Type'
                    id='formType'
                    required
                  />
                )
              }}
            </form.AppField>
          </div>
        </div>

        <form.AppField name='description'>
          {(field) => {
            return (
              <field.Textarea label='Form Description' id='description' />
            )
          }}
        </form.AppField>
      </FieldGroup>
    );
  },
});
