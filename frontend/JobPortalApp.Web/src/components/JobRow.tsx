import { Link } from "react-router-dom";
import type { JobPosting } from "../api/types";
import { CompanyMark } from "./CompanyMark";
import { timeAgo } from "../lib/format";

export function JobRow({ job }: { job: JobPosting }) {
  return (
    <li>
      <Link to={`/ilanlar/${job.id}`} className="job-row">
        <CompanyMark name={job.companyName} logoUrl={job.companyLogoUrl} />
        <div className="job-row__main">
          <h3 className="job-row__title">{job.title}</h3>
          <p className="job-row__company">{job.companyName}</p>
          <div className="job-row__meta">
            <span>{job.location}</span>
            <span className="badge">{job.categoryName}</span>
          </div>
        </div>
        <div className="job-row__side">
          {job.salaryRange ? <span className="job-row__salary">{job.salaryRange}</span> : <span className="muted">Maaş belirtilmemiş</span>}
          <span className="job-row__date">{timeAgo(job.createdAt)}</span>
        </div>
      </Link>
    </li>
  );
}

export function JobRowSkeleton() {
  return (
    <li className="job-row job-row--skeleton" aria-hidden="true">
      <span className="skeleton" style={{ width: 48, height: 48 }} />
      <div className="job-row__main">
        <span className="skeleton" style={{ width: "45%", height: 18 }} />
        <span className="skeleton" style={{ width: "25%", height: 14, marginTop: 8 }} />
      </div>
    </li>
  );
}
