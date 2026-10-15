import { Component, computed as signalComputed, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toObservable, toSignal } from '@angular/core/rxjs-interop';
import { FormsModule, NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import {
  catchError,
  concat,
  debounceTime,
  finalize,
  last,
  map,
  Observable,
  of,
  switchMap,
  tap,
} from 'rxjs';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { DrawerModule } from 'primeng/drawer';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { SelectModule } from 'primeng/select';
import { SelectButtonModule } from 'primeng/selectbutton';
import { TableModule } from 'primeng/table';

import { AdminService } from '../../../core/api/admin.service';
import { AlbumListItem, PagedResult, SongListItem } from '../../../core/api/api.models';
import { CatalogService } from '../../../core/api/catalog.service';
import { PackageName } from '../../../core/auth/auth.models';
import { fieldError, ProblemDetails, toProblem } from '../../../core/http/problem-details';
import { DurationPipe } from '../../../shared/pipes/duration-pipe';
import { PackageBadge } from '../../../shared/ui/package-badge/package-badge';

const PAGE_SIZE = 10;
const MAX_FILE_BYTES = 20 * 1024 * 1024;

@Component({
  selector: 'app-admin-songs-page',
  imports: [
    FormsModule,
    ReactiveFormsModule,
    ButtonModule,
    DrawerModule,
    IconFieldModule,
    InputIconModule,
    InputTextModule,
    MessageModule,
    PaginatorModule,
    SelectModule,
    SelectButtonModule,
    TableModule,
    DurationPipe,
    PackageBadge,
  ],
  templateUrl: './admin-songs-page.html',
  styleUrl: './admin-songs-page.scss',
})
export class AdminSongsPage {
  private readonly catalog = inject(CatalogService);
  private readonly admin = inject(AdminService);
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly toast = inject(MessageService);
  private readonly confirm = inject(ConfirmationService);

  protected readonly pageSize = PAGE_SIZE;
  protected readonly tiers: PackageName[] = ['Basic', 'Gold', 'Premium', 'Elit'];

  // Liste
  protected readonly search = signal('');
  protected readonly page = signal(1);
  private readonly reload = signal(0);
  protected readonly result = signal<PagedResult<SongListItem> | null>(null);
  protected readonly loading = signal(true);

  // Form seçenekleri
  protected readonly artists = toSignal(
    this.catalog.getArtists({ pageSize: 100 }).pipe(map((r) => r.items)),
    { initialValue: [] },
  );
  protected readonly genres = toSignal(this.catalog.getGenres(), { initialValue: [] });
  protected readonly albums = signal<AlbumListItem[]>([]);

  // Drawer
  protected readonly drawerOpen = signal(false);
  protected readonly editing = signal<SongListItem | null>(null);
  protected readonly saving = signal(false);
  protected readonly problem = signal<ProblemDetails | null>(null);
  protected readonly file = signal<File | null>(null);
  protected readonly fileError = signal<string | null>(null);

  protected readonly form = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    artistId: ['', Validators.required],
    genreId: ['', Validators.required],
    albumId: this.fb.control<string | null>(null),
    requiredPackage: this.fb.control<PackageName>('Basic'),
    durationSeconds: [0, [Validators.required, Validators.min(1)]],
  });

  constructor() {
    toObservable(this.searchQuery())
      .pipe(
        debounceTime(300),
        tap(() => this.loading.set(true)),
        switchMap((q) =>
          this.catalog
            .getSongs({ search: q.search || undefined, page: q.page, pageSize: PAGE_SIZE })
            .pipe(catchError(() => of(null))),
        ),
        takeUntilDestroyed(),
      )
      .subscribe((r) => {
        this.result.set(r);
        this.loading.set(false);
      });

    this.form.controls.artistId.valueChanges
      .pipe(
        switchMap((artistId) =>
          artistId
            ? this.catalog.getAlbums({ artistId, pageSize: 100 }).pipe(
                map((r) => r.items),
                catchError(() => of([])),
              )
            : of([]),
        ),
        takeUntilDestroyed(),
      )
      .subscribe((albums) => this.albums.set(albums));
  }

  private searchQuery() {
    return signalComputed(() => ({
      search: this.search().trim(),
      page: this.page(),
      tick: this.reload(),
    }));
  }

  // Liste işlemleri
  protected onSearch(value: string): void {
    this.search.set(value);
    this.page.set(1);
  }

  protected onPage(state: PaginatorState): void {
    this.page.set((state.page ?? 0) + 1);
  }

  // Drawer
  protected openCreate(): void {
    this.editing.set(null);
    this.file.set(null);
    this.fileError.set(null);
    this.problem.set(null);
    this.form.reset({
      title: '',
      artistId: '',
      genreId: '',
      albumId: null,
      requiredPackage: 'Basic',
      durationSeconds: 0,
    });
    this.form.controls.artistId.enable();
    this.form.controls.genreId.enable();
    this.drawerOpen.set(true);
  }

  protected openEdit(song: SongListItem): void {
    this.editing.set(song);
    this.problem.set(null);
    this.form.reset({
      title: song.title,
      artistId: song.artistId,
      genreId: song.genreId,
      albumId: song.albumId,
      requiredPackage: song.requiredPackage,
      durationSeconds: song.durationSeconds,
    });
    this.form.controls.artistId.disable();
    this.form.controls.genreId.disable();
    this.drawerOpen.set(true);
  }

  protected onArtistChange(): void {
    this.form.controls.albumId.setValue(null);
  }

  protected onFile(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    this.fileError.set(null);

    if (!file) return;
    if (!file.name.toLowerCase().endsWith('.mp3')) {
      this.fileError.set('Sadece MP3 dosyası yüklenebilir.');
      input.value = '';
      return;
    }
    if (file.size > MAX_FILE_BYTES) {
      this.fileError.set('Dosya en fazla 20 MB olabilir.');
      input.value = '';
      return;
    }

    this.file.set(file);
    if (!this.form.controls.title.value) {
      this.form.controls.title.setValue(file.name.replace(/\.mp3$/i, ''));
    }
    this.readDuration(file);
  }

  private readDuration(file: File): void {
    const url = URL.createObjectURL(file);
    const audio = new Audio();
    audio.preload = 'metadata';
    audio.onloadedmetadata = () => {
      this.form.controls.durationSeconds.setValue(Math.round(audio.duration));
      URL.revokeObjectURL(url);
    };
    audio.onerror = () => URL.revokeObjectURL(url);
    audio.src = url;
  }

  protected serverError(field: string): string | null {
    return fieldError(this.problem(), field);
  }

  protected save(): void {
    const isCreate = this.editing() === null;
    if (isCreate && !this.file()) {
      this.fileError.set('Bir MP3 dosyası seç.');
    }
    if (this.form.invalid || (isCreate && !this.file())) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.getRawValue();
    const request$ = isCreate
      ? this.admin.createSong({ ...v, audio: this.file()! })
      : this.updateExisting(this.editing()!, v.title, v.requiredPackage, v.albumId);

    this.saving.set(true);
    this.problem.set(null);
    request$.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.drawerOpen.set(false);
        this.toast.add({
          severity: 'success',
          summary: isCreate ? 'Şarkı eklendi.' : 'Şarkı güncellendi.',
        });
        this.reload.update((n) => n + 1);
      },
      error: (err) => this.problem.set(toProblem(err)),
    });
  }

  private updateExisting(
    song: SongListItem,
    title: string,
    requiredPackage: PackageName,
    albumId: string | null,
  ): Observable<unknown> {
    const steps: Observable<unknown>[] = [this.admin.updateSong(song.id, { title, requiredPackage })];
    if (albumId !== song.albumId) {
      steps.push(albumId ? this.admin.assignAlbum(song.id, albumId) : this.admin.removeAlbum(song.id));
    }
    return concat(...steps).pipe(last());
  }

  protected remove(song: SongListItem): void {
    this.confirm.confirm({
      header: 'Şarkıyı yayından kaldır',
      message: `"${song.title}" yayından kaldırılacak. Çalma listelerinde "Kullanılamıyor" olarak görünecek.`,
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Kaldır',
      rejectLabel: 'Vazgeç',
      acceptButtonProps: { severity: 'danger' },
      rejectButtonProps: { severity: 'secondary', text: true },
      accept: () =>
        this.admin.deleteSong(song.id).subscribe({
          next: () => {
            this.toast.add({ severity: 'info', summary: `"${song.title}" yayından kaldırıldı.` });
            this.reload.update((n) => n + 1);
          },
          error: (err) =>
            this.toast.add({ severity: 'error', summary: 'Kaldırılamadı', detail: toProblem(err).title }),
        }),
    });
  }
}
