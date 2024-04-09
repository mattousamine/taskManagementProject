import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SignInComponent } from './sign-in/sign-in.component';
import { SignUpComponent } from './sign-up/sign-up.component';
import { TaskManagerComponent } from './task-manager/task-manager.component';
import { AuthGuardService } from './services/auth-guard/auth-guard.service';

export const routes: Routes = [
  { path: 'sign-in', component: SignInComponent },
  { path: 'sign-up', component: SignUpComponent },
  { path: 'task-manager', component: TaskManagerComponent, canActivate: [AuthGuardService] },
  { path: '', redirectTo: '/sign-in', pathMatch: 'full' },
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
  })
export class AppRoutingModule { }
