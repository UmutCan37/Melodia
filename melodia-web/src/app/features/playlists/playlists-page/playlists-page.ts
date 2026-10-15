import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';

import { PlaylistService } from '../../../core/api/playlist.service';
import { ProblemDetails, toProblem } from '../../../core/http/problem-details';
import { coverArt } from '../../../shared/utils/cover-art';

@Component({
  selector: 'app-playlists-page',
  imports: [FormsModule, RouterLink, ButtonModule, DialogModule, InputTextModule, MessageModule],
  templateUrl: './playlists-page.html',
  styleUrl: './playlists-page.scss',
})
export class PlaylistsPage {
  private readonly playlists = inject(PlaylistService);
  private readonly router = inject(Router);

  protected readonly lists = this.playlists.mine;
  protected readonly loading = signal(true);
  protected readonly art = coverArt;

  protected readonly dialogOpen = signal(false);
  protected readonly newName = signal('');
  protected readonly saving = signal(false);
  protected readonly problem = signal<ProblemDetails | null>(null);

  constructor() {
    this.playlists
      .loadMine()
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe();
  }

  protected openCreate(): void {
    this.newName.set('');
    this.problem.set(null);
    this.dialogOpen.set(true);
  }

  protected create(): void {
    const name = this.newName().trim();
    if (!name) return;

    this.saving.set(true);
    this.playlists
      .create(name)
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: (id) => {
          this.dialogOpen.set(false);
          this.router.navigate(['/playlists', id]);
        },
        error: (err) => this.problem.set(toProblem(err)),
      });
  }
}
