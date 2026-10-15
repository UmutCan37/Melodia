import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { ConfirmationService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';

@Component({
  selector: 'app-admin-layout',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, ConfirmDialogModule],
  providers: [ConfirmationService],
  templateUrl: './admin-layout.html',
  styleUrl: './admin-layout.scss',
})
export class AdminLayout {
  protected readonly tabs = [
    { label: 'Şarkılar', icon: 'pi pi-music', link: 'songs' },
    { label: 'Sanatçılar', icon: 'pi pi-user', link: 'artists' },
    { label: 'Albümler', icon: 'pi pi-circle', link: 'albums' },
    { label: 'Türler', icon: 'pi pi-th-large', link: 'genres' },
  ];
}
