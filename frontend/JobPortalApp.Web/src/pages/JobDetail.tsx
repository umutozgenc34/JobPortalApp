import { useEffect, useState, type FormEvent } from "react";
import { Link, useLocation, useParams } from "react-router-dom";
import { motion, useReducedMotion } from "motion/react";
import { applicationsApi, jobsApi } from "../api/endpoints";
import { ApiError } from "../api/client";
import type { JobPosting } from "../api/types";
import { useAuth } from "../auth/AuthContext";
import { CompanyMark } from "../components/CompanyMark";
import { formatDate } from "../lib/format";

function ApplyPanel({ job }: { job: JobPosting }) {
  const { user, loading } = useAuth();
  const location = useLocation();
  const reduceMotion = useReducedMotion();
  const [applied, setApplied] = useState<boolean | null>(null);
  const [coverLetter, setCoverLetter] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [justApplied, setJustApplied] = useState(false);

  useEffect(() => {
    if (!user) {
      setApplied(null);
      return;
    }
    applicationsApi
      .mine()
      .then((list) => setApplied(list.some((a) => a.jobPostingId === job.id)))
      .catch(() => setApplied(false));
  }, [user, job.id]);

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setSubmitting(true);
    setError(null);
    try {
      await applicationsApi.apply(job.id, coverLetter.trim());
      setApplied(true);
      setJustApplied(true);
    } catch (e) {
      setError(e instanceof ApiError ? e.messages.join(" ") : "Başvuru gönderilemedi.");
    } finally {
      setSubmitting(false);
    }
  }

  if (!job.isActive) {
    return (
      <div className="apply panel">
        <h2 className="apply__title">Başvurular kapandı</h2>
        <p className="muted">Bu ilan artık başvuru kabul etmiyor.</p>
      </div>
    );
  }

  if (loading) return <div className="apply panel skeleton" style={{ height: 180 }} />;

  if (!user) {
    return (
      <div className="apply panel">
        <h2 className="apply__title">Bu ilana başvur</h2>
        <p className="muted">Başvuru yapmak için hesabına giriş yapman gerekiyor.</p>
        <Link to="/giris" state={{ from: location.pathname }} className="button button--primary button--block">Giriş yap</Link>
        <p className="apply__alt muted">
          Hesabın yok mu?{" "}
          <Link to="/kayit" state={{ from: location.pathname }} className="text-link">Kayıt ol</Link>
        </p>
      </div>
    );
  }

  if (applied) {
    return (
      <motion.div
        className="apply panel apply--done"
        initial={justApplied && !reduceMotion ? { opacity: 0, y: 8 } : false}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.4, ease: [0.22, 1, 0.36, 1] }}
      >
        <span className="apply__check" aria-hidden="true">✓</span>
        <h2 className="apply__title">{justApplied ? "Başvurun gönderildi" : "Bu ilana başvurdun"}</h2>
        <p className="muted">Şirket başvurunu inceledikten sonra seninle iletişime geçecek.</p>
      </motion.div>
    );
  }

  return (
    <form className="apply panel" onSubmit={handleSubmit}>
      <h2 className="apply__title">Bu ilana başvur</h2>
      <label className="field">
        <span className="field__label">Ön yazı <span className="muted">(isteğe bağlı)</span></span>
        <textarea
          className="input"
          maxLength={1000}
          value={coverLetter}
          onChange={(e) => setCoverLetter(e.target.value)}
          placeholder="Kendinden ve bu pozisyonla neden ilgilendiğinden kısaca bahset."
        />
        <span className="field__hint apply__counter">{coverLetter.length} / 1000</span>
      </label>
      {error && <p className="alert alert--error">{error}</p>}
      <button type="submit" className="button button--primary button--block" disabled={submitting || applied === null}>
        {submitting ? "Gönderiliyor…" : "Başvuruyu gönder"}
      </button>
    </form>
  );
}

export function JobDetail() {
  const { id } = useParams();
  const [job, setJob] = useState<JobPosting | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) return;
    setJob(null);
    setError(null);
    jobsApi
      .get(id)
      .then(setJob)
      .catch((e: ApiError) => setError(e.status === 404 ? "Bu ilan bulunamadı. Yayından kaldırılmış olabilir." : e.message));
  }, [id]);

  if (error) {
    return (
      <div className="container page state">
        <h1>İlan bulunamadı</h1>
        <p className="muted">{error}</p>
        <Link to="/ilanlar" className="button">Tüm ilanlara dön</Link>
      </div>
    );
  }

  if (!job) {
    return (
      <div className="container page">
        <div className="skeleton" style={{ height: 160 }} />
      </div>
    );
  }

  return (
    <div className="container page">
      <nav className="breadcrumb" aria-label="Konum">
        <Link to="/ilanlar" className="text-link">İş ilanları</Link>
        <span aria-hidden="true">/</span>
        <span className="muted">{job.categoryName}</span>
      </nav>

      <header className="detail-header panel">
        <CompanyMark name={job.companyName} logoUrl={job.companyLogoUrl} size={72} />
        <div className="detail-header__main">
          <h1 className="detail-header__title">{job.title}</h1>
          <Link to={`/sirketler/${job.companyId}`} className="detail-header__company detail-header__company-link">{job.companyName}</Link>
          <div className="detail-header__badges">
            <span className="badge">{job.location}</span>
            <span className="badge">{job.categoryName}</span>
            {job.salaryRange && <span className="badge badge--accent">{job.salaryRange}</span>}
            {!job.isActive && <span className="badge">Başvurulara kapalı</span>}
          </div>
        </div>
      </header>

      <div className="detail-layout">
        <article className="detail-body panel">
          <h2>İş tanımı</h2>
          <p className="detail-body__text">{job.description}</p>
        </article>

        <aside className="detail-aside">
          <ApplyPanel job={job} />
          <dl className="facts panel">
            <div>
              <dt>Şirket</dt>
              <dd>
                <Link to={`/sirketler/${job.companyId}`} className="text-link">{job.companyName}</Link>
              </dd>
            </div>
            <div>
              <dt>Konum</dt>
              <dd>{job.location}</dd>
            </div>
            <div>
              <dt>Maaş</dt>
              <dd>{job.salaryRange ?? "Belirtilmemiş"}</dd>
            </div>
            <div>
              <dt>Yayın tarihi</dt>
              <dd>{formatDate(job.createdAt)}</dd>
            </div>
          </dl>
        </aside>
      </div>
    </div>
  );
}
