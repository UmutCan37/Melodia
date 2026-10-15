import { DatePipe } from '@angular/common';
import { Component, computed, inject, input, signal } from '@angular/core';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { catchError, finalize, of, switchMap, tap } from 'rxjs';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';

import { PlaylistDetail, PlaylistSong } from '../../../core/api/api.models';
import { PlaylistService } from '../../../core/api/playlist.service';
import { toProblem } from '../../../core/http/problem-details';
import { PlayableSong, PlayerService } from '../../../core/player/player.service';
import { DurationPipe } from '../../../shared/pipes/duration-pipe';
import { PackageBadge } from '../../../shared/ui/package-badge/package-badge';
import { coverArt } from '../../../shared/utils/cover-art';
import { parseUtc } from '../../../shared/utils/date';

@Component({
  selector: 'app-playlist-detail-page',
  imports: [
    DatePipe,
    FormsModule,
    RouterLink,
    ButtonModule,
    ConfirmDialogModule,
    DialogModule,
    InputTextModule,
    DurationPipe,
    PackageBadge,
  ],
  providers: [ConfirmationService],
  templateUrl: './playlist-detail-page.html',
  styleUrl: './playlist-detail-page.scss',
})
export class PlaylistDetailPage {
  private readonly playlists = inject(PlaylistService);
  private readonly router = inject(Router);
  private readonly confirm = inject(ConfirmationService);
  private readonly toast = inject(MessageService);
  protected readonly player = inject(PlayerService);

  readonly id = input.required<string>();

  private readonly reload = signal(0);
  protected readonly playlist = signal<PlaylistDetail | null>(null);
  protected readonly loading = signal(true);
  protected readonly notFound = signal(false);

  protected readonly songs = computed(() =>
    (this.playlist()?.songs ?? []).map((s) => ({ ...s, addedAt: parseUtc(s.addedAtUtc) })),
  );
  protected readonly totalSeconds = computed(() =>
    this.songs().reduce((sum, s) => sum + s.durationSeconds, 0),
  );
  protected readonly unavailableCount = computed(
    () => this.songs().filter((s) => !s.isAvailable).length,
  );

  protected readonly renameOpen = signal(false);
  protected readonly renameValue = signal('');
  protected readonly renaming = signal(false);

  protected readonly art = coverArt;

  constructor() {
    toObservable(computed(() => ({ id: this.id(), tick: this.reload() })))
      .pipe(
        tap(() => this.loading.set(true)),
        switchMap(({ id }) => this.playlists.get(id).pipe(catchError(() => of(null)))),
        takeUntilDestroyed(),
      )
      .subscribe((detail) => {
        this.playlist.set(detail);
        this.notFound.set(detail === null);
        this.loading.set(false);
      });
  }

  protected toPlayable(song: PlaylistSong): PlayableSong {
    return {
      id: song.songId,
      title: song.title,
      artistName: song.artistName,
      durationSeconds: song.durationSeconds,
      requiredPackage: song.requiredPackage,
    };
  }

  protected playAll(): void {
    const first = this.songs().find((s) => s.isAvailable);
    if (first) this.player.play(this.toPlayable(first));
  }

  protected isCurrent(songId: string): boolean {
    return this.player.current()?.id === songId;
  }

  protected openRename(): void {
    this.renameValue.set(this.playlist()?.name ?? '');
    this.renameOpen.set(true);
  }

  protected rename(): void {
    const name = this.renameValue().trim();
    if (!name) return;

    this.renaming.set(true);
    this.playlists
      .rename(this.id(), name)
      .pipe(finalize(() => this.renaming.set(false)))
      .subscribe({
        next: () => {
          this.renameOpen.set(false);
          this.reload.update((n) => n + 1);
        },
        error: (err) => this.showError('Liste adı değiştirilemedi', err),
      });
  }

  protected remove(song: PlaylistSong): void {
    this.playlists.removeSong(this.id(), song.songId).subscribe({
      next: () => {
        this.toast.add({ severity: 'info', summary: `"${song.title}" listeden çıkarıldı.` });
        this.reload.update((n) => n + 1);
      },
      error: (err) => this.showError('Şarkı çıkarılamadı', err),
    });
  }

  protected deleteList(): void {
    this.confirm.confirm({
      header: 'Listeyi sil',
      message: `"${this.playlist()?.name}" listesi kalıcı olarak silinecek. Emin misin?`,
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Evet, sil',
      rejectLabel: 'Vazgeç',
      acceptButtonProps: { severity: 'danger' },
      rejectButtonProps: { severity: 'secondary', text: true },
      accept: () =>
        this.playlists.delete(this.id()).subscribe({
          next: () => {
            this.toast.add({ severity: 'info', summary: 'Liste silindi.' });
            this.router.navigateByUrl('/playlists');
          },
          error: (err) => this.showError('Liste silinemedi', err),
        }),
    });
  }

  private showError(summary: string, err: unknown): void {
    this.toast.add({ severity: 'error', summary, detail: toProblem(err).title });
  }
}
