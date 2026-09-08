// Utility function to convert an array of objects to CSV format
export const convertToCSV = (data: any[]) => {
  const csvRows: string[] = [];

  // Extract headers
  const headers = Object.keys(data[0]);
  csvRows.push(headers.join(',')); // Push headers as the first row

  // Map through the data and add rows
  data.forEach((row) => {
    const values = headers.map((header) => `"${row[header]}"`);
    csvRows.push(values.join(','));
  });

  return csvRows.join('\n');
};

// Function to trigger download of CSV file
export const downloadCSV = (csvData: string, filename: string) => {
  const blob = new Blob([csvData], { type: 'text/csv' });
  const url = window.URL.createObjectURL(blob);
  const link = document.createElement('a');
  link.href = url;
  link.download = filename;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  window.URL.revokeObjectURL(url); // Clean up the URL
};
