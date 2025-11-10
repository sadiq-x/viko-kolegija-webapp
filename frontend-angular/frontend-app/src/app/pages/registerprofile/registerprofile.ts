import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ModelUserRegisterRequest } from '../../models/modelUser';

@Component({
  selector: 'app-registerprofile',
  imports: [CommonModule],
  templateUrl: './registerprofile.html',
  styleUrl: './registerprofile.scss',
})
export class RegisterProfile {
  user: ModelUserRegisterRequest | null = null; //Variable of user to set info inside html

  //Load the user info from url state, coming from register page
  constructor(private router: Router) {
    const state = history.state?.user;
    if (!!state) {
      this.user = state;
    } else if (!this.user) {
      this.router.navigate(['/home']);
    }
  }
  //Function to get image from object received, if don't exist, set the default photo
  get imageUrl(): string {
    return this.user?.Image || 'default-user-profile.jpg';
  }
}
