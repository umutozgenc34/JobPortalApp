import { useCallback, useEffect, useState, type FormEvent } from "react";
import { Link } from "react-router-dom";
import { ApiError } from "../../api/client";
import { applicationsApi, categoriesApi, companiesApi, jobsApi } from "../../api/endpoints";
import type { Category, Company, JobApplication, JobPosting, JobPostingInput, PagedResult } from "../../api/types";
import { ConfirmButton } from "../../components/ConfirmButton";
import { Modal } from "../../components/Modal";
import { Pagination } from "../../components/Pagination";
import { formatDate } from "../../lib/format";

function errorText(e: unknown) {
  return e instanceof ApiError ? e.messages.join(" ") : "İşlem tamamlanamadı.";
}

function toInput(job: JobPosting): JobPostingInput {
  return {
    title: job.title,
    description: job.description,
    location: job.location,
    isActive: job.isActive,
    salaryRange: job.salaryRange,
    companyId: job.companyId,
    categoryId: job.categoryId
  };
}

function JobForm({ job, companies, categories, onSaved }: { job: JobPosting | null; companies: Company[]; categories: Category[]; onSaved: () => void }) {
  const [form, setForm] = useState<JobPostingInput>(
    job ? toInput(job) : { title: "", description: "", location: "", isActive: true, salaryRange: "", companyId: 0, categoryId: 0 }
  );
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  function set<K extends keyof JobPostingInput>(key: K, value: JobPostingInput[K]) {
    setForm((current) => ({ ...current, [key]: value }));
  }

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    if (!form.companyId || !form.categoryId) {
      setError("Şirket ve alan seç.");
      return;
    }
    setSaving(true);
    setError(null);
    const payload = { ...form, title: form.title.trim(), location: form.location.trim(), salaryRange: form.salaryRange?.trim() || null };
    try {
      if (job) await jobsApi.update(job.id, payload);
      else await jobsApi.create(payload);
      onSaved();
    } catch (e) {
      setError(errorText(e));
    } finally {
      setSaving(false);
    }
  }

  return (
    <form className="modal-form" onSubmit={handleSubmit}>
      <div className="form-grid">
        <label className="field form-grid__wide">
          <span className="field__label">Pozisyon adı</span>
          <input className="input" value={form.title} maxLength={255} onChange={(e) => set("title", e.target.value)} />
        </label>
        <label className="field">
          <span className="field__label">Şirket</span>
          <select className="input" value={form.companyId || ""} onChange={(e) => set("companyId", Number(e.target.value))}>
            <option value="">Seç</option>
            {companies.map((c) => (
              <option key={c.id} value={c.id}>{c.name}</option>
            ))}
          </select>
        </label>
        <label className="field">
          <span className="field__label">Alan</span>
          <select className="input" value={form.categoryId || ""} onChange={(e) => set("categoryId", Number(e.target.value))}>
            <option value="">Seç</option>
            {categories.map((c) => (
              <option key={c.id} value={c.id}>{c.name}</option>
            ))}
          </select>
        </label>
        <label className="field">
          <span className="field__label">Konum</span>
          <input className="input" value={form.location} maxLength={150} onChange={(e) => set("location", e.target.value)} placeholder="Örn. İstanbul ya da Uzaktan" />
        </label>
        <label className="field">
          <span className="field__label">Maaş <span className="muted">(isteğe bağlı)</span></span>
          <input className="input" value={form.salaryRange ?? ""} maxLength={50} onChange={(e) => set("salaryRange", e.target.value)} placeholder="Örn. 60.000 - 80.000 ₺" />
        </label>
        <label className="field form-grid__wide">
          <span className="field__label">İş tanımı</span>
          <textarea className="input input--tall" value={form.description} onChange={(e) => set("description", e.target.value)} />
        </label>
        <label className="checkbox form-grid__wide">
          <input type="checkbox" checked={form.isActive} onChange={(e) => set("isActive", e.target.checked)} />
          <span>Yayında (başvuru kabul ediyor)</span>
        </label>
      </div>
      {error && <p className="alert alert--error">{error}</p>}
      <div className="form-card__actions">
        <button type="submit" className="button button--primary" disabled={saving}>{saving ? "Kaydediliyor…" : job ? "Değişiklikleri kaydet" : "İlanı yayınla"}</button>
      </div>
    </form>
  );
}

