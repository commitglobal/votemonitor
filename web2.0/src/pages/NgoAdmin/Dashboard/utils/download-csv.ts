/**
 * Byte order mark.
 *
 * Excel reads a CSV as UTF-8 only when the file starts with it; without it,
 * diacritics in location names come out mangled. Written as an escape rather
 * than a literal, which would sit invisible in the source.
 */
const BYTE_ORDER_MARK = '\uFEFF'

/**
 * Escapes one CSV field.
 *
 * Location paths routinely contain commas, and any of them would otherwise
 * split a row into extra columns when the file is opened in a spreadsheet.
 */
const escapeField = (value: string | number): string => {
  const text = String(value ?? '')

  return /[",\n]/.test(text) ? `"${text.replace(/"/g, '""')}"` : text
}

/** Saves `rows` as a CSV file, with the given column order and header. */
export function downloadCsv(
  columns: { key: string; label: string }[],
  rows: Record<string, string | number>[],
  fileName: string
): void {
  const header = columns.map((column) => escapeField(column.label)).join(',')
  const body = rows
    .map((row) =>
      columns.map((column) => escapeField(row[column.key] ?? '')).join(',')
    )
    .join('\n')

  const blob = new Blob([`${BYTE_ORDER_MARK}${header}\n${body}`], {
    type: 'text/csv;charset=utf-8;',
  })

  const url = URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.download = fileName
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
  URL.revokeObjectURL(url)
}
