import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { AuthService } from '../services/auth/auth.service';
import { Router } from '@angular/router';
import { MessageService } from '../services/message/message.service';
import { MatSnackBar } from '@angular/material/snack-bar';


@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.css']
})
export class SignInComponent implements OnInit {
  signInForm!: FormGroup;
  hide = true; // Start with password hidden
  signInError: string | null = null;
  successMessage: string | null = null;


  constructor(private authService: AuthService, private router: Router, private messageService: MessageService, private snackBar: MatSnackBar) { }

  ngOnInit() { // Initialize the form on component creation
    this.signInForm = new FormGroup({
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', Validators.required)
    });

    this.messageService.currentMessage.subscribe(message => this.successMessage = message);
    if (this.successMessage) {
      this.snackBar.open(this.successMessage, 'Close', {
        duration: 5000, // Duration in milliseconds
        horizontalPosition: 'center',
        verticalPosition: 'top',
      });
    }

    
  }

  onSignIn() {
    if (this.signInForm.valid) {
      const { email, password } = this.signInForm.value;
      this.authService.signIn(email, password).subscribe({
        next: (response) => {
          console.log('Sign in successful', response);
          this.router.navigate(['/task-manager']);
        },
        error: (error) => {
          console.error('Sign in failed', error);
          this.signInError = 'Incorrect username or password'; 
        }
      });
    } else {
      this.signInError = 'Please fill in all required fields correctly.';
    }
  }
}
