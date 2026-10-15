import { Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toObservable, toSignal } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { catchError, debounceTime, distinctUntilChanged, of, switchMap, tap } from 'rxjs';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { SelectModule } from 'primeng/select';

import { PagedResult, SongListItem } from '../../../core/api/api.models';
import { CatalogService } from '../../../core/api/catalog.service';
import { SongList } from '../../../shared/ui/song-list/song-list';

const PAGE_SIZE = 10;

@Component({
  selector: 'app-discover-page',
  imports: [
    FormsModule,
    IconFieldModule,
    InputIconModule,
    InputTextModule,
    PaginatorModule,
    SelectModule,
    SongList,
  ],
  templateUrl: './discover-page.html',
  styleUrl: './discover-page.scss',
})
export class DiscoverPage {
  private readonly catalog = inject(CatalogService);

  protected readonly pageSize = PAGE_SIZE;
  protected readonly genres = toSignal(this.catalog.getGenres(), { initialValue: [] });

  protected readonly search = signal('');
  protected readonly genreId = signal<string | null>(null);
  protected readonly page = signal(1);

  protected readonly result = signal<PagedResult<SongListItem> | null>(null);
  protected readonly loading = signal(true);

  private readonly query = computed(() => ({
    search: this.search().trim(),
    genreId: this.genreId(),
    page: this.page(),
  }));

  constructor() {
    toObservable(this.query)
      .pipe(
        debounceTime(300),
        distinctUntilChanged((a, b) => JSON.stringify(a) === JSON.stringify(b)),
        tap(() => this.loading.set(true)),
        switchMap((q) =>
          this.catalog
            .getSongs({
              search: q.search || undefined,
              genreId: q.genreId ?? undefined,
              page: q.page,
              pageSize: PAGE_SIZE,
            })
            .pipe(catchError(() => of(null))),
        ),
        takeUntilDestroyed(),
      )
      .subscribe((result) => {
        this.result.set(result);
        this.loading.set(false);
      });
  }

  protected onSearch(value: string): void {
    this.search.set(value);
    this.page.set(1);
  }

  protected onGenre(id: string | null): void {
    this.genreId.set(id);
    this.page.set(1);
  }

  protected onPage(state: PaginatorState): void {
    this.page.set((state.page ?? 0) + 1);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}
