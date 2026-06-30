// Maps fetch/network failures to Arabic user-facing messages via LanguageContext keys.

export function isNetworkError(err: unknown): boolean {
  if (!navigator.onLine) return true;
  if (err instanceof TypeError) return true;
  if (err instanceof Error) {
    const m = err.message.toLowerCase();
    return m.includes('failed to fetch') || m.includes('network') || m.includes('load failed');
  }
  return false;
}

export function toUserErrorMessage(err: unknown, t: (key: string) => string): string {
  if (!navigator.onLine) return t('network.offline');
  if (isNetworkError(err)) return t('network.connection.failed');
  if (err instanceof Error) {
    if (/unauthorized/i.test(err.message)) return t('login.required');
    return err.message;
  }
  return t('error');
}
