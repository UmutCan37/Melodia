import { DatePipe } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { catchError, of } from 'rxjs';

import { RecentlyPlayedItem } from '../../../core/api/api.models';
import { MeService } from '../../../core/api/me.service';
import { PlayerService } from '../../../core/player/player.service';
import { DurationPipe } from '../../../shared/pipes/duration-pipe';
import { coverArt } from '../../../shared/utils/cover-art';
import { parseUtc } from '../../../shared/utils/date';

@Component({
  selector: 'app-history-page',
  imports: [DatePipe, DurationPipe],
  templateUrl: './history-page.html',
  styleUrl: './history-page.scss',
})
export class HistoryPage {
  private readonly me = inject(MeService);
  protected readonly player = inject(PlayerService);

  private readonly items = toSignal(
    this.me.getRecentlyPlayed(50).pipe(catchError(() => of(null))),
  );

  protected readonly loading = computed(() => this.items() === undefined);
  protected readonly failed = computed(() => this.items() === null);

  protected readonly rows = computed(() =>
    (this.items() ?? []).map((item) => ({ ...item, listenedAt: parseUtc(item.listenedAtUtc) })),
  );

  protected readonly art = coverArt;

  protected play(item: RecentlyPlayedItem): void {
    this.player.play({
      id: item.songId,
      title: item.title,
      artistName: item.artistName,
      durationSeconds: item.durationSeconds,
    });
  }
}
