import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/authService';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})

export class Login {

  form!: FormGroup;

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {
    this.form = this.fb.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required]],
      remember: [false],
    });
  }

  onLoginSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const payload = this.form.value; // { username, password, remember }

    var credentials = {
      "Username": this.form.value.username,
      "PasswordHash": this.form.value.password
    };

    this.authService.login(credentials).subscribe({
      next: (status: boolean) => {
      }, error: (e) => {
        console.log(e)
      }
    })
  }
}

