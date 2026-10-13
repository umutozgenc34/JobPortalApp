import type { ServiceResult, TokenDto } from "./types";
import { translateMessage } from "../lib/messages";

const BASE_URL = "/api/v1/";
const STORAGE_KEY = "jp.tokens";

export class ApiError extends Error {
  status: number;
  messages: string[];

  constructor(status: number, messages: string[]) {
    super(messages[0] ?? "Beklenmeyen bir hata oluştu.");
    this.status = status;
    this.messages = messages;
  }
}

type Listener = (tokens: TokenDto | null) => void;
const listeners = new Set<Listener>();

export function getTokens(): TokenDto | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    return raw ? (JSON.parse(raw) as TokenDto) : null;
  } catch {
    return null;
  }
}

export function setTokens(tokens: TokenDto | null) {
  try {
    if (tokens) localStorage.setItem(STORAGE_KEY, JSON.stringify(tokens));
    else localStorage.removeItem(STORAGE_KEY);
  } catch {
    return;
  } finally {
    listeners.forEach((listener) => listener(tokens));
  }
}

export function onTokensChange(listener: Listener) {
  listeners.add(listener);
  return () => {
    listeners.delete(listener);
  };
}

let refreshing: Promise<boolean> | null = null;

async function refreshTokens(): Promise<boolean> {
  const current = getTokens();
  if (!current) return false;

  refreshing ??= (async () => {
    try {
      const response = await fetch(`${BASE_URL}auth/create-token-by-refresh-token`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ token: current.refreshToken })
      });
      if (!response.ok) {
        setTokens(null);
        return false;
      }
      const result = (await response.json()) as ServiceResult<TokenDto>;
      setTokens(result.data);
      return Boolean(result.data);
    } catch {
      return false;
    } finally {
      refreshing = null;
    }
  })();

  return refreshing;
}

interface RequestOptions {
  method?: string;
  body?: unknown;
  form?: FormData;
  auth?: boolean;
}

export async function request<T>(path: string, options: RequestOptions = {}, retried = false): Promise<T> {
  const headers: Record<string, string> = {};
  const tokens = getTokens();
  if (tokens) headers.Authorization = `Bearer ${tokens.accessToken}`;

  let body: BodyInit | undefined;
  if (options.form) {
    body = options.form;
  } else if (options.body !== undefined) {
    headers["Content-Type"] = "application/json";
    body = JSON.stringify(options.body);
  }

  const response = await fetch(BASE_URL + path, { method: options.method ?? "GET", headers, body });

  if (response.status === 401 && tokens && !retried) {
    const refreshed = await refreshTokens();
    if (refreshed) return request<T>(path, options, true);
  }

  if (response.status === 204) return undefined as T;

  const text = await response.text();
  let result: ServiceResult<T> | null = null;
  try {
    result = text ? (JSON.parse(text) as ServiceResult<T>) : null;
  } catch {
    result = null;
  }

  if (!response.ok) {
    const fallback =
      response.status === 401
        ? "Bu işlem için giriş yapmalısın."
        : response.status === 403
          ? "Bu işlem için yetkin yok."
          : "Sunucuya ulaşılamadı. Biraz sonra tekrar dene.";
    throw new ApiError(response.status, result?.errorMessage?.length ? result.errorMessage.map(translateMessage) : [fallback]);
  }

  return (result?.data ?? undefined) as T;
}

export function toQuery(params: Record<string, string | number | boolean | undefined | null>) {
  const search = new URLSearchParams();
  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== "") search.set(key, String(value));
  });
  const query = search.toString();
  return query ? `?${query}` : "";
}
