const dateFormat = new Intl.DateTimeFormat("tr-TR", { day: "numeric", month: "long", year: "numeric" });

export function formatDate(value: string) {
  return dateFormat.format(new Date(value));
}

export function timeAgo(value: string) {
  const diff = Date.now() - new Date(value).getTime();
  const day = 24 * 60 * 60 * 1000;
  if (diff < day) return "Bugün";
  const days = Math.floor(diff / day);
  if (days === 1) return "Dün";
  if (days < 30) return `${days} gün önce`;
  return formatDate(value);
}
