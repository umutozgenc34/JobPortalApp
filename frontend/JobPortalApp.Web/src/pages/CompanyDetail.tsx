import { useCallback, useEffect, useState, type FormEvent } from "react";
import { Link, useLocation, useParams } from "react-router-dom";
import { ApiError } from "../api/client";
import { companiesApi, reviewsApi } from "../api/endpoints";
import type { CompanyReview, CompanyWithJobPostings } from "../api/types";
import { useAuth } from "../auth/AuthContext";
import { CompanyMark } from "../components/CompanyMark";
import { ConfirmButton } from "../components/ConfirmButton";
import { JobRow } from "../components/JobRow";
import { RatingInput, Stars } from "../components/Rating";
import { formatDate } from "../lib/format";

function ReviewForm({ companyId, review, onDone, onCancel }: { companyId: number; review?: CompanyReview; onDone: () => void; onCancel?: () => void }) {
  const [rating, setRating] = useState(review?.rating ?? 0);
  const [comment, setComment] = useState(review?.comment ?? "");
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    if (!rating) {
      setError("Bir puan seç.");
      return;
    }
    setSaving(true);
    setError(null);
    try {
      if (review) await reviewsApi.update(review.id, rating, comment.trim());
      else await reviewsApi.create(companyId, rating, comment.trim());
      onDone();
    } catch (e) {
      setError(e instanceof ApiError ? e.messages.join(" ") : "Değerlendirme kaydedilemedi.");
    } finally {
      setSaving(false);
    }
  }

  return (
    <form className="review-form" onSubmit={handleSubmit}>
      <div className="field">
        <span className="field__label">Puanın</span>
        <RatingInput value={rating} onChange={setRating} />
      </div>
      <label className="field">
        <span className="field__label">Yorumun</span>
        <textarea className="input" value={comment} maxLength={500} onChange={(e) => setComment(e.target.value)} placeholder="Çalışma ortamı, mülakat süreci ya da şirket kültürü hakkındaki deneyimini paylaş." />
        <span className="field__hint">{comment.length} / 500</span>
      </label>
      {error && <p className="alert alert--error">{error}</p>}
      <div className="form-card__actions">
        <button type="submit" className="button button--primary" disabled={saving}>{saving ? "Kaydediliyor…" : review ? "Değerlendirmeyi güncelle" : "Değerlendirmeyi gönder"}</button>
        {onCancel && <button type="button" className="button button--ghost" onClick={onCancel}>Vazgeç</button>}
      </div>
    </form>
  );
}

