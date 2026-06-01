import Swal from 'sweetalert2';
import { AR } from '../i18n/ar';

// Helper to extract error message from various error formats
export const extractErrorMessage = (error: unknown): string => {
  if (typeof error === 'string') return error;
  if (error instanceof Error) return error.message;
  const e = error as { message?: string; Message?: string; errors?: string | string[]; Errors?: string | string[] };
  if (e?.message) return e.message;
  if (e?.Message) return e.Message;
  if (e?.errors) return Array.isArray(e.errors) ? e.errors.join('، ') : e.errors;
  if (e?.Errors) return Array.isArray(e.Errors) ? e.Errors.join('، ') : e.Errors;
  return 'حدث خطأ غير متوقع';
};

// Success notification
export const showSuccess = (title: string, message?: string) => {
  return Swal.fire({
    icon: 'success',
    title,
    text: message,
    confirmButtonColor: '#007a3d', // Sudan green
    confirmButtonText: 'حسناً',
    timer: 3000,
  });
};

// Error notification
export const showError = (title: string, message?: string, details?: string[]) => {
  let html: string | undefined;
  if (details && details.length > 0) {
    html = `
      <p class="text-sm text-gray-700 mb-3">${message || AR.common.errorTitle}</p>
      <ul class="text-right text-sm text-gray-600 space-y-1" dir="rtl">
        ${details.map((err) => `<li>• ${err}</li>`).join('')}
      </ul>
    `;
  }

  return Swal.fire({
    icon: 'error',
    title,
    html,
    text: html ? undefined : message || 'حدث خطأ غير متوقع',
    confirmButtonColor: '#d21034', // Sudan red
    confirmButtonText: 'حسناً',
  });
};

// Confirmation dialog
export const showConfirm = async (
  title: string,
  message: string,
  confirmText: string = AR.common.yesDelete
): Promise<boolean> => {
  const result = await Swal.fire({
    icon: 'warning',
    title,
    text: message,
    showCancelButton: true,
    confirmButtonColor: '#d21034',
    cancelButtonColor: '#475569',
    confirmButtonText: confirmText,
    cancelButtonText: AR.common.cancel,
    reverseButtons: true,
  });
  return result.isConfirmed;
};

// Loading indicator
export const showLoading = (message: string = AR.common.processing) => {
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
