import type { FieldErrors } from 'react-hook-form';
import { toast } from 'sonner';
import type { EditFormType } from '@/components/FormEditor/FormEditor';

const MAX_LISTED_ERRORS = 5;

const ERROR_METADATA_KEYS = new Set(['message', 'type', 'types', 'ref', 'root']);

const FIELD_LABELS: Record<string, string> = {
  name: 'Name',
  description: 'Description',
  code: 'Code',
  languages: 'Languages',
  languageCode: 'Language',
  defaultLanguage: 'Default language',
  text: 'Question text',
  helptext: 'Help text',
  inputPlaceholder: 'Placeholder',
  lowerLabel: 'Lower label',
  upperLabel: 'Upper label',
  options: 'Options',
  parentQuestionId: 'Display logic question',
  condition: 'Display logic condition',
  value: 'Display logic value',
  optionIds: 'Display logic options',
};

function collectErrorMessages(errors: unknown, path: string[], result: { path: string[]; message: string }[]) {
  if (!errors || typeof errors !== 'object') {
    return;
  }

  const error = errors as Record<string, unknown>;
  if (typeof error['message'] === 'string' && error['message']) {
    result.push({ path, message: error['message'] });
  }

  if (error['root']) {
    collectErrorMessages(error['root'], path, result);
  }

  Object.entries(error)
    .filter(([key]) => !ERROR_METADATA_KEYS.has(key))
    .forEach(([key, value]) => collectErrorMessages(value, [...path, key], result));
}

function describePath(path: string[], values: EditFormType): string {
  const [first, index, ...rest] = path;

  if (first === 'questions' && index !== undefined) {
    const questionCode = values.questions[Number(index)]?.code;
    const question = `Question ${Number(index) + 1}${questionCode ? ` (${questionCode})` : ''}`;
    return [question, ...describeFields(rest)].join(' › ');
  }

  return describeFields(path).join(' › ');
}

function describeFields(path: string[]): string[] {
  return path.map((segment, i) => {
    // Numeric segments after options refer to an option position.
    if (/^\d+$/.test(segment)) {
      return path[i - 1] === 'options' ? `#${Number(segment) + 1}` : segment;
    }

    return FIELD_LABELS[segment] ?? segment;
  });
}

/**
 * Shows an error toast listing the validation errors that prevented the form from being saved.
 */
export function toastFormValidationErrors(errors: FieldErrors<EditFormType>, values: EditFormType): void {
  const messages: { path: string[]; message: string }[] = [];
  collectErrorMessages(errors, [], messages);

  const lines = messages.map(({ path, message }) => {
    const location = describePath(path, values);
    return location ? `${location}: ${message}` : message;
  });
  const uniqueLines = [...new Set(lines)];
  const listed = uniqueLines.slice(0, MAX_LISTED_ERRORS);
  const remaining = uniqueLines.length - listed.length;

  toast.error('Form could not be saved. Please fix the following errors:', {
    description: (
      [...listed, ...(remaining > 0 ? [`…and ${remaining} more`] : [])].join('\n')
    ),
    duration: 10000,
    classNames: { description: 'whitespace-pre-line' },
  });
}
