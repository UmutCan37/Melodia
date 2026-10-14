import { PackageName } from '../auth/auth.models';

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface SongListItem {
  id: string;
  title: string;
  durationSeconds: number;
  artistId: string;
  artistName: string;
  genreId: string;
  genreName: string;
  albumId: string | null;
  albumTitle: string | null;
  requiredPackage: PackageName;
}

export interface SongQuery {
  search?: string;
  artistId?: string;
  genreId?: string;
  albumId?: string;
  page?: number;
  pageSize?: number;
}

export interface Genre {
  id: string;
  name: string;
}

export interface MeProfile {
  id: string;
  email: string;
  displayName: string;
  package: PackageName;
  registeredAtUtc: string;
}

export interface RecentlyPlayedItem {
  songId: string;
  title: string;
  artistId: string;
  artistName: string;
  durationSeconds: number;
  listenedAtUtc: string;
}

export interface RecommendationItem {
  songId: string;
  title: string;
  artistId: string;
  artistName: string;
  genreName: string;
  durationSeconds: number;
  reason: string;
}

export interface PlayResponse {
  songId: string;
  streamUrl: string;
}
