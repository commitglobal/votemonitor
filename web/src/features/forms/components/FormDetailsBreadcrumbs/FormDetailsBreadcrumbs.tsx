import type { FunctionComponent } from '@/common/types';
import { Link } from '@tanstack/react-router';

export interface FormDetailsBreadcrumbsProps {
  formCode: string;
  formName: string
}

export function FormDetailsBreadcrumbs({ formCode, formName }: FormDetailsBreadcrumbsProps): FunctionComponent {
  return (
    <div className='breadcrumbs flex flex-row gap-2 mb-4'>
      <Link className='crumb' to='/election-event/$tab' params={{ tab: 'observer-forms' }} preload='intent'>
        observer-forms
      </Link>
      <Link className='crumb'>{formCode} - {formName}</Link>
    </div>
  );
}
