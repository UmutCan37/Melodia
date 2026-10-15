import { Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toObservable, toSignal } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { catchError, of, switchMap, tap } from 'rxjs';

import {
  RecentlyPlayedItem,
  RecommendationItem,
  SongListItem,
} from '../../../core/api/api.models';
import { CatalogService } from '../../../core/api/catalog.service';
import { MeService } from '../../../core/api/me.service';
import { AuthService } from '../../../core/auth/auth.service';
import { canAccess } from '../../../core/auth/packages';
import { PlayerService } from '../../../core/player/player.service';
import { PackageBadge } from '../../../shared/ui/package-badge/package-badge';
import { SongList } from '../../../shared/ui/song-list/song-list';
import { coverArt } from '../../../shared/utils/cover-art';
import { Cover } from '../../../shared/ui/cover/cover';
@Component({
  selector: 'app-home-page',
  imports: [RouterLink, PackageBadge, SongList, Cover],
  templateUrl: './home-page.html',
  styleUrl: './home-page.scss',
})
export class HomePage {
  private readonly catalog = inject(CatalogService);
  private readonly me = inject(MeService);
  private readonly auth = inject(AuthService);
  private readonly player = inject(PlayerService);

  protected readonly profile = this.me.profile;
  protected readonly myPackage = computed(() => this.auth.user()?.package ?? 'Basic');

  protected readonly genres = toSignal(this.catalog.getGenres(), { initialValue: [] });
  protected readonly recent = toSignal(this.me.getRecentlyPlayed(6), { initialValue: [] });
  protected readonly recommendations = toSignal(this.me.getRecommendations(6), {
    initialValue: [],
  });

  protected readonly selectedGenre = signal<string | null>(null);
  protected readonly songs = signal<SongListItem[]>([]);
  protected readonly loadingSongs = signal(true);

  protected readonly lockedCount = computed(
    () => this.songs().filter((s) => !canAccess(this.myPackage(), s.requiredPackage)).length,
  );

  protected readonly art = coverArt;

  constructor() {
    toObservable(this.selectedGenre)
      .pipe(
        tap(() => this.loadingSongs.set(true)),
        switchMap((genreId) =>
          this.catalog
            .getSongs({ genreId: genreId ?? undefined, pageSize: 50 })
            .pipe(catchError(() => of(null))),
        ),
        takeUntilDestroyed(),
      )
      .subscribe((result) => {
        this.songs.set(result?.items ?? []);
        this.loadingSongs.set(false);
      });
  }

  protected playItem(item: RecentlyPlayedItem | RecommendationItem): void {
    this.player.play({
      id: item.songId,
      title: item.title,
      artistName: item.artistName,
      durationSeconds: item.durationSeconds,
    });
  }
}
