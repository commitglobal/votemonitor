import type { FunctionComponent } from '@/common/types';
import { Link, useParams } from '@tanstack/react-router';
import type { Route } from '@/routes/forms_.$formId.edit';

export function FormDetailsBreadcrumbs(): FunctionComponent {
  const { formId } = useParams<typeof Route>({ strict: false });

  return (
    <div className='breadcrumbs flex flex-row gap-2 mb-4'>
      <Link className='crumb' to='/election-event/$tab' params={{ tab: 'observer-forms' }} preload='intent'>
        observer-forms
      </Link>
      <Link className='crumb'>{formId}</Link>
    </div>
  );
}
