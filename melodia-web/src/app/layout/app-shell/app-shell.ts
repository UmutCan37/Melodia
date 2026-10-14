import { Component, computed, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { AvatarModule } from 'primeng/avatar';
import { MenuModule } from 'primeng/menu';
import { PackageBadge } from '../../shared/ui/package-badge/package-badge';
import { AuthService } from '../../core/auth/auth.service';
import { MeService } from '../../core/api/me.service';

interface NavItem {
  label: string;
  icon: string;
  link: string;
  exact?: boolean;
}

@Component({
  selector: 'app-shell',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, AvatarModule, MenuModule, PackageBadge],
  templateUrl: './app-shell.html',
  styleUrl: './app-shell.scss',
})
export class AppShell {
  private readonly auth = inject(AuthService);
  private readonly me = inject(MeService);

  protected readonly user = this.auth.user;
  protected readonly isAdmin = this.auth.isAdmin;

  protected readonly displayName = computed(
    () => this.me.profile()?.displayName ?? this.user()?.email ?? '',
  );

  protected readonly initial = computed(
    () => this.displayName().charAt(0).toLocaleUpperCase('tr-TR') || '?',
  );

  constructor() {
    this.me.loadProfile().subscribe();
  }

  protected readonly nav: NavItem[] = [
    { label: 'Ana sayfa', icon: 'pi pi-home', link: '/', exact: true },
    { label: 'Keşfet', icon: 'pi pi-compass', link: '/discover' },
    { label: 'Listelerim', icon: 'pi pi-list', link: '/playlists' },
    { label: 'Geçmiş', icon: 'pi pi-history', link: '/history' },
  ];

  protected readonly userMenu: MenuItem[] = [
    { label: 'Paketim', icon: 'pi pi-star', routerLink: '/packages' },
    { separator: true },
    { label: 'Çıkış yap', icon: 'pi pi-sign-out', command: () => this.auth.logout() },
  ];
}
