import { request, toQuery } from "./client";
import type {
  AppNotification,
  Category,
  Company,
  CompanyReview,
  CompanyWithJobPostings,
  CurrentUser,
  JobApplication,
  JobPosting,
  JobPostingInput,
  JobPostingQuery,
  PagedResult,
  TokenDto,
  UserProfile
} from "./types";

export const authApi = {
  login: (email: string, password: string) =>
    request<TokenDto>("auth/login", { method: "POST", body: { email, password } }),
  register: (userName: string, email: string, password: string) =>
    request<unknown>("auth/register", { method: "POST", body: { userName, email, password } }),
  revoke: (token: string) => request<void>("auth/revoke-refresh-token", { method: "POST", body: { token } }),
  me: () => request<CurrentUser>("users/me"),
  updateMe: (userName: string, email: string) => request<void>("users/me", { method: "PUT", body: { userName, email } })
};

export const jobsApi = {
  list: (query: JobPostingQuery & { includeInactive?: boolean }) => request<PagedResult<JobPosting>>(`jobpostings${toQuery({ ...query })}`),
  get: (id: string) => request<JobPosting>(`jobpostings/${id}`),
  create: (input: JobPostingInput) => request<JobPosting>("jobpostings", { method: "POST", body: input }),
  update: (id: string, input: JobPostingInput) => request<void>(`jobpostings/${id}`, { method: "PUT", body: input }),
  remove: (id: string) => request<void>(`jobpostings/${id}`, { method: "DELETE" })
};

export const categoriesApi = {
  list: () => request<Category[]>("categories"),
  create: (name: string) => request<Category>("categories", { method: "POST", body: { name } }),
  update: (id: number, name: string) => request<void>(`categories/${id}`, { method: "PUT", body: { name } }),
  remove: (id: number) => request<void>(`categories/${id}`, { method: "DELETE" })
};

export const companiesApi = {
  list: () => request<Company[]>("companies"),
  get: (id: number) => request<Company>(`companies/${id}`),
  withJobs: (id: number) => request<CompanyWithJobPostings>(`companies/${id}/jobpostings`),
  create: (form: FormData) => request<Company>("companies", { method: "POST", form }),
  update: (id: number, form: FormData) => request<void>(`companies/${id}`, { method: "PUT", form }),
  remove: (id: number) => request<void>(`companies/${id}`, { method: "DELETE" })
};

export const reviewsApi = {
  byCompany: (companyId: number) => request<CompanyReview[]>(`companyreviews/by-company/${companyId}`),
  create: (companyId: number, rating: number, comment: string) =>
    request<CompanyReview>("companyreviews", { method: "POST", body: { companyId, rating, comment } }),
  update: (id: number, rating: number, comment: string) => request<void>(`companyreviews/${id}`, { method: "PUT", body: { rating, comment } }),
  remove: (id: number) => request<void>(`companyreviews/${id}`, { method: "DELETE" })
};

export const applicationsApi = {
  apply: (jobPostingId: string, coverLetter: string) =>
    request<JobApplication>("jobapplications", { method: "POST", body: { jobPostingId, coverLetter: coverLetter || null } }),
  mine: () => request<JobApplication[]>("jobapplications/by-user"),
  byJob: (jobPostingId: string) => request<JobApplication[]>(`jobapplications/by-jobposting/${jobPostingId}`)
};

export const profileApi = {
  mine: () => request<UserProfile>("userprofiles/me"),
  create: (form: FormData) => request<UserProfile>("userprofiles", { method: "POST", form }),
  update: (form: FormData) => request<void>("userprofiles/me", { method: "PUT", form }),
  remove: () => request<void>("userprofiles/me", { method: "DELETE" })
};

export const notificationsApi = {
  mine: () => request<AppNotification[]>("notifications"),
  markRead: (id: string) => request<void>(`notifications/${id}/read`, { method: "PUT" }),
  markAllRead: () => request<void>("notifications/read-all", { method: "PUT" })
};
