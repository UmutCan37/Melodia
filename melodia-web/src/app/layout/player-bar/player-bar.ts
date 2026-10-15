import { Component, computed, inject } from '@angular/core';

import { PlayerService } from '../../core/player/player.service';
import { DurationPipe } from '../../shared/pipes/duration-pipe';
import { Cover } from '../../shared/ui/cover/cover';

@Component({
  selector: 'app-player-bar',
  imports: [DurationPipe, Cover],
  templateUrl: './player-bar.html',
  styleUrl: './player-bar.scss',
})
export class PlayerBar {
  protected readonly player = inject(PlayerService);

  protected readonly progress = computed(() => {
    const duration = this.player.duration();
    return duration > 0 ? (this.player.position() / duration) * 100 : 0;
  });

  protected readonly volumeIcon = computed(() => {
    const v = this.player.volume();
    if (v === 0) return 'pi pi-volume-off';
    return v < 0.5 ? 'pi pi-volume-down' : 'pi pi-volume-up';
  });

  protected onSeek(event: Event): void {
    this.player.seek(Number((event.target as HTMLInputElement).value));
  }

  protected onVolume(event: Event): void {
    this.player.setVolume(Number((event.target as HTMLInputElement).value));
  }
}
