import Swal from 'sweetalert2';

// Helper to extract error message from various error formats
export const extractErrorMessage = (error: any): string => {
  if (typeof error === 'string') return error;
  if (error instanceof Error) return error.message;
  if (error?.message) return error.message;
  if (error?.Message) return error.Message;
  if (error?.errors) return Array.isArray(error.errors) ? error.errors.join(', ') : error.errors;
  if (error?.Errors) return Array.isArray(error.Errors) ? error.Errors.join(', ') : error.Errors;
  return 'An unexpected error occurred';
};

// Success notification
export const showSuccess = (title: string, message?: string) => {
  return Swal.fire({
    icon: 'success',
    title,
    text: message,
    confirmButtonColor: '#10b981', // green
    timer: 3000,
  });
};

// Error notification
export const showError = (title: string, message?: string, details?: string[]) => {
  // If details array is provided, show them in a formatted way
  let html = undefined;
  if (details && details.length > 0) {
    html = `
      <p class="text-sm text-gray-700 mb-3">${message || 'The following errors occurred:'}</p>
      <ul class="text-left text-sm text-gray-600 space-y-1">
        ${details.map(err => `<li>• ${err}</li>`).join('')}
      </ul>
    `;
  }

  return Swal.fire({
    icon: 'error',
    title,
    html: html,
    text: html ? undefined : (message || 'An unexpected error occurred'),
    confirmButtonColor: '#ef4444', // red
  });
};

// Confirmation dialog
export const showConfirm = async (
  title: string,
  message: string,
  confirmText = 'Yes, delete it'
): Promise<boolean> => {
  const result = await Swal.fire({
    icon: 'warning',
    title,
    text: message,
    showCancelButton: true,
    confirmButtonColor: '#ef4444',
    cancelButtonColor: '#6b7280',
    confirmButtonText: confirmText,
    cancelButtonText: 'Cancel',
  });
  return result.isConfirmed;
};

// Loading indicator
export const showLoading = (message = 'Processing...') => {
  Swal.fire({
    title: message,
    allowOutsideClick: false,
    didOpen: () => {
      Swal.showLoading();
    },
  });
};

// Close loading
export const closeLoading = () => {
  Swal.close();
};
