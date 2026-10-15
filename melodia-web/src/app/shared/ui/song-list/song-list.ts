import { Component, computed, inject, input, signal } from '@angular/core';

import { SongListItem } from '../../../core/api/api.models';
import { AuthService } from '../../../core/auth/auth.service';
import { canAccess } from '../../../core/auth/packages';
import { PlayerService } from '../../../core/player/player.service';
import { DurationPipe } from '../../pipes/duration-pipe';
import { coverArt } from '../../utils/cover-art';
import { PackageBadge } from '../package-badge/package-badge';
import { MenuItem, MessageService } from 'primeng/api';
import { Menu, MenuModule } from 'primeng/menu';

import { PlaylistSummary } from '../../../core/api/api.models';
import { PlaylistService } from '../../../core/api/playlist.service';
import { toProblem } from '../../../core/http/problem-details';
@Component({
  selector: 'app-song-list',
  imports: [PackageBadge, DurationPipe, MenuModule],
  templateUrl: './song-list.html',
  styleUrl: './song-list.scss',
})
export class SongList {
  private readonly auth = inject(AuthService);
  protected readonly player = inject(PlayerService);

    private readonly playlists = inject(PlaylistService);
  private readonly toast = inject(MessageService);

  private readonly target = signal<SongListItem | null>(null);

  protected readonly addMenu = computed<MenuItem[]>(() => {
    const lists = this.playlists.mine();
    if (lists.length === 0) {
      return [{ label: 'Henüz listen yok', disabled: true }, { label: 'Liste oluştur', icon: 'pi pi-plus', routerLink: '/playlists' }];
    }
    return [
      {
        label: 'Listeye ekle',
        items: lists.map((list) => ({
          label: list.name,
          icon: 'pi pi-list',
          command: () => this.addTo(list),
        })),
      },
    ];
  });

  constructor() {
    this.playlists.ensureLoaded();
  }

  protected openAdd(event: Event, song: SongListItem, menu: Menu): void {
    this.target.set(song);
    menu.toggle(event);
  }

  private addTo(list: PlaylistSummary): void {
    const song = this.target();
    if (!song) return;

    this.playlists.addSong(list.id, song.id).subscribe({
      next: () =>
        this.toast.add({ severity: 'success', summary: `"${song.title}" → ${list.name}` }),
      error: (err) =>
        this.toast.add({ severity: 'error', summary: 'Eklenemedi', detail: toProblem(err).title }),
    });
  }

  readonly songs = input.required<SongListItem[]>();
  readonly startIndex = input(0);

  private readonly myPackage = computed(() => this.auth.user()?.package ?? 'Basic');

  protected readonly art = coverArt;

  protected isUnlocked(song: SongListItem): boolean {
    return canAccess(this.myPackage(), song.requiredPackage);
  }

  protected isCurrent(songId: string): boolean {
    return this.player.current()?.id === songId;
  }
}
