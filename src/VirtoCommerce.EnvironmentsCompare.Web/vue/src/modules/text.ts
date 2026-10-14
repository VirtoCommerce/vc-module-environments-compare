// value / description text of a setting (legacy: {{ value }} and the "settings.<name>.description" translation)
export const valueText = (v: unknown): string =>
  v === null || v === undefined ? "" : typeof v === "object" ? JSON.stringify(v) : String(v);

export const describe = (
  name: string | undefined,
  description: string | undefined,
  te: (key: string) => boolean,
  t: (key: string) => string,
): string | undefined => {
  const key = `settings.${name}.description`;
  return description || (name && te(key) ? t(key) : undefined);
};
