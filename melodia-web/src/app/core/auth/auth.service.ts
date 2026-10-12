import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { finalize, map, Observable, shareReplay, switchMap, tap, throwError } from 'rxjs';

import { environment } from '../../../environments/environment';
import { AuthTokens, JwtPayload, LoginRequest, RegisterRequest } from './auth.models';

const STORAGE_KEY = 'melodia.auth';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly baseUrl = `${environment.apiUrl}/auth`;

  private readonly tokens = signal<AuthTokens | null>(readTokens());
  private refreshInFlight$: Observable<string> | null = null;

  readonly user = computed(() => {
    const tokens = this.tokens();
    return tokens ? decodeJwt(tokens.accessToken) : null;
  });

  readonly isLoggedIn = computed(() => this.tokens() !== null);

  readonly isAdmin = computed(() => {
    const role = this.user()?.role;
    return Array.isArray(role) ? role.includes('Admin') : role === 'Admin';
  });

  accessToken(): string | null {
    return this.tokens()?.accessToken ?? null;
  }

  login(request: LoginRequest): Observable<void> {
    return this.http.post<AuthTokens>(`${this.baseUrl}/login`, request).pipe(
      tap((tokens) => this.setTokens(tokens)),
      map(() => undefined),
    );
  }

  register(request: RegisterRequest): Observable<void> {
    return this.http.post<string>(`${this.baseUrl}/register`, request).pipe(
      switchMap(() => this.login({ email: request.email, password: request.password })),
    );
  }

  refresh(): Observable<string> {
    const refreshToken = this.tokens()?.refreshToken;
    if (!refreshToken) {
      return throwError(() => new Error('Refresh token yok.'));
    }

    this.refreshInFlight$ ??= this.http
      .post<AuthTokens>(`${this.baseUrl}/refresh`, { refreshToken })
      .pipe(
        tap((tokens) => this.setTokens(tokens)),
        map((tokens) => tokens.accessToken),
        finalize(() => (this.refreshInFlight$ = null)),
        shareReplay(1),
      );

    return this.refreshInFlight$;
  }

  logout(): void {
    this.setTokens(null);
    this.router.navigateByUrl('/login');
  }

  private setTokens(tokens: AuthTokens | null): void {
    this.tokens.set(tokens);
    if (tokens) {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(tokens));
    } else {
      localStorage.removeItem(STORAGE_KEY);
    }
  }
}

function readTokens(): AuthTokens | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    return raw ? (JSON.parse(raw) as AuthTokens) : null;
  } catch {
    return null;
  }
}

function decodeJwt(token: string): JwtPayload | null {
  try {
    const base64 = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
    const bytes = Uint8Array.from(atob(base64), (c) => c.charCodeAt(0));
    return JSON.parse(new TextDecoder().decode(bytes)) as JwtPayload;
  } catch {
    return null;
  }
}
