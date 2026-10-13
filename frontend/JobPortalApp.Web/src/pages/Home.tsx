import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { categoriesApi, companiesApi, jobsApi } from "../api/endpoints";
import type { Category, Company, JobPosting } from "../api/types";
import { JobRow, JobRowSkeleton } from "../components/JobRow";
import { SearchBar } from "../components/SearchBar";
import { CompanyMark } from "../components/CompanyMark";
import { toQuery } from "../api/client";

export function Home() {
  const navigate = useNavigate();
  const [jobs, setJobs] = useState<JobPosting[] | null>(null);
  const [total, setTotal] = useState<number | null>(null);
  const [categories, setCategories] = useState<Category[]>([]);
  const [companies, setCompanies] = useState<Company[]>([]);

  useEffect(() => {
    jobsApi
      .list({ page: 1, pageSize: 6 })
      .then((result) => {
        setJobs(result.items);
        setTotal(result.totalCount);
      })
      .catch(() => {
        setJobs([]);
        setTotal(0);
      });
    categoriesApi.list().then(setCategories).catch(() => setCategories([]));
    companiesApi.list().then(setCompanies).catch(() => setCompanies([]));
  }, []);

  function handleSearch(search: string, location: string) {
    navigate(`/ilanlar${toQuery({ ara: search, konum: location })}`);
  }

  return (
    <>
      <section className="hero">
        <div className="container hero__inner">
          <div className="hero__copy">
            <h1 className="hero__title">Kariyerindeki bir sonraki adımı bul.</h1>
            <p className="hero__lead muted">
              Şirketlerin açık pozisyonlarını incele, ihtiyacına göre filtrele ve tek adımda başvur.
            </p>
          </div>
          <SearchBar size="large" onSearch={handleSearch} />
          <dl className="hero__stats">
            <div>
              <dt>Açık ilan</dt>
              <dd>{total ?? "–"}</dd>
            </div>
            <div>
              <dt>Şirket</dt>
              <dd>{companies.length || "–"}</dd>
            </div>
            <div>
              <dt>Çalışma alanı</dt>
              <dd>{categories.length || "–"}</dd>
            </div>
          </dl>
        </div>
      </section>

      <div className="container page home">
        <section className="home__section">
          <div className="section-head">
            <h2>Son eklenen ilanlar</h2>
            <Link to="/ilanlar" className="text-link">Tüm ilanları gör</Link>
          </div>
          <ul className="job-list panel">
            {jobs === null && Array.from({ length: 4 }, (_, i) => <JobRowSkeleton key={i} />)}
            {jobs?.map((job) => <JobRow key={job.id} job={job} />)}
            {jobs?.length === 0 && <li className="job-list__empty muted">Şu an yayında ilan yok. Yeni ilanlar eklendiğinde burada görünecek.</li>}
          </ul>
        </section>

        {categories.length > 0 && (
          <section className="home__section">
            <div className="section-head">
              <h2>Alanlara göre ilanlar</h2>
            </div>
            <ul className="category-grid">
              {categories.map((category) => (
                <li key={category.id}>
                  <Link to={`/ilanlar?kategori=${category.id}`} className="category-card panel">
                    <span className="category-card__name">{category.name}</span>
                    <span className="category-card__action muted">İlanları gör</span>
                  </Link>
                </li>
              ))}
            </ul>
          </section>
        )}

        {companies.length > 0 && (
          <section className="home__section">
            <div className="section-head">
              <h2>İlan veren şirketler</h2>
            </div>
            <ul className="company-strip">
              {companies.slice(0, 8).map((company) => (
                <li key={company.id}>
                  <Link to={`/ilanlar?sirket=${company.id}`} className="company-chip panel">
                    <CompanyMark name={company.name} logoUrl={company.logoUrl} size={40} />
                    <span>
                      <span className="company-chip__name">{company.name}</span>
                      <span className="company-chip__industry muted">{company.industry}</span>
                    </span>
                  </Link>
                </li>
              ))}
            </ul>
          </section>
        )}
      </div>
    </>
  );
}
