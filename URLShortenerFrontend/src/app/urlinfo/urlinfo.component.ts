import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UrlService } from '../url.service';
import { ShortenedUrl } from '../url.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-url-info',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './urlinfo.component.html',
  styleUrls: ['./urlinfo.component.css'],
})
export class UrlinfoComponent implements OnInit {
  urlDetails?: ShortenedUrl;
  errorMessage: string | null = null;

  constructor(private route: ActivatedRoute, private urlService: UrlService) {}

  ngOnInit(): void {
    const code = this.route.snapshot.paramMap.get('code');
    if (code) {
      this.urlService.getUrlByCode(code).subscribe({
        next: (data) => (this.urlDetails = data),
        error: (error) => console.error('Error fetching URL details:', error),
      });
    }
  }

  loadUrlDetails(code: string): void {
    this.urlService.getUrlByCode(code).subscribe({
      next: (data) => {
        this.urlDetails = data;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load URL details.';
        console.error('Error loading URL details:', error);
      },
    });
  }
}