function Applicants({ job }: { job: JobPosting }) {
  const [items, setItems] = useState<JobApplication[] | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    applicationsApi.byJob(job.id).then(setItems).catch((e) => setError(errorText(e)));
  }, [job.id]);

  if (error) return <p className="alert alert--error">{error}</p>;
  if (!items) return <div className="skeleton" style={{ height: 120 }} />;
  if (items.length === 0) return <p className="muted">Bu ilana henüz başvuru yapılmadı.</p>;

  return (
    <ul className="applicants">
      {items.map((a) => (
        <li key={a.id} className="applicants__item">
          <div className="applicants__head">
            <a href={`mailto:${a.userEmail}`} className="text-link">{a.userEmail}</a>
            <span className="muted">{formatDate(a.createdAt)}</span>
          </div>
          <p className={a.coverLetter ? "applicants__letter" : "muted"}>{a.coverLetter || "Ön yazı eklenmemiş."}</p>
        </li>
      ))}
    </ul>
  );
}

export function AdminJobs() {
  const [page, setPage] = useState(1);
  const [result, setResult] = useState<PagedResult<JobPosting> | null>(null);
  const [companies, setCompanies] = useState<Company[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [editing, setEditing] = useState<JobPosting | null | "new">(null);
  const [viewing, setViewing] = useState<JobPosting | null>(null);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(() => {
    jobsApi
      .list({ page, pageSize: 15, includeInactive: true })
      .then(setResult)
      .catch((e) => setError(errorText(e)));
  }, [page]);

  useEffect(load, [load]);

  useEffect(() => {
    companiesApi.list().then(setCompanies).catch(() => setCompanies([]));
    categoriesApi.list().then(setCategories).catch(() => setCategories([]));
  }, []);

  async function toggleActive(job: JobPosting) {
    setError(null);
    try {
      await jobsApi.update(job.id, { ...toInput(job), isActive: !job.isActive });
      load();
    } catch (e) {
      setError(errorText(e));
    }
  }

  return (
    <section>
      <div className="admin-toolbar">
        <p className="muted">{result ? `${result.totalCount} ilan` : "Yükleniyor"}</p>
        <button
          type="button"
          className="button button--primary"
          onClick={() => setEditing("new")}
          disabled={!companies.length || !categories.length}
          title={!companies.length || !categories.length ? "Önce en az bir şirket ve alan ekle" : undefined}
        >
          Yeni ilan
        </button>
      </div>

      {error && <p className="alert alert--error">{error}</p>}

      <div className="panel table-wrap">
        <table className="table">
          <thead>
            <tr>
              <th>Pozisyon</th>
              <th>Şirket</th>
              <th>Alan</th>
              <th>Durum</th>
              <th>Yayın</th>
              <th><span className="visually-hidden">İşlemler</span></th>
            </tr>
          </thead>
          <tbody>
            {result?.items.map((job) => (
              <tr key={job.id}>
                <td>
                  <Link to={`/ilanlar/${job.id}`} className="table__primary">{job.title}</Link>
                  <span className="table__sub muted">{job.location}</span>
                </td>
                <td>{job.companyName}</td>
                <td>{job.categoryName}</td>
                <td>
                  <span className={job.isActive ? "badge badge--success" : "badge"}>{job.isActive ? "Yayında" : "Kapalı"}</span>
                </td>
                <td className="muted">{formatDate(job.createdAt)}</td>
                <td>
                  <div className="table__actions">
                    <button type="button" className="button button--small" onClick={() => setViewing(job)}>Başvuranlar</button>
                    <button type="button" className="button button--small" onClick={() => setEditing(job)}>Düzenle</button>
                    <button type="button" className="button button--small" onClick={() => toggleActive(job)}>{job.isActive ? "Yayından kaldır" : "Yayınla"}</button>
                    <ConfirmButton
                      className="button button--small button--danger-text"
                      label="Sil"
                      confirmLabel="Emin misin?"
                      onConfirm={async () => {
                        try {
                          await jobsApi.remove(job.id);
                          load();
                        } catch (e) {
                          setError(errorText(e));
                        }
                      }}
                    />
                  </div>
                </td>
              </tr>
            ))}
            {result?.items.length === 0 && (
              <tr>
                <td colSpan={6} className="table__empty muted">Henüz ilan yok. "Yeni ilan" ile ilk ilanı ekle.</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
      {result && <Pagination page={result.page} totalPages={result.totalPages} onChange={setPage} />}

      <Modal title={editing === "new" ? "Yeni ilan" : "İlanı düzenle"} open={editing !== null} onClose={() => setEditing(null)}>
        <JobForm
          job={editing === "new" ? null : editing}
          companies={companies}
          categories={categories}
          onSaved={() => {
            setEditing(null);
            load();
          }}
        />
      </Modal>

      <Modal title={viewing ? `Başvuranlar: ${viewing.title}` : "Başvuranlar"} open={viewing !== null} onClose={() => setViewing(null)}>
        {viewing && <Applicants job={viewing} />}
      </Modal>
    </section>
  );
}
