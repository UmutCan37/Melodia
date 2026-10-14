import { HttpClient, HttpParams } from '@angular/common/http';
import { effect, inject, Injectable, signal } from '@angular/core';
import { map, Observable, switchMap, tap } from 'rxjs';

import { environment } from '../../../environments/environment';
import { PackageName } from '../auth/auth.models';
import { AuthService } from '../auth/auth.service';
import { MeProfile, RecentlyPlayedItem, RecommendationItem } from './api.models';

@Injectable({ providedIn: 'root' })
export class MeService {
  private readonly http = inject(HttpClient);
  private readonly auth = inject(AuthService);
  private readonly api = `${environment.apiUrl}/me`;

  private readonly profileState = signal<MeProfile | null>(null);
  readonly profile = this.profileState.asReadonly();

  constructor() {
    effect(() => {
      if (!this.auth.isLoggedIn()) {
        this.profileState.set(null);
      }
    });
  }

  loadProfile(): Observable<MeProfile> {
    return this.http.get<MeProfile>(this.api).pipe(tap((p) => this.profileState.set(p)));
  }

  getRecentlyPlayed(limit = 10): Observable<RecentlyPlayedItem[]> {
    return this.http.get<RecentlyPlayedItem[]>(`${this.api}/recently-played`, {
      params: new HttpParams().set('limit', limit),
    });
  }

  getRecommendations(limit = 10): Observable<RecommendationItem[]> {
    return this.http.get<RecommendationItem[]>(`${this.api}/recommendations`, {
      params: new HttpParams().set('limit', limit),
    });
  }

  changePackage(pkg: PackageName): Observable<void> {
    return this.http.put(`${this.api}/package`, { package: pkg }).pipe(
      switchMap(() => this.auth.refresh()),
      switchMap(() => this.loadProfile()),
      map(() => undefined),
    );
  }
}
