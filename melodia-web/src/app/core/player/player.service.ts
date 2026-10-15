import { effect, inject, Injectable, signal } from '@angular/core';
import { finalize, Subscription } from 'rxjs';

import { environment } from '../../../environments/environment';
import { CatalogService } from '../api/catalog.service';
import { PackageName } from '../auth/auth.models';
import { AuthService } from '../auth/auth.service';
import { toProblem } from '../http/problem-details';

export interface PlayableSong {
  id: string;
  title: string;
  artistName: string;
  durationSeconds: number;
  requiredPackage?: PackageName;
}

@Injectable({ providedIn: 'root' })
export class PlayerService {
  private readonly catalog = inject(CatalogService);
  private readonly auth = inject(AuthService);
  private readonly audio = new Audio();
  private playSub?: Subscription;

  private readonly currentState = signal<PlayableSong | null>(null);
  private readonly playingState = signal(false);
  private readonly loadingState = signal(false);
  private readonly positionState = signal(0);
  private readonly durationState = signal(0);
  private readonly volumeState = signal(0.8);
  private readonly blockedState = signal<PlayableSong | null>(null);
  private readonly errorState = signal<string | null>(null);

  readonly current = this.currentState.asReadonly();
  readonly isPlaying = this.playingState.asReadonly();
  readonly isLoading = this.loadingState.asReadonly();
  readonly position = this.positionState.asReadonly();
  readonly duration = this.durationState.asReadonly();
  readonly volume = this.volumeState.asReadonly();
  readonly blocked = this.blockedState.asReadonly();
  readonly error = this.errorState.asReadonly();

  constructor() {
    this.audio.volume = this.volumeState();

    this.audio.addEventListener('timeupdate', () => this.positionState.set(this.audio.currentTime));
    this.audio.addEventListener('loadedmetadata', () => this.durationState.set(this.audio.duration));
    this.audio.addEventListener('play', () => this.playingState.set(true));
    this.audio.addEventListener('pause', () => this.playingState.set(false));
    this.audio.addEventListener('ended', () => this.playingState.set(false));
    this.audio.addEventListener('error', () => {
      if (!this.audio.src) return;
      this.playingState.set(false);
      this.errorState.set('Şarkı çalınamadı.');
    });

    effect(() => {
      if (!this.auth.isLoggedIn()) {
        this.stop();
      }
    });
  }

  play(song: PlayableSong): void {
    if (this.currentState()?.id === song.id) {
      this.toggle();
      return;
    }

    this.playSub?.unsubscribe();
    this.loadingState.set(true);
    this.errorState.set(null);

    this.playSub = this.catalog
      .play(song.id)
      .pipe(finalize(() => this.loadingState.set(false)))
      .subscribe({
        next: ({ streamUrl }) => {
          this.currentState.set(song);
          this.positionState.set(0);
          this.durationState.set(song.durationSeconds);
          this.audio.src = this.resolveUrl(streamUrl);
          this.audio.play().catch(() => this.errorState.set('Tarayıcı çalmayı engelledi.'));
        },
        error: (err) => {
          const problem = toProblem(err);
          if (problem.status === 403) {
            this.blockedState.set(song);
          } else {
            this.errorState.set(problem.title ?? 'Şarkı çalınamadı.');
          }
        },
      });
  }

  toggle(): void {
    if (!this.audio.src) return;
    if (this.audio.paused) {
      this.audio.play();
    } else {
      this.audio.pause();
    }
  }

  seek(seconds: number): void {
    this.audio.currentTime = seconds;
  }

  setVolume(value: number): void {
    this.audio.volume = value;
    this.volumeState.set(value);
  }

  dismissBlocked(): void {
    this.blockedState.set(null);
  }

  stop(): void {
    this.playSub?.unsubscribe();
    this.audio.pause();
    this.audio.removeAttribute('src');
    this.audio.load();
    this.currentState.set(null);
    this.positionState.set(0);
  }

  private resolveUrl(url: string): string {
    if (/^https?:\/\//.test(url)) return url;
    return new URL(url, new URL(environment.apiUrl, location.origin)).toString();
  }
}