export function CompanyDetail() {
  const { id } = useParams();
  const companyId = Number(id);
  const { user, isAdmin } = useAuth();
  const location = useLocation();
  const [company, setCompany] = useState<CompanyWithJobPostings | null>(null);
  const [reviews, setReviews] = useState<CompanyReview[] | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [editing, setEditing] = useState(false);

  const loadReviews = useCallback(() => {
    reviewsApi.byCompany(companyId).then(setReviews).catch(() => setReviews([]));
  }, [companyId]);

  useEffect(() => {
    setCompany(null);
    setError(null);
    companiesApi
      .withJobs(companyId)
      .then(setCompany)
      .catch((e: ApiError) => setError(e.status === 404 ? "Bu şirket bulunamadı." : e.message));
    loadReviews();
  }, [companyId, loadReviews]);

  if (error) {
    return (
      <div className="container page state">
        <h1>Şirket bulunamadı</h1>
        <p className="muted">{error}</p>
        <Link to="/sirketler" className="button">Tüm şirketlere dön</Link>
      </div>
    );
  }

  if (!company) {
    return (
      <div className="container page">
        <div className="skeleton" style={{ height: 160 }} />
      </div>
    );
  }

  const openJobs = company.jobPostings.filter((job) => job.isActive);
  const average = reviews?.length ? reviews.reduce((sum, r) => sum + r.rating, 0) / reviews.length : 0;
  const myReview = reviews?.find((r) => r.userId === user?.id);

  return (
    <div className="container page">
      <nav className="breadcrumb" aria-label="Konum">
        <Link to="/sirketler" className="text-link">Şirketler</Link>
        <span aria-hidden="true">/</span>
        <span className="muted">{company.name}</span>
      </nav>

      <header className="detail-header panel">
        <CompanyMark name={company.name} logoUrl={company.logoUrl} size={72} />
        <div className="detail-header__main">
          <h1 className="detail-header__title">{company.name}</h1>
          <p className="detail-header__company">{company.industry}</p>
          <div className="detail-header__badges">
            {company.location && <span className="badge">{company.location}</span>}
            <span className="badge">{openJobs.length} açık pozisyon</span>
            {reviews && reviews.length > 0 && (
              <span className="badge">
                <Stars value={average} size={13} /> {average.toFixed(1)} ({reviews.length})
              </span>
            )}
          </div>
        </div>
      </header>

      <div className="detail-layout">
        <div className="detail-stack">
          <section className="detail-body panel">
            <h2>Hakkında</h2>
            <p className="detail-body__text">{company.about}</p>
          </section>

          <section>
            <div className="section-head">
              <h2>Açık pozisyonlar</h2>
            </div>
            <ul className="job-list panel">
              {openJobs.map((job) => (
                <JobRow key={job.id} job={job} />
              ))}
              {openJobs.length === 0 && <li className="job-list__empty muted">Bu şirketin şu an açık ilanı yok.</li>}
            </ul>
          </section>

          <section>
            <div className="section-head">
              <h2>Değerlendirmeler</h2>
            </div>
            <div className="panel reviews">
              {user && !myReview && reviews !== null && (
                <div className="reviews__compose">
                  <h3>Bu şirketi değerlendir</h3>
                  <ReviewForm companyId={company.id} onDone={loadReviews} />
                </div>
              )}
              {!user && (
                <p className="reviews__login muted">
                  Değerlendirme yazmak için <Link to="/giris" state={{ from: location.pathname }} className="text-link">giriş yap</Link>.
                </p>
              )}

              {reviews === null && <div className="skeleton" style={{ height: 120 }} />}
              {reviews?.length === 0 && <p className="reviews__empty muted">Henüz değerlendirme yok.</p>}

              <ul className="reviews__list">
                {reviews?.map((review) => (
                  <li key={review.id} className="review">
                    {editing && review.id === myReview?.id ? (
                      <ReviewForm
                        companyId={company.id}
                        review={review}
                        onDone={() => {
                          setEditing(false);
                          loadReviews();
                        }}
                        onCancel={() => setEditing(false)}
                      />
                    ) : (
                      <>
                        <div className="review__head">
                          <span className="avatar" aria-hidden="true">{review.userName.slice(0, 1).toLocaleUpperCase("tr-TR")}</span>
                          <div>
                            <strong>{review.userName}</strong>
                            {review.userId === user?.id && <span className="badge review__mine">Senin değerlendirmen</span>}
                            <div className="review__meta">
                              <Stars value={review.rating} size={14} />
                              <span className="muted">{formatDate(review.createdAt)}</span>
                            </div>
                          </div>
                        </div>
                        <p className="review__text">{review.comment}</p>
                        {(review.userId === user?.id || isAdmin) && (
                          <div className="review__actions">
                            {review.userId === user?.id && (
                              <button type="button" className="button button--ghost button--small" onClick={() => setEditing(true)}>Düzenle</button>
                            )}
                            <ConfirmButton
                              className="button button--ghost button--small button--danger-text"
                              label="Sil"
                              confirmLabel="Silmek için tekrar tıkla"
                              onConfirm={async () => {
                                await reviewsApi.remove(review.id);
                                loadReviews();
                              }}
                            />
                          </div>
                        )}
                      </>
                    )}
                  </li>
                ))}
              </ul>
            </div>
          </section>
        </div>

        <aside className="detail-aside">
          <dl className="facts panel">
            <div>
              <dt>Sektör</dt>
              <dd>{company.industry}</dd>
            </div>
            {company.location && (
              <div>
                <dt>Konum</dt>
                <dd>{company.location}</dd>
              </div>
            )}
            {company.websiteUrl && (
              <div>
                <dt>Web sitesi</dt>
                <dd>
                  <a href={company.websiteUrl.startsWith("http") ? company.websiteUrl : `https://${company.websiteUrl}`} target="_blank" rel="noreferrer" className="text-link">
                    {company.websiteUrl.replace(/^https?:\/\//, "")}
                  </a>
                </dd>
              </div>
            )}
            {company.email && (
              <div>
                <dt>E-posta</dt>
                <dd>
                  <a href={`mailto:${company.email}`} className="text-link">{company.email}</a>
                </dd>
              </div>
            )}
            {company.phoneNumber && (
              <div>
                <dt>Telefon</dt>
                <dd>{company.phoneNumber}</dd>
              </div>
            )}
          </dl>
        </aside>
      </div>
    </div>
  );
}
