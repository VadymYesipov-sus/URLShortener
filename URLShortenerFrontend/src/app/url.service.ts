import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ShortenedUrl {
  longUrl: string;
  shortUrl: string;
  code: string;
  createdOnUtc: Date;
  userId: string;
}

export interface ShortenUrlRequest {
  url: string;
}

@Injectable({
  providedIn: 'root',
})
export class UrlService {
  private apiUrl = 'https://localhost:7178/api';

  constructor(private http: HttpClient) {}

  getUrls(): Observable<ShortenedUrl[]> {
    return this.http.get<ShortenedUrl[]>(`${this.apiUrl}/url/all/`, {
      withCredentials: true,
    });
  }

  getUrlByCode(code: string): Observable<ShortenedUrl> {
    return this.http.get<ShortenedUrl>(`${this.apiUrl}/url/info/${code}`);
  }

  createUrl(request: ShortenUrlRequest): Observable<string> {
    return this.http.post<string>(`${this.apiUrl}/url/shorten`, request, {
      responseType: 'text' as 'json',
      withCredentials: true,
    });
  }

  deleteUrl(code: string): Observable<HttpResponse<void>> {
    return this.http.delete<void>(`${this.apiUrl}/url/${code}`, {
      observe: 'response',
    });
  }

  deleteAllUrls(): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/url/all`, {
      withCredentials: true,
    });
  }

  checkAdmin(): Observable<{ isAdmin: boolean }> {
    return this.http.get<{ isAdmin: boolean }>(
      `${this.apiUrl}/Account/is-admin`
    );
  }
}
