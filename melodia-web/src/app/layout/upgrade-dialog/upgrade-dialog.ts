import { Component, computed, effect, inject, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { MessageModule } from 'primeng/message';

import { MeService } from '../../core/api/me.service';
import { PackageName } from '../../core/auth/auth.models';
import { AuthService } from '../../core/auth/auth.service';
import { canAccess, PACKAGE_LEVEL } from '../../core/auth/packages';
import { ProblemDetails, toProblem } from '../../core/http/problem-details';
import { PlayerService } from '../../core/player/player.service';
import { PackageBadge } from '../../shared/ui/package-badge/package-badge';

const ALL_TIERS: PackageName[] = ['Basic', 'Gold', 'Premium', 'Elit'];

@Component({
  selector: 'app-upgrade-dialog',
  imports: [DialogModule, ButtonModule, MessageModule, PackageBadge],
  templateUrl: './upgrade-dialog.html',
  styleUrl: './upgrade-dialog.scss',
})
export class UpgradeDialog {
  private readonly auth = inject(AuthService);
  private readonly me = inject(MeService);
  private readonly player = inject(PlayerService);

  protected readonly song = this.player.blocked;
  protected readonly visible = computed(() => this.song() !== null);
  protected readonly currentPackage = computed(() => this.auth.user()?.package ?? 'Basic');

  protected readonly options = computed(() =>
    ALL_TIERS.filter((t) => PACKAGE_LEVEL[t] > PACKAGE_LEVEL[this.currentPackage()]),
  );

  protected readonly selected = signal<PackageName>('Elit');
  protected readonly saving = signal(false);
  protected readonly problem = signal<ProblemDetails | null>(null);

  constructor() {
    effect(() => {
      const song = this.song();
      if (song) {
        this.selected.set(song.requiredPackage ?? 'Elit');
        this.problem.set(null);
      }
    });
  }

  protected unlocks(tier: PackageName): boolean {
    const required = this.song()?.requiredPackage;
    return required ? canAccess(tier, required) : true;
  }

  protected close(): void {
    this.player.dismissBlocked();
  }

  protected upgrade(): void {
    const song = this.song();
    if (!song) return;

    const tier = this.selected();
    this.saving.set(true);
    this.problem.set(null);

    this.me
      .changePackage(tier)
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => {
          this.player.dismissBlocked();
          if (this.unlocks(tier)) {
            this.player.play(song);
          }
        },
        error: (err) => this.problem.set(toProblem(err)),
      });
  }
}
