import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { companiesApi } from "../api/endpoints";
import type { Company } from "../api/types";
import { CompanyMark } from "../components/CompanyMark";

export function Companies() {
  const [companies, setCompanies] = useState<Company[] | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [filter, setFilter] = useState("");

  useEffect(() => {
    companiesApi.list().then(setCompanies).catch((e: Error) => setError(e.message));
  }, []);

  const visible = useMemo(() => {
    const term = filter.trim().toLocaleLowerCase("tr-TR");
    if (!companies || !term) return companies;
    return companies.filter((c) =>
      [c.name, c.industry, c.location ?? ""].some((value) => value.toLocaleLowerCase("tr-TR").includes(term))
    );
  }, [companies, filter]);

  return (
    <div className="container page">
      <header className="page-header page-header--row">
        <div>
          <h1>Şirketler</h1>
          <p className="muted">Platformda ilan veren şirketleri incele, değerlendirmeleri oku.</p>
        </div>
        <label className="field page-header__search">
          <span className="visually-hidden">Şirket ara</span>
          <input className="input" value={filter} onChange={(e) => setFilter(e.target.value)} placeholder="Şirket, sektör ya da şehir ara" />
        </label>
      </header>

      {error && <p className="alert alert--error">{error}</p>}
      {companies === null && !error && <div className="skeleton" style={{ height: 240 }} />}

      {visible && visible.length === 0 && (
        <div className="panel empty-card">
          <h2>{companies?.length ? "Eşleşen şirket yok" : "Henüz şirket eklenmedi"}</h2>
          {companies?.length ? <p className="muted">Farklı bir arama terimi dene.</p> : null}
        </div>
      )}

      {visible && visible.length > 0 && (
        <ul className="company-grid">
          {visible.map((company) => (
            <li key={company.id}>
              <Link to={`/sirketler/${company.id}`} className="company-card panel">
                <div className="company-card__head">
                  <CompanyMark name={company.name} logoUrl={company.logoUrl} size={56} />
                  <div>
                    <h2 className="company-card__name">{company.name}</h2>
                    <p className="muted">{[company.industry, company.location].filter(Boolean).join(", ")}</p>
                  </div>
                </div>
                <p className="company-card__about">{company.about}</p>
              </Link>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
