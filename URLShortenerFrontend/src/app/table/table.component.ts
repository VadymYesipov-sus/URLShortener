import { Component, OnInit } from '@angular/core';
import { UrlService, ShortenedUrl, ShortenUrlRequest } from '../url.service';
import { CommonModule } from '@angular/common';
import { HttpResponse } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { NgZone } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-table',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './table.component.html',
  styleUrls: ['./table.component.css'],
})
export class TableComponent implements OnInit {
  isAdmin: boolean = false;
  urls: ShortenedUrl[] = [];
  newUrl: ShortenUrlRequest = { url: '' };
  currentUser: string | null = null;

  constructor(
    private urlService: UrlService,
    private router: Router,
    public authService: AuthService,
    private zone: NgZone
  ) {}

  ngOnInit(): void {
    this.zone.run(() => {
      this.isAdmin = this.authService.isUserAdmin();
    });
    this.currentUser = this.authService.getCurrentUserId();
    console.log('Current user id:', this.currentUser);
    this.loadUrls();
    console.log('Is Admin:', this.isAdmin);
  }

  logUserId(userId: string): void {
    console.log('User ID for this URL:', userId);
  }

  loadUrls(): void {
    this.urlService.getUrls().subscribe((data) => {
      console.log('Received URLs:', data);
      this.urls = data.map((url) => ({
        ...url,
        shortUrl: url.shortUrl.replace(/;/g, ''),
      }));
    });
  }

  createUrl(): void {
    if (this.newUrl.url) {
      const userId = this.authService.getCurrentUserId();

      if (!userId) {
        console.error('User ID is not available. Cannot create URL.');
        return;
      }

      this.urlService.createUrl(this.newUrl).subscribe({
        next: (shortenedUrl: string) => {
          const urlEntry: ShortenedUrl = {
            longUrl: this.newUrl.url,
            shortUrl: shortenedUrl,
            code: '',
            createdOnUtc: new Date(),
            userId: userId,
          };
          this.urls.push(urlEntry);
          this.newUrl.url = '';
        },
        error: (error) => {
          if (error.status === 409) {
            alert(
              error.error.message || 'This URL has already been shortened.'
            );
          } else {
            console.error('Error creating URL:', error);
          }
        },
      });
    }
  }

  editUrl(url: ShortenedUrl): void {
    this.router.navigate(['/edit', url.code]);
  }

  deleteUrl(code: string): void {
    this.urlService.deleteUrl(code).subscribe({
      next: () => {
        this.urls = this.urls.filter((url) => url.code !== code);
      },
      error: (error) => {
        console.error('Error deleting URL:', error);
      },
    });
  }

  deleteAllUrls() {
    if (
      confirm(
        'Are you sure you want to delete all URLs? This action cannot be undone. Think twice.'
      )
    ) {
      this.urlService.deleteAllUrls().subscribe({
        next: () => {
          alert('All URLs deleted successfully.');
          this.loadUrls();
        },
        error: (err) => {
          alert('Error deleting URLs: ' + err.message);
        },
      });
    }
  }

  logout(): void {
    this.authService.logout();
  }
}
