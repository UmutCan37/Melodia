import { HttpErrorResponse, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';

import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';

const AUTH_ENDPOINTS = ['/auth/login', '/auth/register', '/auth/refresh'];

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);

  const isApiCall = req.url.startsWith(environment.apiUrl);
  const isAuthCall = AUTH_ENDPOINTS.some((path) => req.url.endsWith(path));

  if (!isApiCall || isAuthCall) {
    return next(req);
  }

  return next(withToken(req, auth.accessToken())).pipe(
    catchError((error: unknown) => {
      const is401 = error instanceof HttpErrorResponse && error.status === 401;
      if (!is401 || !auth.accessToken()) {
        return throwError(() => error);
      }

      return auth.refresh().pipe(
        catchError((refreshError) => {
          auth.logout();
          return throwError(() => refreshError);
        }),
        switchMap((newToken) => next(withToken(req, newToken))),
      );
    }),
  );
};

function withToken(req: HttpRequest<unknown>, token: string | null): HttpRequest<unknown> {
  return token ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : req;
}
