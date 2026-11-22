import type { CalendarItem } from './api'
import { fromIso, isoDate } from './dates'

export type CalendarView = 'month' | 'week' | 'day' | 'agenda'

export function addDays(date: Date, days: number): Date {
  return new Date(date.getFullYear(), date.getMonth(), date.getDate() + days)
}

/** Monday of the week containing the date (local midnight). */
export function startOfWeek(date: Date): Date {
  return addDays(date, -((date.getDay() + 6) % 7))
}

export function startOfMonth(date: Date): Date {
  return new Date(date.getFullYear(), date.getMonth(), 1)
}

/** The 6x7 grid of a month, weeks starting on Monday. */
export function monthGrid(anchor: Date): Date[] {
  const first = startOfWeek(startOfMonth(anchor))
  return Array.from({ length: 42 }, (_, i) => addDays(first, i))
}

export function weekDays(anchor: Date): Date[] {
  const first = startOfWeek(anchor)
  return Array.from({ length: 7 }, (_, i) => addDays(first, i))
}

const AGENDA_DAYS = 30

/** The days a view shows, first inclusive. */
export function visibleDays(view: CalendarView, anchor: Date): Date[] {
  if (view === 'month') return monthGrid(anchor)
  if (view === 'week') return weekDays(anchor)
  if (view === 'day') return [anchor]
  return Array.from({ length: AGENDA_DAYS }, (_, i) => addDays(anchor, i))
}

/** The window to ask the API for: local midnight of the first day to local midnight after the last, as ISO instants. */
export function windowOf(view: CalendarView, anchor: Date): { from: string; to: string } {
  const days = visibleDays(view, anchor)
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

/** Local date and HH:mm (inputs) to an ISO instant. */
export function toInstant(date: string, time: string): string {
  const [h, m] = (time || '00:00').split(':').map(Number)
  const d = fromIso(date)
  d.setHours(h ?? 0, m ?? 0, 0, 0)
  return d.toISOString()
}

export function localTime(instant: string): string {
  const d = new Date(instant)
  return `${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}`
}

export function localDay(instant: string): string {
  return isoDate(new Date(instant))
}
