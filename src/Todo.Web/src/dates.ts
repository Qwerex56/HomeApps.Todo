/** The household's "today" as YYYY-MM-DD: the device's local date (the server has no time zone for a household). */
export function todayIso(now = new Date()): string {
  return isoDate(now)
}

export function isoDate(date: Date): string {
  const pad = (n: number) => String(n).padStart(2, '0')
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}`
}

/** A YYYY-MM-DD string as a local Date (midnight), for the date picker. */
export function fromIso(value: string): Date {
  const [y, m, d] = value.split('-').map(Number)
  return new Date(y ?? 1970, (m ?? 1) - 1, d ?? 1)
}

export type DueState = 'overdue' | 'today' | 'soon' | 'later'

export function dueState(dueDate: string, today = todayIso()): DueState {
  if (dueDate < today) return 'overdue'
  if (dueDate === today) return 'today'
  const days = (fromIso(dueDate).getTime() - fromIso(today).getTime()) / 86_400_000
  return days <= 7 ? 'soon' : 'later'
}

/** "dziś", "jutro", "pt 14.11" style label; time as HH:mm when there is one. */
export function dueLabel(
  dueDate: string,
  dueTime: string | null,
  words: { today: string; tomorrow: string; yesterday: string },
  locale = 'pl-PL',
  today = todayIso(),
): string {
  const days = Math.round((fromIso(dueDate).getTime() - fromIso(today).getTime()) / 86_400_000)
  const day =
    days === 0
      ? words.today
      : days === 1
        ? words.tomorrow
        : days === -1
          ? words.yesterday
          : fromIso(dueDate).toLocaleDateString(locale, { weekday: 'short', day: 'numeric', month: 'numeric' })
  return dueTime ? `${day} ${dueTime.slice(0, 5)}` : day
}
