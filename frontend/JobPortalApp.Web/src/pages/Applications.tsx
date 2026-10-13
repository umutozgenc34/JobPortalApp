import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { applicationsApi } from "../api/endpoints";
import type { JobApplication } from "../api/types";
import { formatDate } from "../lib/format";

export function Applications() {
  const [items, setItems] = useState<JobApplication[] | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    applicationsApi.mine().then(setItems).catch((e: Error) => setError(e.message));
  }, []);

  return (
    <div className="container page">
      <header className="page-header">
        <h1>Başvurularım</h1>
        <p className="muted">Gönderdiğin başvurular, en yeniden eskiye.</p>
      </header>

      {error && <p className="alert alert--error">{error}</p>}

      {items === null && !error && <div className="skeleton" style={{ height: 200 }} />}

      {items?.length === 0 && (
        <div className="panel empty-card">
          <h2>Henüz başvuru yapmadın</h2>
          <p className="muted">İlgini çeken bir ilan bulduğunda başvurun burada listelenir.</p>
          <Link to="/ilanlar" className="button button--primary">İlanlara göz at</Link>
        </div>
      )}

      {items && items.length > 0 && (
        <ul className="panel list">
          {items.map((application) => (
            <li key={application.id} className="list__item application">
              <div className="application__main">
                <Link to={`/ilanlar/${application.jobPostingId}`} className="application__title">{application.jobPostingTitle}</Link>
                <p className="muted">{application.companyName}</p>
                {application.coverLetter && <p className="application__letter">{application.coverLetter}</p>}
              </div>
              <div className="application__side">
                <span className="badge">Gönderildi</span>
                <span className="muted application__date">{formatDate(application.createdAt)}</span>
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
