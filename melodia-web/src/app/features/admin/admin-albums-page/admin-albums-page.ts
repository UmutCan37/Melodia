import { DatePipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toObservable, toSignal } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { catchError, debounceTime, finalize, map, of, switchMap, tap } from 'rxjs';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';

import { AdminService } from '../../../core/api/admin.service';
import { AlbumListItem, PagedResult } from '../../../core/api/api.models';
import { CatalogService } from '../../../core/api/catalog.service';
import { ProblemDetails, toProblem } from '../../../core/http/problem-details';

const PAGE_SIZE = 10;

@Component({
  selector: 'app-admin-albums-page',
  imports: [
    DatePipe,
    ReactiveFormsModule,
    ButtonModule,
    DialogModule,
    IconFieldModule,
    InputIconModule,
    InputTextModule,
    MessageModule,
    PaginatorModule,
    SelectModule,
    TableModule,
  ],
  templateUrl: './admin-albums-page.html',
  styleUrl: '../admin-crud.scss',
})
export class AdminAlbumsPage {
  private readonly catalog = inject(CatalogService);
  private readonly admin = inject(AdminService);
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly toast = inject(MessageService);
  private readonly confirm = inject(ConfirmationService);

  protected readonly pageSize = PAGE_SIZE;
  protected readonly search = signal('');
  protected readonly page = signal(1);
  private readonly reload = signal(0);
  protected readonly result = signal<PagedResult<AlbumListItem> | null>(null);
  protected readonly loading = signal(true);

  protected readonly artists = toSignal(
    this.catalog.getArtists({ pageSize: 100 }).pipe(map((r) => r.items)),
    { initialValue: [] },
  );

  protected readonly dialogOpen = signal(false);
  protected readonly editingId = signal<string | null>(null);
  protected readonly saving = signal(false);
  protected readonly problem = signal<ProblemDetails | null>(null);

  protected readonly form = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    artistId: ['', Validators.required],
    releaseDate: ['', Validators.required],
  });

  constructor() {
    toObservable(computed(() => ({ s: this.search().trim(), p: this.page(), t: this.reload() })))
      .pipe(
        debounceTime(300),
        tap(() => this.loading.set(true)),
        switchMap((q) =>
          this.catalog
            .getAlbums({ search: q.s || undefined, page: q.p, pageSize: PAGE_SIZE })
            .pipe(catchError(() => of(null))),
        ),
        takeUntilDestroyed(),
      )
      .subscribe((r) => {
        this.result.set(r);
        this.loading.set(false);
      });
  }

  protected onSearch(value: string): void {
    this.search.set(value);
    this.page.set(1);
  }

  protected onPage(state: PaginatorState): void {
    this.page.set((state.page ?? 0) + 1);
  }

  protected openCreate(): void {
    this.editingId.set(null);
    this.problem.set(null);
    this.form.reset({ title: '', artistId: '', releaseDate: '' });
    this.form.controls.artistId.enable();
    this.dialogOpen.set(true);
  }

  protected openEdit(album: AlbumListItem): void {
    this.editingId.set(album.id);
    this.problem.set(null);
    this.form.reset({ title: album.title, artistId: album.artistId, releaseDate: album.releaseDate });
    this.form.controls.artistId.disable();
    this.dialogOpen.set(true);
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { title, artistId, releaseDate } = this.form.getRawValue();
    const id = this.editingId();
    const request$ = id
      ? this.admin.updateAlbum(id, { title: title.trim(), releaseDate })
      : this.admin.createAlbum({ title: title.trim(), artistId, releaseDate });

    this.saving.set(true);
    this.problem.set(null);
    request$.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.dialogOpen.set(false);
        this.toast.add({ severity: 'success', summary: id ? 'Albüm güncellendi.' : 'Albüm eklendi.' });
        this.reload.update((n) => n + 1);
      },
      error: (err) => this.problem.set(toProblem(err)),
    });
  }

  protected remove(album: AlbumListItem): void {
    this.confirm.confirm({
      header: 'Albümü sil',
      message: `"${album.title}" silinecek. İçindeki şarkılar silinmez, sadece albümden çıkar.`,
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sil',
      rejectLabel: 'Vazgeç',
      acceptButtonProps: { severity: 'danger' },
      rejectButtonProps: { severity: 'secondary', text: true },
      accept: () =>
        this.admin.deleteAlbum(album.id).subscribe({
          next: () => {
            this.toast.add({ severity: 'info', summary: `"${album.title}" silindi.` });
            this.reload.update((n) => n + 1);
          },
          error: (err) =>
            this.toast.add({ severity: 'error', summary: 'Silinemedi', detail: toProblem(err).title }),
        }),
    });
  }
}
