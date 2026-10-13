export interface ServiceResult<T> {
  data: T | null;
  errorMessage: string[] | null;
  message: string | null;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface TokenDto {
  accessToken: string;
  accessTokenExpiration: string;
  refreshToken: string;
  refreshTokenExpiration: string;
}

export interface CurrentUser {
  id: string;
  userName: string;
  email: string;
  roles: string[];
}

export interface JobPosting {
  id: string;
  title: string;
  description: string;
  location: string;
  isActive: boolean;
  salaryRange: string | null;
  companyId: number;
  companyName: string;
  companyLogoUrl: string | null;
  categoryId: number;
  categoryName: string;
  createdAt: string;
  updatedAt: string;
}

export interface Category {
  id: number;
  name: string;
}

export interface Company {
  id: number;
  name: string;
  about: string;
  industry: string;
  logoUrl: string | null;
  email: string | null;
  phoneNumber: string | null;
  websiteUrl: string | null;
  location: string | null;
}

export interface JobApplication {
  id: string;
  userId: string;
  userEmail: string;
  jobPostingId: string;
  jobPostingTitle: string;
  companyName: string;
  coverLetter: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface JobPostingQuery {
  search?: string;
  location?: string;
  categoryId?: number;
  companyId?: number;
  page?: number;
  pageSize?: number;
}
