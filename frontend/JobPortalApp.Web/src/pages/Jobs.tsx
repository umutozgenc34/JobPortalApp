import { useEffect, useMemo, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { categoriesApi, companiesApi, jobsApi } from "../api/endpoints";
import type { Category, Company, JobPosting, PagedResult } from "../api/types";
import { JobRow, JobRowSkeleton } from "../components/JobRow";
import { Pagination } from "../components/Pagination";
import { SearchBar } from "../components/SearchBar";

const PAGE_SIZE = 10;

export function Jobs() {
  const [params, setParams] = useSearchParams();
  const [categories, setCategories] = useState<Category[]>([]);
  const [companies, setCompanies] = useState<Company[]>([]);
  const [result, setResult] = useState<PagedResult<JobPosting> | null>(null);
  const [error, setError] = useState<string | null>(null);

  const query = useMemo(
    () => ({
      search: params.get("ara") ?? "",
      location: params.get("konum") ?? "",
      categoryId: params.get("kategori") ? Number(params.get("kategori")) : undefined,
      companyId: params.get("sirket") ? Number(params.get("sirket")) : undefined,
      page: Number(params.get("sayfa") ?? 1) || 1
    }),
    [params]
  );

  useEffect(() => {
    categoriesApi.list().then(setCategories).catch(() => setCategories([]));
    companiesApi.list().then(setCompanies).catch(() => setCompanies([]));
  }, []);

  useEffect(() => {
    setError(null);
    setResult(null);
    jobsApi
      .list({ ...query, pageSize: PAGE_SIZE })
      .then(setResult)
      .catch((e: Error) => setError(e.message));
  }, [query]);

  function update(next: Record<string, string | number | undefined>) {
    const merged = new URLSearchParams(params);
    Object.entries(next).forEach(([key, value]) => {
      if (value === undefined || value === "") merged.delete(key);
      else merged.set(key, String(value));
    });
    if (!("sayfa" in next)) merged.delete("sayfa");
    setParams(merged);
  }

  const activeCategory = categories.find((c) => c.id === query.categoryId);
  const activeCompany = companies.find((c) => c.id === query.companyId);
  const hasFilters = Boolean(query.search || query.location || query.categoryId || query.companyId);

  return (
    <div className="container page">
      <header className="page-header">
        <h1>İş ilanları</h1>
        <p className="muted">Pozisyona, konuma, alana ya da şirkete göre ara.</p>
      </header>

      <SearchBar
        key={`${query.search}|${query.location}`}
        initialSearch={query.search}
        initialLocation={query.location}
        onSearch={(search, location) => update({ ara: search, konum: location })}
      />

      <div className="jobs-layout">
        <aside className="filters panel" aria-label="Filtreler">
          <h2 className="filters__title">Filtreler</h2>
          <label className="field">
            <span className="field__label">Çalışma alanı</span>
            <select className="input" value={query.categoryId ?? ""} onChange={(e) => update({ kategori: e.target.value })}>
              <option value="">Tüm alanlar</option>
              {categories.map((c) => (
                <option key={c.id} value={c.id}>{c.name}</option>
              ))}
            </select>
          </label>
          <label className="field">
            <span className="field__label">Şirket</span>
            <select className="input" value={query.companyId ?? ""} onChange={(e) => update({ sirket: e.target.value })}>
              <option value="">Tüm şirketler</option>
              {companies.map((c) => (
                <option key={c.id} value={c.id}>{c.name}</option>
              ))}
            </select>
          </label>
          {hasFilters && (
            <button type="button" className="button button--block" onClick={() => setParams(new URLSearchParams())}>
              Filtreleri temizle
            </button>
          )}
        </aside>

        <section className="jobs-results" aria-live="polite">
          <div className="jobs-results__head">
            <p>
              {result ? (
                <>
                  <strong>{result.totalCount}</strong> ilan bulundu
                </>
              ) : (
                "İlanlar yükleniyor"
              )}
            </p>
            <div className="jobs-results__chips">
              {query.search && <span className="badge">“{query.search}”</span>}
              {query.location && <span className="badge">{query.location}</span>}
              {activeCategory && <span className="badge">{activeCategory.name}</span>}
              {activeCompany && <span className="badge">{activeCompany.name}</span>}
            </div>
          </div>

          {error && <p className="alert alert--error">{error}</p>}

          <ul className="job-list panel">
            {!result && !error && Array.from({ length: 5 }, (_, i) => <JobRowSkeleton key={i} />)}
            {result?.items.map((job) => <JobRow key={job.id} job={job} />)}
            {result && result.items.length === 0 && (
              <li className="job-list__empty">
                <p>{hasFilters ? "Bu kriterlere uyan ilan bulunamadı." : "Şu an yayında ilan yok."}</p>
                {hasFilters && <p className="muted">Daha az filtreyle ya da farklı bir anahtar kelimeyle tekrar dene.</p>}
              </li>
            )}
          </ul>

          {result && (
            <Pagination
              page={result.page}
              totalPages={result.totalPages}
              onChange={(page) => {
                update({ sayfa: page });
                window.scrollTo({ top: 0, behavior: "smooth" });
              }}
            />
          )}
        </section>
      </div>
    </div>
  );
}
