export function parseUtc(value: string): Date {
  const hasZone = /([zZ]|[+-]\d{2}:\d{2})$/.test(value);
  return new Date(hasZone ? value : `${value}Z`);
}
