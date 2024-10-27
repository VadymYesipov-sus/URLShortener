import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { RouterModule } from '@angular/router';
import { UrlService } from './url.service';
import { CommonModule } from '@angular/common';
import { NgIf } from '@angular/common';
import { AuthService } from './auth.service';


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterModule, CommonModule, NgIf
  ],
  templateUrl: './app.component.html',
  styles: [],
})
export class AppComponent {
  title = 'URLShortenerFrontend';
  
  isAdmin: boolean = false;

  constructor(private urlService: UrlService, public authService: AuthService) {}

  ngOnInit(): void {
    this.urlService.checkAdmin().subscribe({
      next: (response) => (this.isAdmin = response.isAdmin),
      error: (error) => console.error('Error checking admin role:', error),
    });
  }

  logout(): void {
    this.authService.logout();
  }
}



