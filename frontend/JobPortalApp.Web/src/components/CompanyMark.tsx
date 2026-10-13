export function initials(name: string) {
  return name
    .split(/\s+/)
    .filter(Boolean)
    .slice(0, 2)
    .map((word) => word[0])
    .join("")
    .toLocaleUpperCase("tr-TR");
}

export function CompanyMark({ name, logoUrl, size = 48 }: { name: string; logoUrl?: string | null; size?: number }) {
  return (
    <span className="company-mark" style={{ width: size, height: size, fontSize: size * 0.36 }} aria-hidden="true">
      {logoUrl ? <img src={logoUrl} alt="" loading="lazy" /> : initials(name)}
    </span>
  );
}
