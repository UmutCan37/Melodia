import { Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { catchError, finalize, of, switchMap, tap } from 'rxjs';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { TableModule } from 'primeng/table';

import { AdminService } from '../../../core/api/admin.service';
import { Genre } from '../../../core/api/api.models';
import { CatalogService } from '../../../core/api/catalog.service';
import { ProblemDetails, toProblem } from '../../../core/http/problem-details';

@Component({
  selector: 'app-admin-genres-page',
  imports: [ReactiveFormsModule, ButtonModule, DialogModule, InputTextModule, MessageModule, TableModule],
  templateUrl: './admin-genres-page.html',
  styleUrl: '../admin-crud.scss',
})
export class AdminGenresPage {
  private readonly catalog = inject(CatalogService);
  private readonly admin = inject(AdminService);
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly toast = inject(MessageService);
  private readonly confirm = inject(ConfirmationService);

  private readonly reload = signal(0);
  protected readonly genres = signal<Genre[]>([]);
  protected readonly loading = signal(true);

  protected readonly dialogOpen = signal(false);
  protected readonly editingId = signal<string | null>(null);
  protected readonly saving = signal(false);
  protected readonly problem = signal<ProblemDetails | null>(null);

  protected readonly form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(50)]],
  });

  constructor() {
    toObservable(this.reload)
      .pipe(
        tap(() => this.loading.set(true)),
        switchMap(() => this.catalog.getGenres().pipe(catchError(() => of([])))),
        takeUntilDestroyed(),
      )
      .subscribe((g) => {
        this.genres.set(g);
        this.loading.set(false);
      });
  }

  protected openCreate(): void {
    this.editingId.set(null);
    this.problem.set(null);
    this.form.reset({ name: '' });
    this.dialogOpen.set(true);
  }

  protected openEdit(genre: Genre): void {
    this.editingId.set(genre.id);
    this.problem.set(null);
    this.form.reset({ name: genre.name });
    this.dialogOpen.set(true);
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const name = this.form.getRawValue().name.trim();
    const id = this.editingId();
    const request$ = id ? this.admin.renameGenre(id, name) : this.admin.createGenre(name);

    this.saving.set(true);
    this.problem.set(null);
    request$.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.dialogOpen.set(false);
        this.toast.add({ severity: 'success', summary: id ? 'Tür güncellendi.' : 'Tür eklendi.' });
        this.reload.update((n) => n + 1);
      },
      error: (err) => this.problem.set(toProblem(err)),
    });
  }

  protected remove(genre: Genre): void {
    this.confirm.confirm({
      header: 'Türü sil',
      message: `"${genre.name}" silinecek. Emin misin?`,
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sil',
      rejectLabel: 'Vazgeç',
      acceptButtonProps: { severity: 'danger' },
      rejectButtonProps: { severity: 'secondary', text: true },
      accept: () =>
        this.admin.deleteGenre(genre.id).subscribe({
          next: () => {
            this.toast.add({ severity: 'info', summary: `"${genre.name}" silindi.` });
            this.reload.update((n) => n + 1);
          },
          error: (err) =>
            this.toast.add({ severity: 'error', summary: 'Silinemedi', detail: toProblem(err).title }),
        }),
    });
  }
}
