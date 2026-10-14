import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Genre, PagedResult, PlayResponse, SongListItem, SongQuery } from './api.models';

@Injectable({ providedIn: 'root' })
export class CatalogService {
  private readonly http = inject(HttpClient);
  private readonly api = environment.apiUrl;

  getSongs(query: SongQuery = {}): Observable<PagedResult<SongListItem>> {
    return this.http.get<PagedResult<SongListItem>>(`${this.api}/songs`, {
      params: toParams(query),
    });
  }

  getSong(id: string): Observable<SongListItem> {
    return this.http.get<SongListItem>(`${this.api}/songs/${id}`);
  }

  getGenres(): Observable<Genre[]> {
    return this.http.get<Genre[]>(`${this.api}/genres`);
  }

  play(songId: string): Observable<PlayResponse> {
    return this.http.post<PlayResponse>(`${this.api}/songs/${songId}/play`, null);
  }
}

function toParams(query: object): HttpParams {
  let params = new HttpParams();
  for (const [key, value] of Object.entries(query)) {
    if (value !== undefined && value !== null && value !== '') {
      params = params.set(key, String(value));
    }
  }
  return params;
}
