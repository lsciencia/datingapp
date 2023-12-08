import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {}

  constructor(public accountService: AccountService,
    private router: Router,
    private toastr: ToastrService) {

  }
  ngOnInit(): void {
  }

  login() {
    this.accountService.login(this.model)
      .subscribe({
        next: () => {
          console.log(this.model);
          this.router.navigateByUrl('/members');
        },
        error: (err) => {
          console.log(err);
          console.log(this.model);
        }
      })
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

}
