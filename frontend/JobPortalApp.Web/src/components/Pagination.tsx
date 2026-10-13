export function Pagination({ page, totalPages, onChange }: { page: number; totalPages: number; onChange: (page: number) => void }) {
  if (totalPages <= 1) return null;

  return (
    <nav className="pagination" aria-label="Sayfalar">
      <button type="button" className="button" disabled={page <= 1} onClick={() => onChange(page - 1)}>
        Önceki
      </button>
      <span className="pagination__status muted">
        Sayfa {page} / {totalPages}
      </span>
      <button type="button" className="button" disabled={page >= totalPages} onClick={() => onChange(page + 1)}>
        Sonraki
      </button>
    </nav>
  );
}
