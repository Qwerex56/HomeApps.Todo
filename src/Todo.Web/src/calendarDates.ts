import type { CalendarItem } from './api'
import { fromIso, isoDate } from './dates'

export type CalendarView = 'month' | 'week' | 'day' | 'agenda'

export function addDays(date: Date, days: number): Date {
  return new Date(date.getFullYear(), date.getMonth(), date.getDate() + days)
}

/** First day of the week containing the date (local midnight); `first` is 0 Sunday, 1 Monday, 6 Saturday. */
export function startOfWeek(date: Date, first = 1): Date {
  return addDays(date, -((date.getDay() - first + 7) % 7))
}

export function startOfMonth(date: Date): Date {
  return new Date(date.getFullYear(), date.getMonth(), 1)
}

/** The 6x7 grid of a month, weeks starting on the chosen day. */
export function monthGrid(anchor: Date, first = 1): Date[] {
  const start = startOfWeek(startOfMonth(anchor), first)
  return Array.from({ length: 42 }, (_, i) => addDays(start, i))
}

export function weekDays(anchor: Date, first = 1): Date[] {
  const start = startOfWeek(anchor, first)
  return Array.from({ length: 7 }, (_, i) => addDays(start, i))
}

const AGENDA_DAYS = 30

/** The days a view shows, first inclusive. */
export function visibleDays(view: CalendarView, anchor: Date, firstDay = 1): Date[] {
  if (view === 'month') return monthGrid(anchor, firstDay)
  if (view === 'week') return weekDays(anchor, firstDay)
  if (view === 'day') return [anchor]
  return Array.from({ length: AGENDA_DAYS }, (_, i) => addDays(anchor, i))
}

/** The window to ask the API for: local midnight of the first day to local midnight after the last, as ISO instants. */
export function windowOf(view: CalendarView, anchor: Date, firstDay = 1): { from: string; to: string } {
  const days = visibleDays(view, anchor, firstDay)
  const first = days[0]!
  const last = days[days.length - 1]!
  return { from: first.toISOString(), to: addDays(last, 1).toISOString() }
}

export function shift(view: CalendarView, anchor: Date, direction: 1 | -1): Date {
  if (view === 'month') return new Date(anchor.getFullYear(), anchor.getMonth() + direction, 1)
  if (view === 'week') return addDays(anchor, 7 * direction)
  if (view === 'agenda') return addDays(anchor, AGENDA_DAYS * direction)
  return addDays(anchor, direction)
}

/** The local day (YYYY-MM-DD) an item starts on. */
export function startDay(item: CalendarItem): string {
  if (item.date) return item.date
  return isoDate(new Date(item.startUtc!))
}

/** The local day an item ends on; an end at exactly midnight belongs to the day before. */
export function endDay(item: CalendarItem): string {
  if (item.date || !item.endUtc) return startDay(item)
  const end = new Date(item.endUtc)
  const last = end > new Date(item.startUtc!) ? new Date(end.getTime() - 1) : end
  return isoDate(last)
}

export function onDay(item: CalendarItem, day: string): boolean {
  return startDay(item) <= day && day <= endDay(item)
}

/** HH:mm of an item's start; null for all-day events and tasks without a time. */
export function timeLabel(item: CalendarItem, locale = 'pl-PL'): string | null {
  if (item.itemType === 'Task') return item.time ? item.time.slice(0, 5) : null
  if (item.isAllDay || !item.startUtc) return null
  return new Date(item.startUtc).toLocaleTimeString(locale, { hour: '2-digit', minute: '2-digit' })
}

function sortKey(item: CalendarItem): string {
  return timeLabel(item) ?? '00:00'
}

export function itemsOn(items: CalendarItem[], day: string): CalendarItem[] {
  return items.filter((i) => onDay(i, day)).sort((a, b) => sortKey(a).localeCompare(sortKey(b)) || a.title.localeCompare(b.title))
}

const zoneParts = (instant: number, zone: string) => {
  const parts = Object.fromEntries(
    new Intl.DateTimeFormat('en-CA', { timeZone: zone, hourCycle: 'h23', year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit' })
      .formatToParts(new Date(instant))
      .map((p) => [p.type, p.value]),
  ) as Record<string, string>
  return { day: `${parts.year}-${parts.month}-${parts.day}`, time: `${parts.hour}:${parts.minute}`, asUtc: Date.UTC(+parts.year!, +parts.month! - 1, +parts.day!, +parts.hour!, +parts.minute!) }
}

/** Date (YYYY-MM-DD) and HH:mm as typed in the given time zone to an ISO instant. */
export function toInstant(date: string, time: string, zone: string): string {
  const [h, m] = (time || '00:00').split(':').map(Number)
  const [y, mo, d] = date.split('-').map(Number)
  const wanted = Date.UTC(y!, mo! - 1, d!, h ?? 0, m ?? 0)
  let guess = wanted
  for (let i = 0; i < 2; i++) guess = wanted - (zoneParts(guess, zone).asUtc - guess)
  return new Date(guess).toISOString()
}

/** HH:mm of an instant in the given time zone. */
export function localTime(instant: string, zone: string): string {
  return zoneParts(Date.parse(instant), zone).time
}

/** YYYY-MM-DD of an instant in the given time zone. */
export function localDay(instant: string, zone: string): string {
  return zoneParts(Date.parse(instant), zone).day
}
