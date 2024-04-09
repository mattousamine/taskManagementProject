import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { AuthService } from '../services/auth/auth.service';
import { Router } from '@angular/router';
import { MessageService } from '../services/message/message.service';

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.css']
})
export class SignUpComponent implements OnInit {
  signUpForm!: FormGroup;
  hide = true; // Start with password hidden

  constructor(private authService: AuthService, private router: Router, private messageService: MessageService) { }

  ngOnInit() {
    this.signUpForm = new FormGroup({
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required, Validators.minLength(6)]), 
      confirmPassword: new FormControl('', Validators.required) 
    });
  }

  onSignUp() {
    if (this.signUpForm.valid) {
      if (this.signUpForm.value.password === this.signUpForm.value.confirmPassword) {
        const { email, password } = this.signUpForm.value;
        this.authService.signUp(email, password).subscribe({
          next: (response) => {
            console.log('Signup successful', response);
            this.messageService.changeMessage('Signup successful, please log in.');
            this.router.navigate(['/sign-in']);

            
          },
          error: (error) => {
            console.error('Signup failed', error);
            
          }
        });
      } else {
        console.error('Passwords do not match');
      }
    } else {
    }
  }
}
