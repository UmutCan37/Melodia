import { Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toObservable, toSignal } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { catchError, of, switchMap, tap } from 'rxjs';
import { PackageBadge } from '../../../shared/ui/package-badge/package-badge';
import { DurationPipe } from '../../../shared/pipes/duration-pipe';
import { SongListItem } from '../../../core/api/api.models';
import { CatalogService } from '../../../core/api/catalog.service';
import { MeService } from '../../../core/api/me.service';
import { AuthService } from '../../../core/auth/auth.service';
import { canAccess } from '../../../core/auth/packages';

@Component({
  selector: 'app-home-page',
  imports: [RouterLink, PackageBadge, DurationPipe],
  templateUrl: './home-page.html',
  styleUrl: './home-page.scss',
})
export class HomePage {
  private readonly catalog = inject(CatalogService);
  private readonly me = inject(MeService);
  private readonly auth = inject(AuthService);

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
    () => this.songs().filter((s) => !this.isUnlocked(s)).length,
  );

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

  protected isUnlocked(song: SongListItem): boolean {
    return canAccess(this.myPackage(), song.requiredPackage);
  }
  protected art(seed: string): string {
  const palette = [
    'linear-gradient(135deg, #1f6f52, #0e2e24)',
    'linear-gradient(135deg, #3b3f57, #171a24)',
    'linear-gradient(135deg, #5b4a1e, #211b0c)',
    'linear-gradient(135deg, #4a2e5e, #1b1124)',
    'linear-gradient(135deg, #1e4a5b, #0c1d24)',
  ];
  let hash = 0;
  for (const ch of seed) {
    hash = (hash * 31 + ch.charCodeAt(0)) | 0;
  }
  return palette[Math.abs(hash) % palette.length];
}
}

