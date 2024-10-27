import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-about',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.css']
})
export class AboutComponent {
  aboutText: string = "I've worked 2 days on this project and am very proud of it. How Shorten URL works? It takes long URL from user request, checks if it's an actual URl, then checks if user is authorized for such action and, if there's no duplicates in the database, generates a code to replace URL path segment. Special method randomly picks certain amount of characters from the alphabet string that consist of lower and upper case symbols as well as numbers to create unique code for new short URL. Before returning it, method checks if there's no identical codes in the database generated previously. If there is, it will generate code again until it gets unique one. Then new shorten URL is created using string concatenation. You can easily acces the endpoint with new URL by using method that unwraps short URL by finding its long counterpart in the database.";
  isAdmin: boolean = false;
  editMode: boolean = false;

  constructor(
    private authService: AuthService,
  ) {}


  ngOnInit(): void {
    this.isAdmin = this.authService.isUserAdmin();
    const savedText = localStorage.getItem('aboutText');
    if (savedText) {
      this.aboutText = savedText;
    }
  }

  toggleEdit() {
    this.editMode = !this.editMode;
  }

  saveText(newText: string) {
    this.aboutText = newText;
    this.editMode = false;
    localStorage.setItem('aboutText', newText);
  }

  
}
