import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { PackageName } from '../auth/auth.models';

export interface CreateSongRequest {
  title: string;
  durationSeconds: number;
  artistId: string;
  genreId: string;
  albumId: string | null;
  requiredPackage: PackageName;
  audio: File;
}

@Injectable({ providedIn: 'root' })
export class AdminService {
  private readonly http = inject(HttpClient);
  private readonly api = environment.apiUrl;

  // Şarkılar
  createSong(req: CreateSongRequest): Observable<unknown> {
    const data = new FormData();
    data.append('title', req.title);
    data.append('durationSeconds', String(req.durationSeconds));
    data.append('artistId', req.artistId);
    data.append('genreId', req.genreId);
    if (req.albumId) data.append('albumId', req.albumId);
    data.append('requiredPackage', req.requiredPackage);
    data.append('audio', req.audio);
    return this.http.post(`${this.api}/songs`, data);
  }

  updateSong(id: string, body: { title: string; requiredPackage: PackageName }): Observable<unknown> {
    return this.http.put(`${this.api}/songs/${id}`, body);
  }

  deleteSong(id: string): Observable<unknown> {
    return this.http.delete(`${this.api}/songs/${id}`);
  }

  assignAlbum(songId: string, albumId: string): Observable<unknown> {
    return this.http.put(`${this.api}/songs/${songId}/album`, { albumId });
  }

  removeAlbum(songId: string): Observable<unknown> {
    return this.http.delete(`${this.api}/songs/${songId}/album`);
  }

  // Sanatçılar
  createArtist(body: { name: string; biography: string | null }): Observable<unknown> {
    return this.http.post(`${this.api}/artists`, body);
  }

  updateArtist(id: string, body: { name: string; biography: string | null }): Observable<unknown> {
    return this.http.put(`${this.api}/artists/${id}`, body);
  }

  deleteArtist(id: string): Observable<unknown> {
    return this.http.delete(`${this.api}/artists/${id}`);
  }

  // Albümler
  createAlbum(body: { title: string; artistId: string; releaseDate: string }): Observable<unknown> {
    return this.http.post(`${this.api}/albums`, body);
  }

  updateAlbum(id: string, body: { title: string; releaseDate: string }): Observable<unknown> {
    return this.http.put(`${this.api}/albums/${id}`, body);
  }

  deleteAlbum(id: string): Observable<unknown> {
    return this.http.delete(`${this.api}/albums/${id}`);
  }

  // Türler
  createGenre(name: string): Observable<unknown> {
    return this.http.post(`${this.api}/genres`, { name });
  }

  renameGenre(id: string, name: string): Observable<unknown> {
    return this.http.put(`${this.api}/genres/${id}`, { name });
  }

  deleteGenre(id: string): Observable<unknown> {
    return this.http.delete(`${this.api}/genres/${id}`);
  }
}
