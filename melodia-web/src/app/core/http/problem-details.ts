import { HttpErrorResponse } from '@angular/common/http';

export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  errorCode?: string;
  traceId?: string;
  errors?: Record<string, string[]>;
}

export function toProblem(error: unknown): ProblemDetails {
  if (!(error instanceof HttpErrorResponse)) {
    return { title: 'Beklenmeyen bir hata oluştu.' };
  }

  if (error.status === 0) {
    return { status: 0, title: 'Sunucuya ulaşılamadı. Bağlantını kontrol et.' };
  }

  if (error.status === 429) {
    return { status: 429, title: 'Çok fazla deneme yaptın. Biraz bekleyip tekrar dene.' };
  }

  const body = error.error as ProblemDetails | null;
  return {
    ...body,
    status: error.status,
    title: body?.detail ?? body?.title ?? 'Beklenmeyen bir hata oluştu.',
  };
}

export function fieldError(problem: ProblemDetails | null, field: string): string | null {
  if (!problem?.errors) return null;

  const key = Object.keys(problem.errors).find(
    (k) => k.toLowerCase() === field.toLowerCase(),
  );
  return key ? problem.errors[key][0] : null;
}
