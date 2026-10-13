import { request, toQuery } from "./client";
import type {
  Category,
  Company,
  CurrentUser,
  JobApplication,
  JobPosting,
  JobPostingQuery,
  PagedResult,
  TokenDto
} from "./types";

export const authApi = {
  login: (email: string, password: string) =>
    request<TokenDto>("auth/login", { method: "POST", body: { email, password } }),
  register: (userName: string, email: string, password: string) =>
    request<unknown>("auth/register", { method: "POST", body: { userName, email, password } }),
  revoke: (token: string) => request<void>("auth/revoke-refresh-token", { method: "POST", body: { token } }),
  me: () => request<CurrentUser>("users/me")
};

export const jobsApi = {
  list: (query: JobPostingQuery) => request<PagedResult<JobPosting>>(`jobpostings${toQuery({ ...query })}`),
  get: (id: string) => request<JobPosting>(`jobpostings/${id}`)
};

export const categoriesApi = {
  list: () => request<Category[]>("categories")
};

export const companiesApi = {
  list: () => request<Company[]>("companies")
};

export const applicationsApi = {
  apply: (jobPostingId: string, coverLetter: string) =>
    request<JobApplication>("jobapplications", { method: "POST", body: { jobPostingId, coverLetter: coverLetter || null } }),
  mine: () => request<JobApplication[]>("jobapplications/by-user")
};
