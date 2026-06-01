/** Parse backend TimeSpan ("HH:mm:ss") or minutes number to total minutes. */
export function parseOffsetToMinutes(offset: number | string | null | undefined): number | null {
  if (offset == null || offset === '') return null;
  if (typeof offset === 'number' && !Number.isNaN(offset)) return offset;

  if (typeof offset === 'string') {
    // .NET TimeSpan long form: "d.HH:mm:ss"
    const daySplit = offset.split('.');
    let dayMinutes = 0;
    let timePart = offset;
    if (daySplit.length === 2 && daySplit[1].includes(':')) {
      dayMinutes = (parseInt(daySplit[0], 10) || 0) * 24 * 60;
      timePart = daySplit[1];
    }

    const parts = timePart.split(':').map((p) => parseInt(p, 10));
    if (parts.some((n) => Number.isNaN(n))) return null;

    if (parts.length === 3) {
      return dayMinutes + parts[0] * 60 + parts[1] + parts[2] / 60;
    }
    if (parts.length === 2) {
      return dayMinutes + parts[0] * 60 + parts[1];
    }
  }

  return null;
}

/** Add minutes (or TimeSpan string) to an ISO datetime; never throws. */
export function addMinutesToIso(iso: string | undefined | null, offset: number | string | null | undefined): string {
  if (!iso) return iso ?? '';
  const minutes = parseOffsetToMinutes(offset);
  if (minutes == null) return iso;

  const t = new Date(iso).getTime();
  if (Number.isNaN(t)) return iso;

  const result = new Date(t + minutes * 60_000);
  if (Number.isNaN(result.getTime())) return iso;
  return result.toISOString();
}

export function formatTimeSafe(iso: string | undefined | null): string {
  if (!iso) return '--:--';
  const d = new Date(iso);
  if (Number.isNaN(d.getTime())) return '--:--';
  try {
    return d.toLocaleTimeString('ar-EG', { hour: '2-digit', minute: '2-digit' });
  } catch {
    return '--:--';
  }
}

export function formatDateSafe(iso: string | undefined | null): string {
  if (!iso) return '--';
  const d = new Date(iso);
  if (Number.isNaN(d.getTime())) return '--';
  try {
    return d.toLocaleDateString('ar-EG', { weekday: 'short', year: 'numeric', month: 'short', day: 'numeric' });
  } catch {
    return '--';
  }
}

export function formatDurationSafe(startIso: string, endIso: string): string {
  const start = new Date(startIso).getTime();
  const end = new Date(endIso).getTime();
  if (Number.isNaN(start) || Number.isNaN(end) || end < start) return '--';
  const mins = Math.round((end - start) / 60000);
  return `${Math.floor(mins / 60)}h ${mins % 60}m`;
}

/** "YYYY-MM-DD" from <input type="date"> compared to local calendar today. */
export function isTodayDateString(dateStr: string): boolean {
  if (!dateStr) return false;
  const now = new Date();
  const y = now.getFullYear();
  const m = String(now.getMonth() + 1).padStart(2, '0');
  const d = String(now.getDate()).padStart(2, '0');
  return dateStr === `${y}-${m}-${d}`;
}

/** True when search date is before today (local). */
export function isPastDateString(dateStr: string): boolean {
  if (!dateStr) return false;
  const [y, m, d] = dateStr.split('-').map((n) => parseInt(n, 10));
  if (!y || !m || !d) return false;
  const searchDay = new Date(y, m - 1, d);
  const today = new Date();
  searchDay.setHours(0, 0, 0, 0);
  today.setHours(0, 0, 0, 0);
  return searchDay < today;
}

/** Segment/trip departure already passed (local clock). */
export function isDepartureInPast(departureISO: string): boolean {
  const dep = new Date(departureISO).getTime();
  if (Number.isNaN(dep)) return false;
  return dep <= Date.now();
}

/** Hide trip when searching today and departure time has passed, or date is in the past. */
export function shouldShowTripForSearchDate(searchDate: string | undefined, departureISO: string): boolean {
  if (!searchDate) return true;
  if (isPastDateString(searchDate)) return false;
  if (isTodayDateString(searchDate) && isDepartureInPast(departureISO)) return false;
  return true;
}
