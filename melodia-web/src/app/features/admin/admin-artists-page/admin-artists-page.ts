import { Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { catchError, debounceTime, finalize, of, switchMap, tap } from 'rxjs';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { TextareaModule } from 'primeng/textarea';

import { AdminService } from '../../../core/api/admin.service';
import { ArtistListItem, PagedResult } from '../../../core/api/api.models';
import { CatalogService } from '../../../core/api/catalog.service';
import { ProblemDetails, toProblem } from '../../../core/http/problem-details';

const PAGE_SIZE = 10;

@Component({
  selector: 'app-admin-artists-page',
  imports: [
    ReactiveFormsModule,
    ButtonModule,
    DialogModule,
    IconFieldModule,
    InputIconModule,
    InputTextModule,
    MessageModule,
    PaginatorModule,
    TableModule,
    TextareaModule,
  ],
  templateUrl: './admin-artists-page.html',
  styleUrl: '../admin-crud.scss',
})
export class AdminArtistsPage {
  private readonly catalog = inject(CatalogService);
  private readonly admin = inject(AdminService);
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly toast = inject(MessageService);
  private readonly confirm = inject(ConfirmationService);

  protected readonly pageSize = PAGE_SIZE;
  protected readonly search = signal('');
  protected readonly page = signal(1);
  private readonly reload = signal(0);
  protected readonly result = signal<PagedResult<ArtistListItem> | null>(null);
  protected readonly loading = signal(true);

  protected readonly dialogOpen = signal(false);
  protected readonly editingId = signal<string | null>(null);
  protected readonly saving = signal(false);
  protected readonly problem = signal<ProblemDetails | null>(null);

  protected readonly form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    biography: [''],
  });

  constructor() {
    toObservable(computed(() => ({ s: this.search().trim(), p: this.page(), t: this.reload() })))
      .pipe(
        debounceTime(300),
        tap(() => this.loading.set(true)),
        switchMap((q) =>
          this.catalog
            .getArtists({ search: q.s || undefined, page: q.p, pageSize: PAGE_SIZE })
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
    this.form.reset({ name: '', biography: '' });
    this.dialogOpen.set(true);
  }

  protected openEdit(artist: ArtistListItem): void {
    this.editingId.set(artist.id);
    this.problem.set(null);
    this.form.reset({ name: artist.name, biography: '' });
    this.dialogOpen.set(true);
    this.catalog.getArtist(artist.id).subscribe((d) =>
      this.form.controls.biography.setValue(d.biography ?? ''),
    );
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { name, biography } = this.form.getRawValue();
    const body = { name: name.trim(), biography: biography.trim() || null };
    const id = this.editingId();
    const request$ = id ? this.admin.updateArtist(id, body) : this.admin.createArtist(body);

    this.saving.set(true);
    this.problem.set(null);
    request$.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.dialogOpen.set(false);
        this.toast.add({ severity: 'success', summary: id ? 'Sanatçı güncellendi.' : 'Sanatçı eklendi.' });
        this.reload.update((n) => n + 1);
      },
      error: (err) => this.problem.set(toProblem(err)),
    });
  }

  protected remove(artist: ArtistListItem): void {
    this.confirm.confirm({
      header: 'Sanatçıyı sil',
      message: `"${artist.name}" silinecek. Emin misin?`,
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sil',
      rejectLabel: 'Vazgeç',
      acceptButtonProps: { severity: 'danger' },
      rejectButtonProps: { severity: 'secondary', text: true },
      accept: () =>
        this.admin.deleteArtist(artist.id).subscribe({
          next: () => {
            this.toast.add({ severity: 'info', summary: `"${artist.name}" silindi.` });
            this.reload.update((n) => n + 1);
          },
          error: (err) =>
            this.toast.add({ severity: 'error', summary: 'Silinemedi', detail: toProblem(err).title }),
        }),
    });
  }
}
