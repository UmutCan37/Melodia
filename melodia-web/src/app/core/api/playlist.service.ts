import { HttpClient } from '@angular/common/http';
import { effect, inject, Injectable, signal } from '@angular/core';
import { map, Observable, tap } from 'rxjs';

import { environment } from '../../../environments/environment';
import { AuthService } from '../auth/auth.service';
import { PlaylistDetail, PlaylistSummary } from './api.models';

@Injectable({ providedIn: 'root' })
export class PlaylistService {
  private readonly http = inject(HttpClient);
  private readonly auth = inject(AuthService);
  private readonly api = environment.apiUrl;

  private readonly mineState = signal<PlaylistSummary[]>([]);
  readonly mine = this.mineState.asReadonly();
  private loaded = false;

  constructor() {
    effect(() => {
      if (!this.auth.isLoggedIn()) {
        this.mineState.set([]);
        this.loaded = false;
      }
    });
  }

  ensureLoaded(): void {
    if (!this.loaded) {
      this.loaded = true;
      this.refresh();
    }
  }

  loadMine(): Observable<PlaylistSummary[]> {
    return this.http
      .get<PlaylistSummary[]>(`${this.api}/me/playlists`)
      .pipe(tap((lists) => this.mineState.set(lists)));
  }

  get(id: string): Observable<PlaylistDetail> {
    return this.http.get<PlaylistDetail>(`${this.api}/playlists/${id}`);
  }

  create(name: string): Observable<string> {
    return this.http.post<{ id: string }>(`${this.api}/playlists`, { name }).pipe(
      map((r) => r.id),
      tap(() => this.refresh()),
    );
  }

  rename(id: string, name: string): Observable<unknown> {
    return this.http.put(`${this.api}/playlists/${id}`, { name }).pipe(tap(() => this.refresh()));
  }

  delete(id: string): Observable<unknown> {
    return this.http.delete(`${this.api}/playlists/${id}`).pipe(tap(() => this.refresh()));
  }

  addSong(id: string, songId: string): Observable<unknown> {
    return this.http
      .post(`${this.api}/playlists/${id}/songs`, { songId })
      .pipe(tap(() => this.refresh()));
  }

  removeSong(id: string, songId: string): Observable<unknown> {
    return this.http
      .delete(`${this.api}/playlists/${id}/songs/${songId}`)
      .pipe(tap(() => this.refresh()));
  }

  private refresh(): void {
    this.loadMine().subscribe();
  }
}
